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

        private DataGridView _gridResults = null!;
        private DataGridView _gridBudget = null!;

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
                    Text = $"Kênh {j + 1}:"
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
                    HeaderText = "Lần đo",
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
                        HeaderText = $"Kênh {j} (°C)",
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
                    HeaderText = "Tủ lần 1 ttn1i (°C)",
                    Width = 75,
                    FillWeight = 75,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                ttn1Column.DefaultCellStyle.BackColor = Color.LightYellow;
                gridData.Columns.Add(ttn1Column);

                var ttn2Column = new DataGridViewTextBoxColumn
                {
                    Name = "Ttn2",
                    HeaderText = "Tủ lần 2 ttn2i (°C)",
                    Width = 75,
                    FillWeight = 75,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                ttn2Column.DefaultCellStyle.BackColor = Color.LightYellow;
                gridData.Columns.Add(ttn2Column);

                var ttnMeanColumn = new DataGridViewTextBoxColumn
                {
                    Name = "TtnMean",
                    HeaderText = "t̄tn,i (°C)",
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
                    row.Cells[0].Value = $"Lần {i + 1}";
                    for (int col = 1; col < gridData.Columns.Count; col++)
                        row.Cells[col].Value = "0.00";
                    row.Cells[_j + 3].ReadOnly = true;
                    row.Cells[_j + 3].Style.BackColor = SystemColors.Control;
                }

                AddSummaryRow("t̄ⱼ (hiệu chính)", Color.LightBlue);
                AddSummaryRow("Sⱼ (độ lệch chuẩn)", SystemColors.Control);
                AddSummaryRow("uch1,j (loại A)", SystemColors.Control);
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
                row.Cells[col].Value = "—";
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

            int totalWeight = 55 + 75 + (_j * 70) + 75 + 68;
            int usedWidth = 0;

            int WidthFromWeight(int weight)
                => Math.Max(45, (int)Math.Floor(availableWidth * (weight / (double)totalWeight)));

            gridData.Columns[0].Width = WidthFromWeight(55);
            usedWidth += gridData.Columns[0].Width;

            gridData.Columns[1].Width = WidthFromWeight(75);
            usedWidth += gridData.Columns[1].Width;

            for (int j = 0; j < _j; j++)
            {
                gridData.Columns[j + 2].Width = WidthFromWeight(70);
                usedWidth += gridData.Columns[j + 2].Width;
            }

            gridData.Columns[_j + 2].Width = WidthFromWeight(75);
            usedWidth += gridData.Columns[_j + 2].Width;

            gridData.Columns[_j + 3].Width = Math.Max(45, availableWidth - usedWidth);
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
                        MessageBox.Show($"Dữ liệu không hợp lệ: {error}", "Lỗi",
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
                    MessageBox.Show($"Lỗi tính toán: {ex.Message}", "Lỗi",
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
                ttn1[i] = ReadGridDouble(i, 1);
                for (int j = 0; j < _j; j++)
                    measurementData[i, j] = ReadGridDouble(i, j + 2);
                ttn2[i] = ReadGridDouble(i, _j + 2);
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
                    double ttn1 = ReadGridDouble(i, 1);
                    double ttn2 = ReadGridDouble(i, _j + 2);
                    gridData.Rows[i].Cells[_j + 3].Value = ((ttn1 + ttn2) / 2.0).ToString("F4");
                }

                int correctedMeanRow = _n;
                int stdDevRow = _n + 1;
                int typeARow = _n + 2;

                gridData.Rows[correctedMeanRow].Cells[1].Value = "—";
                gridData.Rows[correctedMeanRow].Cells[_j + 2].Value = "—";
                gridData.Rows[correctedMeanRow].Cells[_j + 3].Value = r.Ttn.ToString("F4");
                gridData.Rows[stdDevRow].Cells[1].Value = "—";
                gridData.Rows[stdDevRow].Cells[_j + 2].Value = "—";
                gridData.Rows[stdDevRow].Cells[_j + 3].Value = "—";
                gridData.Rows[typeARow].Cells[1].Value = "—";
                gridData.Rows[typeARow].Cells[_j + 2].Value = "—";
                gridData.Rows[typeARow].Cells[_j + 3].Value = "—";

                for (int j = 0; j < _j; j++)
                {
                    gridData.Rows[correctedMeanRow].Cells[j + 2].Value = r.ChannelCorrectedMeans[j].ToString("F4");
                    gridData.Rows[stdDevRow].Cells[j + 2].Value = r.ChannelStdDevs[j].ToString("F4");
                    gridData.Rows[typeARow].Cells[j + 2].Value = r.ChannelTypeAUncertainties[j].ToString("F4");
                }

                lblR_Dt.Text = $"Δt = {r.DeltaT:F4} °C";
                lblR_Od.Text = $"±{r.DeltaOd:F3} °C";
                lblR_Dd.Text = $"±{r.DeltaDd:F3} °C";
                lblR_Uch1.Text = $"{r.Uch1:F4} °C";
                lblR_Uch2.Text = $"{r.Uch2:F4} °C";
                lblR_Uch.Text = $"{r.Uc:F4} °C";
                lblR_Ubk.Text = $"{r.Ubk:F4} °C";
                lblR_U.Text = $"±{r.UFinal:F3} °C";
            }
            finally
            {
                _suppressGridEvents = false;
            }
        }

        private void ResetResultLabels()
        {
            if (lblR_Dt == null) return;
            lblR_Dt.Text = "—";
            lblR_Od.Text = "—";
            lblR_Dd.Text = "—";
            lblR_Uch1.Text = "—";
            lblR_Uch2.Text = "—";
            lblR_Uch.Text = "—";
            lblR_Ubk.Text = "—";
            lblR_U.Text = "—";
        }

        private void BuildResultCards()
        {
            resultCards.Controls.Clear();

            lblR_Dt = AddResultCard(0, 0, "Số hiệu chính Δt (CT4)");
            lblR_Od = AddResultCard(1, 0, "Độ ổn định δtod (CT5)");
            lblR_Dd = AddResultCard(2, 0, "Độ đồng đều δtdd (CT6)");
            lblR_Uch1 = AddResultCard(3, 0, "uch1 — loại A (CT7)");
            lblR_Uch2 = AddResultCard(0, 1, "uch2 — loại B (CT10/11)");
            lblR_Uch = AddResultCard(1, 1, "uch — liên hợp chuẩn (CT12)");
            lblR_Ubk = AddResultCard(2, 1, "ubk — liên hợp tủ (CT18)");
            lblR_U = AddResultCard(3, 1, "U mở rộng k=2 P=95% (CT19)");

            ResetResultLabels();
        }

        private Label AddResultCard(int column, int row, string title)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(3),
                Padding = new Padding(8, 3, 8, 3),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var titleLabel = new Label
            {
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10F),
                Height = 22,
                Text = title,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var valueLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Text = "—",
                TextAlign = ContentAlignment.MiddleLeft
            };

            panel.Controls.Add(valueLabel);
            panel.Controls.Add(titleLabel);
            resultCards.Controls.Add(panel, column, row);
            return valueLabel;
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
                double ttn1 = ReadGridDouble(i, 1);
                double ttn2 = ReadGridDouble(i, _j + 2);
                double chiThi = (ttn1 + ttn2) / 2.0;

                for (int j = 0; j < _j; j++)
                {
                    if (!double.TryParse(gridData.Rows[i].Cells[j + 2].Value?.ToString(), out double val))
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
                MessageBox.Show("Form không được mở với callback. Vui lòng mở lại từ màn hình chính.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await _onResultAdded(_lastCalculatedResult);

            int stt = _gridResults.Rows.Count + 1;
            AppendResultRow(_lastFullResult, _lastInput, stt);
            BuildBudgetTable(_lastFullResult);
            lblStatus.Text = $"Đã thêm điểm {txtGiaTriDat.Text} °C";

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
                        gridData.Rows[row].Cells[col].Value = "—";

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
                Title = "Import dữ liệu từ CSV"
            };
            if (openDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                ImportFromCSV(openDialog.FileName);
                MessageBox.Show("Import thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi import: {ex.Message}", "Lỗi",
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
                    gridData.Rows[i - 1].Cells[j + 2].Value = values[j];
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
                MessageBox.Show("Export thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi export: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string filePath)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{_j},{_n}");
            sb.Append("Lần đo,t_tn1");
            for (int j = 1; j <= _j; j++) sb.Append($",Kênh {j}");
            sb.AppendLine(",t_tn2,t_tn_mean");

            for (int i = 0; i < _n; i++)
            {
                sb.Append($"Lần {i + 1}");
                for (int col = 1; col < gridData.Columns.Count; col++)
                    sb.Append($",{gridData.Rows[i].Cells[col].Value}");
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("Kết quả tính toán:");
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
            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResGTDat", HeaderText = "Giá trị đặt\n(°C)", FillWeight = 70, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResChiThi", HeaderText = "Chỉ thị tủ\n(°C)", FillWeight = 70, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResTch", HeaderText = "t̄_ch (°C)", FillWeight = 75, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResDeltaT", HeaderText = "Δt (°C)", FillWeight = 65, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResDeltaOd", HeaderText = "δt_od (°C)", FillWeight = 70, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ResDeltaDd", HeaderText = "δt_dd (°C)", FillWeight = 70, SortMode = DataGridViewColumnSortMode.NotSortable });

            var colU = new DataGridViewTextBoxColumn
            {
                Name = "ResU",
                HeaderText = "U (k=2, P=95%)\n(°C)",
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

            _gridBudget.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudSym", HeaderText = "Ký hiệu", FillWeight = 70, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridBudget.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudSource", HeaderText = "Nguồn không đảm bảo", FillWeight = 200, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridBudget.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudVal", HeaderText = "Giá trị", FillWeight = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridBudget.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudUnit", HeaderText = "Đơn vị", FillWeight = 50, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridBudget.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudDiv", HeaderText = "Hệ số chia", FillWeight = 65, SortMode = DataGridViewColumnSortMode.NotSortable });
            _gridBudget.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudCi", HeaderText = "Ci", FillWeight = 50, SortMode = DataGridViewColumnSortMode.NotSortable });

            var colUi = new DataGridViewTextBoxColumn
            {
                Name = "BudUi",
                HeaderText = "u_i (°C)",
                FillWeight = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            colUi.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _gridBudget.Columns.Add(colUi);

            _tabBudget.Controls.Add(_gridBudget);
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
                $"±{r.UFinal:F4}");

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

            void AddRow(string sym, string source, double val, double divisor, string unit = "°C", double ci = 1.0)
            {
                double ui = val / divisor;
                _gridBudget.Rows.Add(sym, source,
                    val.ToString("F4"), unit,
                    divisor == 1.0 ? "1" : divisor.ToString("F4"),
                    ci.ToString("F1"),
                    ui.ToString("F4"));
            }

            string methodB = r.MethodUsed == "U"
                ? "Thiết bị chuẩn (U/2)"
                : "Thiết bị chuẩn (∂/√3)";

            AddRow("u_ch1", "Lặp lại phép đo - Loại A", r.Uch1, 1.0);
            AddRow("u_ch2", methodB, r.Uch2, 1.0);
            AddRow("u_bk1", "Lặp lại chỉ thị tủ - Loại A", r.Ubk1, 1.0);
            AddRow("u_bk2", "Độ ổn định tủ nhiệt", r.Ubk2, 1.0);
            AddRow("u_bk3", "Độ đồng đều tủ nhiệt", r.Ubk3, 1.0);
            AddRow("u_bk4", "Độ phân giải thiết bị chỉ thị", r.Ubk4, 1.0);

            int ucIdx = _gridBudget.Rows.Add(
                "u_c", "Liên hợp chuẩn tổng hợp",
                r.Uc.ToString("F4"), "°C", "—", "—",
                r.Uc.ToString("F4"));
            _gridBudget.Rows[ucIdx].DefaultCellStyle.BackColor = Color.FromArgb(220, 235, 255);
            _gridBudget.Rows[ucIdx].DefaultCellStyle.Font = new Font(_gridBudget.Font, FontStyle.Bold);

            int uIdx = _gridBudget.Rows.Add(
                "U", "Mở rộng - k=2, P=95%",
                $"±{r.UFinal:F4}", "°C", "—", "2",
                $"±{r.UFinal:F4}");
            _gridBudget.Rows[uIdx].DefaultCellStyle.BackColor = Color.FromArgb(200, 255, 200);
            _gridBudget.Rows[uIdx].DefaultCellStyle.Font = new Font(_gridBudget.Font, FontStyle.Bold);
            _gridBudget.Rows[uIdx].DefaultCellStyle.ForeColor = Color.DarkGreen;
        }
    }
}
