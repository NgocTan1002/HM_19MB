using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HM_19MB_Demo.Data;
using HM_19MB_Demo.Models;
using HM_19MB_Demo.Services;

namespace HM_19MB_Demo
{
    public partial class UncertaintyCalculationForm : Form
    {
        private int _j = 3;
        private int _n = 5;
        private int _phienId;
        private readonly Func<CalibrationResultRow, Task>? _onResultAdded;
        private readonly Action<int, int>? _onConfigChanged;
        private bool _updatingConfigFromOwner;
        private bool _suppressGridEvents;
        private double[] _corrections = new double[3];
        private readonly List<TextBox> _correctionBoxes = new();

        private CalibrationResultRow? _lastCalculatedResult;
        private UncertaintyFullResult? _lastFullResult;
        private UncertaintyInput? _lastInput;
        private bool _editMode;

        private DataGridView _gridResults = null!;
        private DataGridView _gridBudget = null!;

        private int ChannelColumn(int channelIndex) => channelIndex + 1;
        private int Ttn1Column => _j + 1;
        private int Ttn2Column => _j + 2;
        private int TtnMeanColumn => _j + 3;

        public UncertaintyCalculationForm()
            : this(0, null)
        {
        }

        public UncertaintyCalculationForm(int phienId, Func<CalibrationResultRow, Task>? onResultAdded)
            : this(phienId, onResultAdded, null)
        {
        }

        public UncertaintyCalculationForm(
            int phienId,
            Func<CalibrationResultRow, Task>? onResultAdded,
            Action<int, int>? onConfigChanged)
        {
            _phienId = phienId;
            _onResultAdded = onResultAdded;
            _onConfigChanged = onConfigChanged;

            InitializeComponent();
            btnAddToTable = btnCalculateAndAdd;
            InitResultsGrid();
            InitBudgetGrid();
            BuildResultCards();
            WireEvents();
            ApplyConfiguration();

            btnAddToTable.Enabled = false;
        }

        public UncertaintyCalculationForm(int kenhCount, int? phienId, Action<CalibrationResultRow>? onResultAdded)
            : this(kenhCount, 5, phienId, onResultAdded, null)
        {
        }

        public UncertaintyCalculationForm(
            int kenhCount,
            int measurementCount,
            int? phienId,
            Action<CalibrationResultRow>? onResultAdded,
            Action<int, int>? onConfigChanged)
            : this(phienId ?? 0, row =>
            {
                onResultAdded?.Invoke(row);
                return Task.CompletedTask;
            }, onConfigChanged)
        {
            SetConfiguration(kenhCount, measurementCount, notifyOwner: false);
        }

        public void SetConfiguration(int kenhCount, int measurementCount)
            => SetConfiguration(kenhCount, measurementCount, notifyOwner: false);

        private void SetConfiguration(int kenhCount, int measurementCount, bool notifyOwner)
        {
            int newJ = Math.Max((int)numChannels.Minimum, Math.Min((int)numChannels.Maximum, kenhCount));
            int newN = Math.Max((int)numMeasurements.Minimum, Math.Min((int)numMeasurements.Maximum, measurementCount));
            bool changed = newJ != _j || newN != _n;

            _j = newJ;
            _n = newN;

            _updatingConfigFromOwner = !notifyOwner;
            try
            {
                numChannels.Value = newJ;
                numMeasurements.Value = newN;
            }
            finally
            {
                _updatingConfigFromOwner = false;
            }

            RebuildGridData();

            _lastCalculatedResult = null;
            _lastFullResult = null;
            _lastInput = null;
            btnCalculateAndAdd.Enabled = false;
            lblStatus.Text = string.Empty;

            if (notifyOwner && !_updatingConfigFromOwner)
                _onConfigChanged?.Invoke(_j, _n);
        }

        private void WireEvents()
        {
            btnApplyConfig.Click += BtnApplyConfig_Click;
            btnCalculateAndAdd.Click += BtnCalculateAndAdd_Click;

            gridData.CellValueChanged += GridData_CellValueChanged;
            gridData.CellEndEdit += GridData_CellEndEdit;
            gridData.CurrentCellDirtyStateChanged += GridData_CurrentCellDirtyStateChanged;
            gridData.Resize += (s, e) => FitGridDataLayout();

            rbUseU.CheckedChanged += InputValueChanged;
            rbUseDelta.CheckedChanged += InputValueChanged;
            txtGiaTriDat.TextChanged += InputValueChanged;
            txtUMax.TextChanged += InputValueChanged;
            txtDeltaMax.TextChanged += InputValueChanged;
            txtResA.TextChanged += InputValueChanged;
            txtResD.TextChanged += InputValueChanged;
        }

        private void BtnApplyConfig_Click(object? sender, EventArgs e)
        {
            SetConfiguration((int)numChannels.Value, (int)numMeasurements.Value, notifyOwner: true);
            RebuildGridData();
        }

        private void InputValueChanged(object? sender, EventArgs e)
        {
            if (_suppressGridEvents) return;
            RecalculateAll(showErrors: false);
        }

        private void ApplyConfiguration()
        {
            RebuildCorrectionStrip();
            RebuildGridData();
        }

        private void RebuildCorrectionStrip()
        {
            double[] previous = _corrections;
            _corrections = new double[_j];
            for (int i = 0; i < _j && i < previous.Length; i++)
                _corrections[i] = previous[i];

            while (correctionStrip.Controls.Count > 1)
                correctionStrip.Controls.RemoveAt(1);

            _correctionBoxes.Clear();
            for (int j = 0; j < _j; j++)
            {
                var label = new Label
                {
                    AutoSize = true,
                    Margin = new Padding(j == 0 ? 0 : 10, 5, 4, 0),
                    Text = $"KÃªnh {j + 1}:"
                };

                var textBox = new TextBox
                {
                    Margin = new Padding(0, 1, 0, 0),
                    Size = new Size(55, 27),
                    Text = _corrections[j].ToString("F2"),
                    Tag = j
                };
                textBox.TextChanged += Correction_TextChanged;

                correctionStrip.Controls.Add(label);
                correctionStrip.Controls.Add(textBox);
                _correctionBoxes.Add(textBox);
            }
        }

        private void Correction_TextChanged(object? sender, EventArgs e)
        {
            ReadCorrections();
            RecalculateAll(showErrors: false);
        }

        private void RebuildGridData()
        {
            _suppressGridEvents = true;
            try
            {
                RebuildCorrectionStrip();
                gridData.Columns.Clear();
                gridData.Rows.Clear();

                gridData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                gridData.SelectionMode = DataGridViewSelectionMode.CellSelect;
                gridData.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

                var measurementNoColumn = new DataGridViewTextBoxColumn
                {
                    Name = "MeasurementNo",
                    HeaderText = "Láº§n Ä‘o",
                    ReadOnly = true,
                    Width = 55,
                    FillWeight = 55,
                    Frozen = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                gridData.Columns.Add(measurementNoColumn);


                for (int j = 1; j <= _j; j++)
                {
                    var channelColumn = new DataGridViewTextBoxColumn
                    {
                        Name = $"Channel{j}",
                        HeaderText = $"KÃªnh {j} (Â°C)",
                        Width = 70,
                        FillWeight = 70,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    };
                    channelColumn.DefaultCellStyle.BackColor = Color.AliceBlue;
                    gridData.Columns.Add(channelColumn);
                }

                var ttn1Column = new DataGridViewTextBoxColumn
                {
                    Name = "Ttn1",
                    HeaderText = "Tá»§ láº§n 1 ttn1i (Â°C)",
                    Width = 75,
                    FillWeight = 75,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                ttn1Column.DefaultCellStyle.BackColor = Color.LightYellow;
                gridData.Columns.Add(ttn1Column);

                var ttn2Column = new DataGridViewTextBoxColumn
                {
                    Name = "Ttn2",
                    HeaderText = "Tá»§ láº§n 2 ttn2i (Â°C)",
                    Width = 75,
                    FillWeight = 75,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                ttn2Column.DefaultCellStyle.BackColor = Color.LightYellow;
                gridData.Columns.Add(ttn2Column);

                var ttnMeanColumn = new DataGridViewTextBoxColumn
                {
                    Name = "TtnMean",
                    HeaderText = "tÌ„tn,i (Â°C)",
                    ReadOnly = true,
                    Width = 68,
                    FillWeight = 68,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                ttnMeanColumn.DefaultCellStyle.BackColor = SystemColors.Control;
                gridData.Columns.Add(ttnMeanColumn);

                for (int i = 0; i < _n; i++)
                {
                    int rowIndex = gridData.Rows.Add();
                    var row = gridData.Rows[rowIndex];
                    row.Cells[0].Value = $"Láº§n {i + 1}";
                    for (int col = 1; col < gridData.Columns.Count; col++)
                        row.Cells[col].Value = "0.00";
                    row.Cells[TtnMeanColumn].ReadOnly = true;
                    row.Cells[TtnMeanColumn].Style.BackColor = SystemColors.Control;
                }

                AddSummaryRow("tÌ„â±¼ (hiá»‡u chÃ­nh)", Color.LightBlue);
                AddSummaryRow("Sâ±¼ (Ä‘á»™ lá»‡ch chuáº©n)", SystemColors.Control);
                AddSummaryRow("uch1,j (loáº¡i A)", SystemColors.Control);
                FitGridDataLayout();
                ResetResultLabels();
            }
            finally
            {
                _suppressGridEvents = false;
            }
        }

        private void AddSummaryRow(string label, Color backColor)
        {
            int rowIndex = gridData.Rows.Add();
            var row = gridData.Rows[rowIndex];
            row.ReadOnly = true;
            row.DefaultCellStyle.BackColor = backColor;
            row.Cells[0].Value = label;
            for (int col = 1; col < gridData.Columns.Count; col++)
                row.Cells[col].Value = "â€”";
        }

        private void FitGridDataLayout()
        {
            FitGridDataColumns();
            FitGridDataRows();
        }

        private void FitGridDataColumns()
        {
            if (gridData.Columns.Count == 0) return;

            int availableWidth = gridData.ClientSize.Width - 2;
            if (gridData.Rows.Count > 0)
                availableWidth -= SystemInformation.VerticalScrollBarWidth;
            if (availableWidth <= 0) return;

            int totalWeight = 55 + (_j * 70) + 75 + 75 + 68;
            int usedWidth = 0;

            int WidthFromWeight(int weight)
                => Math.Max(45, (int)Math.Floor(availableWidth * (weight / (double)totalWeight)));

            gridData.Columns[0].Width = WidthFromWeight(55);
            usedWidth += gridData.Columns[0].Width;

            gridData.Columns[ChannelColumn(0)].Width = WidthFromWeight(70);
            usedWidth += gridData.Columns[ChannelColumn(0)].Width;

            for (int j = 1; j < _j; j++)
            {
                gridData.Columns[ChannelColumn(j)].Width = WidthFromWeight(70);
                usedWidth += gridData.Columns[ChannelColumn(j)].Width;
            }

            gridData.Columns[Ttn1Column].Width = WidthFromWeight(75);
            usedWidth += gridData.Columns[Ttn1Column].Width;

            gridData.Columns[Ttn2Column].Width = WidthFromWeight(75);
            usedWidth += gridData.Columns[Ttn2Column].Width;

            gridData.Columns[TtnMeanColumn].Width = Math.Max(45, availableWidth - usedWidth);
        }

        private void FitGridDataRows()
        {
            if (gridData.Rows.Count == 0) return;

            int availableHeight = gridData.ClientSize.Height - gridData.ColumnHeadersHeight - 2;
            if (availableHeight <= 0) return;

            int rowHeight = Math.Max(24, availableHeight / gridData.Rows.Count);
            foreach (DataGridViewRow row in gridData.Rows)
                row.Height = rowHeight;
        }

        private void GridData_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (gridData.IsCurrentCellDirty)
                gridData.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void GridData_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _n) return;
            RecalculateAll(showErrors: false);
        }

        private void GridData_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (_suppressGridEvents || e.RowIndex < 0 || e.ColumnIndex < 1 || e.RowIndex >= _n)
                return;

            RecalculateAll(showErrors: false);
        }

        private bool RecalculateAll(bool showErrors = true)
        {
            try
            {
                var input = ReadInputFromGrid();
                var (isValid, error) = input.Validate();
                if (!isValid)
                {
                    InvalidateLastCalculation();
                    if (showErrors)
                        MessageBox.Show($"Dá»¯ liá»‡u khÃ´ng há»£p lá»‡: {error}", "Lá»—i",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                var result = UncertaintyService.Calculate(input);
                UpdateUIFromResult(result);
                _lastInput = input;
                _lastFullResult = result;
                _lastCalculatedResult = MapToCalibrationRow(result, input);
                btnCalculateAndAdd.Enabled = true;
                return true;
            }
            catch (Exception ex)
            {
                InvalidateLastCalculation();
                if (showErrors)
                    MessageBox.Show($"Lá»—i tÃ­nh toÃ¡n: {ex.Message}", "Lá»—i",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void InvalidateLastCalculation()
        {
            _lastCalculatedResult = null;
            _lastFullResult = null;
            _lastInput = null;
            btnCalculateAndAdd.Enabled = false;
        }

        private UncertaintyInput ReadInputFromGrid()
        {
            ReadCorrections();

            double[,] measurementData = new double[_n, _j];
            double[] ttn1 = new double[_n];
            double[] ttn2 = new double[_n];

            for (int i = 0; i < _n; i++)
            {
                ttn1[i] = ReadGridDouble(i, Ttn1Column);
                for (int j = 0; j < _j; j++)
                    measurementData[i, j] = ReadGridDouble(i, ChannelColumn(j));
                ttn2[i] = ReadGridDouble(i, Ttn2Column);
            }

            double.TryParse(txtUMax.Text, out double uMax);
            double.TryParse(txtDeltaMax.Text, out double deltaMax);
            double.TryParse(txtResA.Text, out double resolutionA);
            double.TryParse(txtResD.Text, out double resolutionD);
            double.TryParse(txtGiaTriDat.Text, out double giaTriDat);

            return new UncertaintyInput
            {
                J = _j,
                N = _n,
                MeasurementData = measurementData,
                Corrections = _corrections.ToArray(),
                UValues = Enumerable.Repeat(uMax, _j).ToArray(),
                DeltaValues = Enumerable.Repeat(deltaMax, _j).ToArray(),
                Ttn1 = ttn1,
                Ttn2 = ttn2,
                ResolutionA = resolutionA,
                ResolutionD = resolutionD,
                UseUMethod = rbUseU.Checked,
                GiaTriDat = giaTriDat
            };
        }

        private double ReadGridDouble(int row, int column)
        {
            double.TryParse(gridData.Rows[row].Cells[column].Value?.ToString(), out double value);
            return value;
        }

        private void ReadCorrections()
        {
            for (int j = 0; j < _j && j < _correctionBoxes.Count; j++)
                double.TryParse(_correctionBoxes[j].Text, out _corrections[j]);
        }

        private void UpdateUIFromResult(UncertaintyFullResult r)
        {
            _suppressGridEvents = true;
            try
            {
                for (int i = 0; i < _n; i++)
                {
                    double ttn1 = ReadGridDouble(i, Ttn1Column);
                    double ttn2 = ReadGridDouble(i, Ttn2Column);
                    gridData.Rows[i].Cells[TtnMeanColumn].Value = ((ttn1 + ttn2) / 2.0).ToString("F4");
                }

                int correctedMeanRow = _n;
                int stdDevRow = _n + 1;
                int typeARow = _n + 2;

                gridData.Rows[correctedMeanRow].Cells[Ttn1Column].Value = "â€”";
                gridData.Rows[correctedMeanRow].Cells[Ttn2Column].Value = "â€”";
                gridData.Rows[correctedMeanRow].Cells[TtnMeanColumn].Value = r.Ttn.ToString("F4");
                gridData.Rows[stdDevRow].Cells[Ttn1Column].Value = "â€”";
                gridData.Rows[stdDevRow].Cells[Ttn2Column].Value = "â€”";
                gridData.Rows[stdDevRow].Cells[TtnMeanColumn].Value = "â€”";
                gridData.Rows[typeARow].Cells[Ttn1Column].Value = "â€”";
                gridData.Rows[typeARow].Cells[Ttn2Column].Value = "â€”";
                gridData.Rows[typeARow].Cells[TtnMeanColumn].Value = "â€”";

                for (int j = 0; j < _j; j++)
                {
                    gridData.Rows[correctedMeanRow].Cells[ChannelColumn(j)].Value = r.ChannelCorrectedMeans[j].ToString("F4");
                    gridData.Rows[stdDevRow].Cells[ChannelColumn(j)].Value = r.ChannelStdDevs[j].ToString("F4");
                    gridData.Rows[typeARow].Cells[ChannelColumn(j)].Value = r.ChannelTypeAUncertainties[j].ToString("F4");
                }

                lblR_Dt.Text = $"Î”t = {r.DeltaT:F4} Â°C";
                lblR_Od.Text = $"Â±{r.DeltaOd:F3} Â°C";
                lblR_Dd.Text = $"Â±{r.DeltaDd:F3} Â°C";
                lblR_Uch1.Text = $"{r.Uch1:F4} Â°C";
                lblR_Uch2.Text = $"{r.Uch2:F4} Â°C";
                lblR_Uch.Text = $"{r.Uc:F4} Â°C";
                lblR_Ubk.Text = $"{r.Ubk:F4} Â°C";
                lblR_U.Text = $"Â±{r.UFinal:F3} Â°C";
            }
            finally
            {
                _suppressGridEvents = false;
            }
        }

        private void ResetResultLabels()
        {
            if (lblR_Dt == null) return;
            lblR_Dt.Text = "â€”";
            lblR_Od.Text = "â€”";
            lblR_Dd.Text = "â€”";
            lblR_Uch1.Text = "â€”";
            lblR_Uch2.Text = "â€”";
            lblR_Uch.Text = "â€”";
            lblR_Ubk.Text = "â€”";
            lblR_U.Text = "â€”";
        }

        private void BuildResultCards()
        {
            ResetResultLabels();
        }

        private CalibrationResultRow MapToCalibrationRow(UncertaintyFullResult r, UncertaintyInput input)
        {
            var row = new CalibrationResultRow
            {
                GiaTriDat = input.GiaTriDat,
                GiaTriChiThi = r.Ttn,
                GiaTriTrungBinh = r.Tch,
                SoHieuChinh = r.DeltaT,
                DoOnDinh = r.DeltaOd,
                DoDongDeu = r.DeltaDd,
                DoKhongDamBao = r.UFinal,
                Uch = r.Uc,
                Ubk = r.Ubk,
                SoKenh = input.J,
                SoLanDo = input.N,
                PhuongPhapB = input.UseUMethod ? "U" : "Delta",
            };

            for (int j = 0; j < input.J && j < row.Kenh.Length; j++)
                row.Kenh[j] = r.ChannelCorrectedMeans[j];

            row.ChiTietLanDos = ExtractChiTietLanDo();
            return row;
        }

        private List<ChiTietLanDo> ExtractChiTietLanDo()
        {
            var list = new List<ChiTietLanDo>();

            for (int i = 0; i < _n; i++)
            {
                double ttn1 = ReadGridDouble(i, Ttn1Column);
                double ttn2 = ReadGridDouble(i, Ttn2Column);
                double chiThi = (ttn1 + ttn2) / 2.0;

                for (int j = 0; j < _j; j++)
                {
                    if (!double.TryParse(gridData.Rows[i].Cells[ChannelColumn(j)].Value?.ToString(), out double val))
                        continue;

                    list.Add(new ChiTietLanDo
                    {
                        LanDo = i + 1,
                        Kenh = j + 1,
                        GiaTri = val,
                        ChiThiUut = double.IsNaN(chiThi) ? null : chiThi,
                    });
                }
            }

            return list;
        }

        private async void BtnCalculateAndAdd_Click(object? sender, EventArgs e)
        {
            if (!RecalculateAll(showErrors: true) || _lastFullResult == null || _lastInput == null || _lastCalculatedResult == null)
                return;

            if (_onResultAdded == null)
            {
                MessageBox.Show("Form khÃ´ng Ä‘Æ°á»£c má»Ÿ vá»›i callback. Vui lÃ²ng má»Ÿ láº¡i tá»« mÃ n hÃ¬nh chÃ­nh.", "ThÃ´ng bÃ¡o",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await _onResultAdded(_lastCalculatedResult);

            if (_editMode)
            {
                lblStatus.Text = "\u0110\u00e3 l\u01b0u ch\u1ec9nh s\u1eeda \u0111i\u1ec3m \u0111o.";
                return;
            }

            int stt = _gridResults.Rows.Count + 1;
            AppendResultRow(_lastFullResult, _lastInput, stt);

            AppendBudgetPoint(_lastFullResult, stt);

            lblStatus.Text = $"ÄÃ£ thÃªm Ä‘iá»ƒm {txtGiaTriDat.Text} Â°C  " +
                             $"(tá»•ng {stt} Ä‘iá»ƒm)";

            ResetMeasurementCells();
        }

        private void ResetMeasurementCells()
        {
            _suppressGridEvents = true;
            try
            {
                for (int i = 0; i < _n; i++)
                {
                    for (int col = 1; col < gridData.Columns.Count; col++)
                        gridData.Rows[i].Cells[col].Value = "0.00";
                }

                for (int row = _n; row < _n + 3; row++)
                    for (int col = 1; col < gridData.Columns.Count; col++)
                        gridData.Rows[row].Cells[col].Value = "â€”";

                _lastCalculatedResult = null;
                _lastFullResult = null;
                _lastInput = null;
                btnCalculateAndAdd.Enabled = false;
                ResetResultLabels();
            }
            finally
            {
                _suppressGridEvents = false;
            }
        }

        private void BtnImportCSV_Click(object? sender, EventArgs e)
        {
            using var openDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Import dá»¯ liá»‡u tá»« CSV"
            };
            if (openDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                ImportFromCSV(openDialog.FileName);
                MessageBox.Show("Import thÃ nh cÃ´ng!", "ThÃ´ng bÃ¡o",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lá»—i import: {ex.Message}", "Lá»—i",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportFromCSV(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            if (lines.Length < 2) return;

            var config = lines[0].Split(',');
            if (config.Length >= 2)
                SetConfiguration(int.Parse(config[0]), int.Parse(config[1]), notifyOwner: true);

            for (int i = 1; i < lines.Length && i <= _n; i++)
            {
                var values = lines[i].Split(',');
                for (int j = 0; j < values.Length && j < _j; j++)
                    gridData.Rows[i - 1].Cells[ChannelColumn(j)].Value = values[j];
            }

            RecalculateAll();
        }

        private void BtnExportCSV_Click(object? sender, EventArgs e)
        {
            using var saveDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                DefaultExt = "csv",
                FileName = $"UncertaintyData_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };
            if (saveDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                ExportToCSV(saveDialog.FileName);
                MessageBox.Show("Export thÃ nh cÃ´ng!", "ThÃ´ng bÃ¡o",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lá»—i export: {ex.Message}", "Lá»—i",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string filePath)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{_j},{_n}");
            sb.Append("Láº§n Ä‘o,t_tn1");
            for (int j = 1; j <= _j; j++) sb.Append($",KÃªnh {j}");
            sb.AppendLine(",t_tn2,t_tn_mean");

            for (int i = 0; i < _n; i++)
            {
                sb.Append($"Láº§n {i + 1}");
                for (int col = 1; col < gridData.Columns.Count; col++)
                    sb.Append($",{gridData.Rows[i].Cells[col].Value}");
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("Káº¿t quáº£ tÃ­nh toÃ¡n:");
            sb.AppendLine($"DeltaT,{lblR_Dt.Text}");
            sb.AppendLine($"Uch1,{lblR_Uch1.Text}");
            sb.AppendLine($"Uch2,{lblR_Uch2.Text}");
            sb.AppendLine($"Uch,{lblR_Uch.Text}");
            sb.AppendLine($"Ubk,{lblR_Ubk.Text}");
            sb.AppendLine($"U,{lblR_U.Text}");

            File.WriteAllText(filePath, sb.ToString());
        }

        private void InitResultsGrid()
        {
            _gridResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9F),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                }
            };

            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResSTT", HeaderText = "STT", FillWeight = 35, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResGTDat", HeaderText = "GiÃ¡ trá»‹ Ä‘áº·t\n(Â°C)", FillWeight = 70, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResChiThi", HeaderText = "Chá»‰ thá»‹ tá»§\n(Â°C)", FillWeight = 70, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResTch", HeaderText = "tÌ„_ch (Â°C)", FillWeight = 75, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResDeltaT", HeaderText = "Î”t (Â°C)", FillWeight = 65, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResDeltaOd", HeaderText = "Î´t_od (Â°C)", FillWeight = 70, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResDeltaDd", HeaderText = "Î´t_dd (Â°C)", FillWeight = 70, SortMode = DataGridViewColumnSortMode.NotSortable });

            var colU = new DataGridViewTextBoxColumn
            {
                Name = "ResU",
                HeaderText = "U (k=2, P=95%)\n(Â°C)",
                FillWeight = 90,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            colU.DefaultCellStyle.ForeColor = Color.DarkRed;
            colU.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _gridResults.Columns.Add(colU);

            _tabResults.Controls.Add(_gridResults);
        }

        private void InitBudgetGrid()
        {
            _gridBudget = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9F),
                BackgroundColor = Color.White,
            };

            _gridBudget.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudSym", HeaderText = "KÃ½ hiá»‡u", FillWeight = 70, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridBudget.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudSource", HeaderText = "Nguá»“n khÃ´ng Ä‘áº£m báº£o", FillWeight = 200, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridBudget.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudVal", HeaderText = "GiÃ¡ trá»‹", FillWeight = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridBudget.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudUnit", HeaderText = "ÄÆ¡n vá»‹", FillWeight = 50, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridBudget.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudDiv", HeaderText = "Há»‡ sá»‘ chia", FillWeight = 65, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridBudget.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudCi", HeaderText = "Ci", FillWeight = 50, SortMode = DataGridViewColumnSortMode.NotSortable });

            var colUi = new DataGridViewTextBoxColumn
            {
                Name = "BudUi",
                HeaderText = "u_i (Â°C)",
                FillWeight = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            colUi.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _gridBudget.Columns.Add(colUi);

            _tabBudget.Controls.Add(_gridBudget);
        }

        private void InitBudgetTable()
        {
            _gridBudget.Columns.Clear();
        }

        private void AppendBudgetPoint(UncertaintyFullResult r, int pointIndex)
        {
            int n = _lastInput!.N;
            bool useU = _lastInput.UseUMethod;
            double uMax = _lastInput.UValues[0];
            double delta = _lastInput.DeltaValues[0];
            double A = _lastInput.ResolutionA;
            double d = _lastInput.ResolutionD;

            // â”€â”€ DÃ²ng phÃ¢n cÃ¡ch náº¿u Ä‘Ã£ cÃ³ Ä‘iá»ƒm trÆ°á»›c â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            if (_gridBudget.Rows.Count > 0)
                AddBudgetSeparatorRow($"Äiá»ƒm {pointIndex}");

            // â”€â”€ u1: Táº£n mÃ¡t KQ Ä‘o cá»§a chuáº©n â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            AddBudgetRow($"u1-{pointIndex}",
                         "Táº£n mÃ¡t KQ Ä‘o cá»§a chuáº©n",
                         r.Uch1, "Â°C", "1", 1.0, 1.0, r.Uch1);

            // â”€â”€ u2: Táº£n mÃ¡t KQ Ä‘o cá»§a UUT â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            AddBudgetRow($"u2-{pointIndex}",
                         "Táº£n mÃ¡t KQ Ä‘o cá»§a UUT",
                         r.Ubk1, "Â°C", "1", 1.0, 1.0, r.Ubk1);

            // â”€â”€ u3: ÄKÄBÄ cá»§a chuáº©n â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            AddBudgetRow("u3",
                         "ÄKÄBÄ cá»§a chuáº©n",
                         useU ? uMax : delta, "Â°C",
                         useU ? "2" : "âˆš3",
                         useU ? 2.0 : Math.Sqrt(3),
                         1.0, r.Uch2);

            // â”€â”€ u4: Äá»™ phÃ¢n giáº£i cá»§a UUT â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            AddBudgetRow("u4",
                         "Äá»™ phÃ¢n giáº£i cá»§a UUT",
                         A * d, "Â°C", "âˆš3", Math.Sqrt(3), 1.0, r.Ubk4);

            // â”€â”€ u5: Äá»™ á»•n Ä‘á»‹nh â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            AddBudgetRow($"u5-{pointIndex}",
                         "Äá»™ á»•n Ä‘á»‹nh",
                         r.DeltaOd, "Â°C", "âˆš3", Math.Sqrt(3), 1.0, r.Ubk2);

            // â”€â”€ u6: Äá»™ Ä‘á»“ng Ä‘á»u â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            AddBudgetRow($"u6-{pointIndex}",
                         "Äá»™ Ä‘á»“ng Ä‘á»u",
                         r.DeltaDd, "Â°C", "âˆš3", Math.Sqrt(3), 1.0, r.Ubk3);

            // â”€â”€ Tá»•ng há»£p Ä‘iá»ƒm nÃ y â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            AddSummaryBudgetRow($"u_ch-{pointIndex}",
                                $"LiÃªn há»£p chuáº©n â€” Ä‘iá»ƒm {pointIndex}",
                                r.Uc, Color.FromArgb(220, 235, 255));

            AddSummaryBudgetRow($"u_bk-{pointIndex}",
                                $"LiÃªn há»£p tá»§ nhiá»‡t â€” Ä‘iá»ƒm {pointIndex}",
                                r.Ubk, Color.FromArgb(220, 235, 255));

            AddSummaryBudgetRow($"U-{pointIndex}",
                                $"ÄKÄB má»Ÿ rá»™ng â€” Ä‘iá»ƒm {pointIndex}, k=2, P=95%",
                                r.UFinal,
                                Color.FromArgb(200, 255, 200),
                                prefix: "Â±", bold: true,
                                foreColor: Color.DarkGreen);

            // Scroll xuá»‘ng dÃ²ng má»›i nháº¥t
            _gridBudget.FirstDisplayedScrollingRowIndex =
                _gridBudget.Rows.Count - 1;
        }

        private void AddBudgetSeparatorRow(string text)
        {
            int idx = _gridBudget.Rows.Add(text, "", "", "", "", "", "");
            var style = _gridBudget.Rows[idx].DefaultCellStyle;
            style.BackColor = Color.FromArgb(230, 230, 230);
            style.ForeColor = Color.FromArgb(80, 80, 80);
            style.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            _gridBudget.Rows[idx].ReadOnly = true;
        }

        private void AppendResultRow(UncertaintyFullResult r, UncertaintyInput input, int stt)
        {
            int idx = _gridResults.Rows.Add(
                stt,
                input.GiaTriDat.ToString("F1"),
                r.Ttn.ToString("F4"),
                r.Tch.ToString("F4"),
                r.DeltaT.ToString("F4"),
                r.DeltaOd.ToString("F4"),
                r.DeltaDd.ToString("F4"),
                $"Â±{r.UFinal:F4}");

            var row = _gridResults.Rows[idx];
            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 200);
            _gridResults.FirstDisplayedScrollingRowIndex = idx;

            Task.Delay(2000).ContinueWith(_ =>
            {
                if (IsDisposed || !IsHandleCreated) return;
                Invoke(() =>
                {
                    if (idx < _gridResults.Rows.Count)
                        _gridResults.Rows[idx].DefaultCellStyle.BackColor = Color.Empty;
                });
            });
        }

        private void BuildBudgetTable(UncertaintyFullResult r)
        {
            _gridBudget.Rows.Clear();
            RebuildBudgetColumns();

            // Index Ä‘iá»ƒm hiá»‡u chuáº©n hiá»‡n táº¡i (Ä‘áº¿m tá»« káº¿t quáº£ Ä‘Ã£ thÃªm)
            int pointIndex = _gridResults.Rows.Count; // Ä‘iá»ƒm thá»© máº¥y (1-based)

            int n = _lastInput!.N;
            int j = _lastInput!.J;

            // â”€â”€ u1-{pointIndex}: Táº£n mÃ¡t KQ Ä‘o cá»§a chuáº©n â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            // = u_ch1 tá»•ng há»£p táº¡i Ä‘iá»ƒm nÃ y (CT7) â€” má»™t dÃ²ng duy nháº¥t
            AddBudgetRow(
                sym: $"u1-{pointIndex}",
                source: "Táº£n mÃ¡t KQ Ä‘o cá»§a chuáº©n",
                rawValue: r.Uch1,   // Ä‘Ã£ lÃ  âˆšÎ£(uch1,jÂ²) â€” CT7
                unit: "Â°C",
                divisorText: "1",
                divisorValue: 1.0,
                ci: 1.0,
                ui: r.Uch1);

            // â”€â”€ u2-{pointIndex}: Táº£n mÃ¡t KQ Ä‘o cá»§a UUT â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            AddBudgetRow(
                sym: $"u2-{pointIndex}",
                source: "Táº£n mÃ¡t KQ Ä‘o cá»§a UUT",
                rawValue: r.Ubk1,   // CT13
                unit: "Â°C",
                divisorText: "1",
                divisorValue: 1.0,
                ci: 1.0,
                ui: r.Ubk1);

            // â”€â”€ u3: ÄKÄBÄ cá»§a chuáº©n â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            bool useU = _lastInput.UseUMethod;
            double uMax = _lastInput.UValues[0];
            double delta = _lastInput.DeltaValues[0];
            AddBudgetRow(
                sym: "u3",
                source: "ÄKÄBÄ cá»§a chuáº©n",
                rawValue: useU ? uMax : delta,
                unit: "Â°C",
                divisorText: useU ? "2" : "âˆš3",
                divisorValue: useU ? 2.0 : Math.Sqrt(3),
                ci: 1.0,
                ui: r.Uch2);

            // â”€â”€ u4: Äá»™ phÃ¢n giáº£i cá»§a UUT â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            double A = _lastInput.ResolutionA;
            double d = _lastInput.ResolutionD;
            AddBudgetRow(
                sym: "u4",
                source: "Äá»™ phÃ¢n giáº£i cá»§a UUT",
                rawValue: A * d,
                unit: "Â°C",
                divisorText: "âˆš3",
                divisorValue: Math.Sqrt(3),
                ci: 1.0,
                ui: r.Ubk4);

            // â”€â”€ u5-{pointIndex}: Äá»™ á»•n Ä‘á»‹nh â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            // Î´t_od táº¡i Ä‘iá»ƒm nÃ y = max qua k kÃªnh cá»§a Â½(max-min)
            AddBudgetRow(
                sym: $"u5-{pointIndex}",
                source: "Äá»™ á»•n Ä‘á»‹nh",
                rawValue: r.DeltaOd,   // CT5
                unit: "Â°C",
                divisorText: "âˆš3",
                divisorValue: Math.Sqrt(3),
                ci: 1.0,
                ui: r.Ubk2);    // CT15

            // â”€â”€ u6-{pointIndex}: Äá»™ Ä‘á»“ng Ä‘á»u â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            AddBudgetRow(
                sym: $"u6-{pointIndex}",
                source: "Äá»™ Ä‘á»“ng Ä‘á»u",
                rawValue: r.DeltaDd,   // CT6
                unit: "Â°C",
                divisorText: "âˆš3",
                divisorValue: Math.Sqrt(3),
                ci: 1.0,
                ui: r.Ubk3);    // CT16

            // â”€â”€ Separator + Tá»•ng há»£p â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            AddSummaryBudgetRow("u_ch",
                $"Äá»™ KÄB chuáº©n liÃªn há»£p â€” Ä‘iá»ƒm {pointIndex} (CT12)",
                r.Uc,
                Color.FromArgb(220, 235, 255));

            AddSummaryBudgetRow("u_bk",
                $"Äá»™ KÄB tá»§ nhiá»‡t liÃªn há»£p â€” Ä‘iá»ƒm {pointIndex} (CT18)",
                r.Ubk,
                Color.FromArgb(220, 235, 255));

            AddSummaryBudgetRow("U",
                $"Äá»™ KÄB má»Ÿ rá»™ng â€” Ä‘iá»ƒm {pointIndex}, k=2, P=95% (CT19)",
                r.UFinal,
                Color.FromArgb(200, 255, 200),
                prefix: "Â±",
                bold: true,
                foreColor: Color.DarkGreen);
        }

        // â”€â”€ Helpers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        private void RebuildBudgetColumns()
        {
            _gridBudget.Columns.Clear();

            void Add(string name, string header, int weight) =>
                _gridBudget.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = name,
                    HeaderText = header,
                    FillWeight = weight,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

            Add("BudSym", "KÃ½ hiá»‡u", 55);
            Add("BudSource", "Nguá»“n gÃ¢y ra ÄKÄBÄ", 200);
            Add("BudVal", "GiÃ¡ trá»‹", 70);
            Add("BudUnit", "ÄÆ¡n vá»‹", 45);
            Add("BudDiv", "Há»‡ sá»‘ chia", 70);
            Add("BudCi", "Ci", 40);
            Add("BudUi", "ÄKÄB chuáº©n (ui)", 80);

            _gridBudget.Columns["BudVal"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            _gridBudget.Columns["BudUi"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            _gridBudget.Columns["BudUi"].DefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);
        }

        private void AddBudgetRow(
            string sym, string source,
            double rawValue, string unit,
            string divisorText, double divisorValue,
            double ci, double ui,
            Color? backColor = null)
        {
            double safeUi = divisorValue != 0 ? rawValue * ci / divisorValue : 0;
            // ui truyá»n vÃ o Ä‘Æ°á»£c Æ°u tiÃªn náº¿u Ä‘Ã£ tÃ­nh sáºµn, fallback tÃ­nh láº¡i
            double displayUi = double.IsNaN(ui) ? safeUi : ui;

            int idx = _gridBudget.Rows.Add(
                sym,
                source,
                double.IsNaN(rawValue) || rawValue == 0 ? "â€”" : rawValue.ToString("F4"),
                unit,
                divisorText,
                ci.ToString("F1"),
                displayUi == 0 ? "â€”" : displayUi.ToString("F4"));

            if (backColor.HasValue)
                _gridBudget.Rows[idx].DefaultCellStyle.BackColor = backColor.Value;
        }

        private void AddSummaryBudgetRow(
            string sym, string source, double value,
            Color backColor,
            string prefix = "",
            bool bold = false,
            Color? foreColor = null)
        {
            int idx = _gridBudget.Rows.Add(
                sym, source,
                "â€”", "Â°C", "â€”", "â€”",
                $"{prefix}{value:F4}");

            var style = _gridBudget.Rows[idx].DefaultCellStyle;
            style.BackColor = backColor;
            style.Font = new Font(_gridBudget.Font,
                                  bold ? FontStyle.Bold : FontStyle.Regular);
            if (foreColor.HasValue)
                style.ForeColor = foreColor.Value;
        }

        /// <summary>
        /// Load dá»¯ liá»‡u tá»« má»™t Ä‘iá»ƒm Ä‘o Ä‘Ã£ tÃ­nh Ä‘á»ƒ cho phÃ©p chá»‰nh sá»­a láº¡i.
        /// </summary>
        public void LoadExistingData(CalibrationResultRow row)
        {
            // Cáº­p nháº­t config trÆ°á»›c
            SetConfiguration(
                row.SoKenh > 0 ? row.SoKenh : _j,
                row.SoLanDo > 0 ? row.SoLanDo : _n,
                notifyOwner: false);

            // Äiá»n giÃ¡ trá»‹ Ä‘áº·t
            txtGiaTriDat.Text = row.GiaTriDat.ToString("F1");

            // Náº¿u khÃ´ng cÃ³ chi tiáº¿t tá»«ng láº§n Ä‘o, dÃ¹ng giÃ¡ trá»‹ trung bÃ¬nh kÃªnh
            if (row.ChiTietLanDos == null || row.ChiTietLanDos.Count == 0)
                return;

            _suppressGridEvents = true;
            try
            {
                for (int i = 0; i < _n; i++)
                {
                    int lanDo = i + 1;

                    // Äiá»n giÃ¡ trá»‹ tá»«ng kÃªnh
                    for (int j = 0; j < _j; j++)
                    {
                        var detail = row.ChiTietLanDos
                            .FirstOrDefault(d => d.LanDo == lanDo && d.Kenh == j + 1);

                        if (detail != null)
                            gridData.Rows[i].Cells[ChannelColumn(j)].Value =
                                detail.GiaTri.ToString("F2");
                    }

                    // Äiá»n chá»‰ thá»‹ tá»§ â€” ttn1 vÃ  ttn2 Ä‘á»u dÃ¹ng chi_thi_uut
                    // (giÃ¡ trá»‹ lÆ°u lÃ  (ttn1+ttn2)/2; khi load láº¡i,
                    //  Ä‘iá»n vÃ o cáº£ hai Ã´ Ä‘á»ƒ tÃ­nh láº¡i cho Ä‘Ãºng)
                    var anyDetail = row.ChiTietLanDos
                        .FirstOrDefault(d => d.LanDo == lanDo && d.ChiThiUut.HasValue);

                    if (anyDetail?.ChiThiUut is double chiThi)
                    {
                        gridData.Rows[i].Cells[Ttn1Column].Value = chiThi.ToString("F2");
                        gridData.Rows[i].Cells[Ttn2Column].Value = chiThi.ToString("F2");
                    }
                }
            }
            finally
            {
                _suppressGridEvents = false;
            }

            // TÃ­nh láº¡i ngay Ä‘á»ƒ hiá»ƒn thá»‹ káº¿t quáº£
            RecalculateAll(showErrors: false);
            _editMode = true;
            btnCalculateAndAdd.Text = "TÃ­nh láº¡i vÃ  LÆ°u chá»‰nh sá»­a";
        }
    }
}
