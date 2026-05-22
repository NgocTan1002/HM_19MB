using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        private bool _mathResultLabelsInitialized;

        private sealed class StandardTraceabilityData
        {
            public double[] Corrections { get; set; } = Array.Empty<double>();
            public double[] UValues { get; set; } = Array.Empty<double>();
            public double[] DeltaValues { get; set; } = Array.Empty<double>();
        }

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
                    HeaderText = "Chỉ thị tủ lần 1(°C)",
                    Width = 75,
                    FillWeight = 75,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                ttn1Column.DefaultCellStyle.BackColor = Color.LightYellow;
                gridData.Columns.Add(ttn1Column);

                var ttn2Column = new DataGridViewTextBoxColumn
                {
                    Name = "Ttn2",
                    HeaderText = "Chỉ thị tủ lần 2(°C)",
                    Width = 75,
                    FillWeight = 75,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                ttn2Column.DefaultCellStyle.BackColor = Color.LightYellow;
                gridData.Columns.Add(ttn2Column);

                var ttnMeanColumn = new DataGridViewTextBoxColumn
                {
                    Name = "TtnMean",
                    HeaderText = "Chỉ thị tủ của loạt đo(°C)",
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
                    row.Cells[TtnMeanColumn].ReadOnly = true;
                    row.Cells[TtnMeanColumn].Style.BackColor = SystemColors.Control;
                }

                AddSummaryRow("Chỉ thị chuẩn", Color.LightBlue);
                AddSummaryRow("Độ lệch chuẩn", SystemColors.Control);
                AddSummaryRow("ĐKĐBĐ chuẩn (loại A)", SystemColors.Control);
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

                gridData.Rows[correctedMeanRow].Cells[Ttn1Column].Value = "—";
                gridData.Rows[correctedMeanRow].Cells[Ttn2Column].Value = "—";
                gridData.Rows[correctedMeanRow].Cells[TtnMeanColumn].Value = r.Ttn.ToString("F4");
                gridData.Rows[stdDevRow].Cells[Ttn1Column].Value = "—";
                gridData.Rows[stdDevRow].Cells[Ttn2Column].Value = "—";
                gridData.Rows[stdDevRow].Cells[TtnMeanColumn].Value = "—";
                gridData.Rows[typeARow].Cells[Ttn1Column].Value = "—";
                gridData.Rows[typeARow].Cells[Ttn2Column].Value = "—";
                gridData.Rows[typeARow].Cells[TtnMeanColumn].Value = "—";

                for (int j = 0; j < _j; j++)
                {
                    gridData.Rows[correctedMeanRow].Cells[ChannelColumn(j)].Value = r.ChannelCorrectedMeans[j].ToString("F4");
                    gridData.Rows[stdDevRow].Cells[ChannelColumn(j)].Value = r.ChannelStdDevs[j].ToString("F4");
                    gridData.Rows[typeARow].Cells[ChannelColumn(j)].Value = r.ChannelTypeAUncertainties[j].ToString("F4");
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
            InitializeMathResultLabels();
            ResetResultLabels();
        }

        private void InitializeMathResultLabels()
        {
            if (_mathResultLabelsInitialized)
                return;

            ReplaceWithMathLabel(lblDtTitle, "Số hiệu chỉnh _Delta_t", 9f);
            ReplaceWithMathLabel(lblUch1Title, "Tản mát của chuẩn u SUB{ch1}", 9f);
            ReplaceWithMathLabel(lblUch2Title, "ĐKĐBĐ chuẩn u SUB{ch2}", 9f);
            ReplaceWithMathLabel(lblUchTitle, "Liên hợp chuẩn u SUB{ch}", 9f);
            ReplaceWithMathLabel(lblUbkTitle, "Liên hợp tủ u SUB{bk}", 9f);
            ReplaceWithMathLabel(lblUTitle, "U(k=2, P=95%)", 11f);

            _mathResultLabelsInitialized = true;
        }

        private static void ReplaceWithMathLabel(Label source, string mathText, float baseFontSize = 10f)
        {
            if (source.Parent is not TableLayoutPanel parent)
                return;

            var position = parent.GetPositionFromControl(source);
            int index = parent.Controls.GetChildIndex(source);

            var mathLabel = new MathLabel
            {
                Name = source.Name + "Math",
                MathText = mathText,
                BaseFontSize = baseFontSize,
                Dock = source.Dock,
                Margin = source.Margin,
                Padding = source.Padding,
                ForeColor = source.ForeColor,
                BackColor = Color.Transparent,
                TabIndex = source.TabIndex
            };

            parent.Controls.Remove(source);
            source.Dispose();

            parent.Controls.Add(mathLabel, position.Column, position.Row);
            parent.Controls.SetChildIndex(mathLabel, index);
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
                DoPhanGiai = input.ResolutionA,
                HeSoPhanGiai = input.ResolutionD,
                ThongSoChuanJson = SerializeStandardTraceability(input),
            };

            for (int j = 0; j < input.J && j < row.Kenh.Length; j++)
                row.Kenh[j] = r.ChannelCorrectedMeans[j];

            row.ChiTietLanDos = ExtractChiTietLanDo();
            return row;
        }

        private static string SerializeStandardTraceability(UncertaintyInput input)
        {
            var data = new StandardTraceabilityData
            {
                Corrections = input.Corrections.ToArray(),
                UValues = input.UValues.ToArray(),
                DeltaValues = input.DeltaValues.ToArray(),
            };

            return JsonSerializer.Serialize(data);
        }

        private static StandardTraceabilityData? DeserializeStandardTraceability(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return JsonSerializer.Deserialize<StandardTraceabilityData>(json);
            }
            catch (JsonException)
            {
                return null;
            }
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
                MessageBox.Show("Form không được mở với callback. Vui lòng mở lại từ màn hình chính.", "Thông báo",
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

            lblStatus.Text = $"Đã thêm điểm {txtGiaTriDat.Text} °C  " +
                             $"(tổng {stt} điểm)";

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

            // ── Dòng phân cách nếu đã có điểm trước ─────────────────────────
            if (_gridBudget.Rows.Count > 0)
                AddBudgetSeparatorRow($"Điểm {pointIndex}");

            // ── u1: Tản mát KQ đo của chuẩn ──────────────────────────────────
            AddBudgetRow($"u1-{pointIndex}",
                         "Tản mát KQ đo của chuẩn",
                         r.Uch1, "°C", "1", 1.0, 1.0, r.Uch1);

            // ── u2: Tản mát KQ đo của UUT ────────────────────────────────────
            AddBudgetRow($"u2-{pointIndex}",
                         "Tản mát KQ đo của UUT",
                         r.Ubk1, "°C", "1", 1.0, 1.0, r.Ubk1);

            // ── u3: ĐKĐBĐ của chuẩn ──────────────────────────────────────────
            AddBudgetRow("u3",
                         "ĐKĐBĐ của chuẩn",
                         useU ? uMax : delta, "°C",
                         useU ? "2" : "√3",
                         useU ? 2.0 : Math.Sqrt(3),
                         1.0, r.Uch2);

            // ── u4: Độ phân giải của UUT ──────────────────────────────────────
            AddBudgetRow("u4",
                         "Độ phân giải của UUT",
                         A * d, "°C", "√3", Math.Sqrt(3), 1.0, r.Ubk4);

            // ── u5: Độ ổn định ────────────────────────────────────────────────
            AddBudgetRow($"u5-{pointIndex}",
                         "Độ ổn định",
                         r.DeltaOd, "°C", "√3", Math.Sqrt(3), 1.0, r.Ubk2);

            // ── u6: Độ đồng đều ───────────────────────────────────────────────
            AddBudgetRow($"u6-{pointIndex}",
                         "Độ đồng đều",
                         r.DeltaDd, "°C", "√3", Math.Sqrt(3), 1.0, r.Ubk3);

            // ── Tổng hợp điểm này ────────────────────────────────────────────
            AddSummaryBudgetRow($"u_ch-{pointIndex}",
                                $"Liên hợp chuẩn — điểm {pointIndex}",
                                r.Uc, Color.FromArgb(220, 235, 255));

            AddSummaryBudgetRow($"u_bk-{pointIndex}",
                                $"Liên hợp tủ nhiệt — điểm {pointIndex}",
                                r.Ubk, Color.FromArgb(220, 235, 255));

            AddSummaryBudgetRow($"U-{pointIndex}",
                                $"ĐKĐB mở rộng — điểm {pointIndex}, k=2, P=95%",
                                r.UFinal,
                                Color.FromArgb(200, 255, 200),
                                prefix: "±", bold: true,
                                foreColor: Color.DarkGreen);

            // Scroll xuống dòng mới nhất
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
            RebuildBudgetColumns();

            // Index điểm hiệu chuẩn hiện tại (đếm từ kết quả đã thêm)
            int pointIndex = _gridResults.Rows.Count; // điểm thứ mấy (1-based)

            int n = _lastInput!.N;
            int j = _lastInput!.J;

            // ── u1-{pointIndex}: Tản mát KQ đo của chuẩn ────────────────────
            // = u_ch1 tổng hợp tại điểm này (CT7) — một dòng duy nhất
            AddBudgetRow(
                sym: $"u1-{pointIndex}",
                source: "Tản mát KQ đo của chuẩn",
                rawValue: r.Uch1,   // đã là √Σ(uch1,j²) — CT7
                unit: "°C",
                divisorText: "1",
                divisorValue: 1.0,
                ci: 1.0,
                ui: r.Uch1);

            // ── u2-{pointIndex}: Tản mát KQ đo của UUT ──────────────────────
            AddBudgetRow(
                sym: $"u2-{pointIndex}",
                source: "Tản mát KQ đo của UUT",
                rawValue: r.Ubk1,   // CT13
                unit: "°C",
                divisorText: "1",
                divisorValue: 1.0,
                ci: 1.0,
                ui: r.Ubk1);

            // ── u3: ĐKĐBĐ của chuẩn ─────────────────────────────────────────
            bool useU = _lastInput.UseUMethod;
            double uMax = _lastInput.UValues[0];
            double delta = _lastInput.DeltaValues[0];
            AddBudgetRow(
                sym: "u3",
                source: "ĐKĐBĐ của chuẩn",
                rawValue: useU ? uMax : delta,
                unit: "°C",
                divisorText: useU ? "2" : "√3",
                divisorValue: useU ? 2.0 : Math.Sqrt(3),
                ci: 1.0,
                ui: r.Uch2);

            // ── u4: Độ phân giải của UUT ─────────────────────────────────────
            double A = _lastInput.ResolutionA;
            double d = _lastInput.ResolutionD;
            AddBudgetRow(
                sym: "u4",
                source: "Độ phân giải của UUT",
                rawValue: A * d,
                unit: "°C",
                divisorText: "√3",
                divisorValue: Math.Sqrt(3),
                ci: 1.0,
                ui: r.Ubk4);

            // ── u5-{pointIndex}: Độ ổn định ──────────────────────────────────
            // δt_od tại điểm này = max qua k kênh của ½(max-min)
            AddBudgetRow(
                sym: $"u5-{pointIndex}",
                source: "Độ ổn định",
                rawValue: r.DeltaOd,   // CT5
                unit: "°C",
                divisorText: "√3",
                divisorValue: Math.Sqrt(3),
                ci: 1.0,
                ui: r.Ubk2);    // CT15

            // ── u6-{pointIndex}: Độ đồng đều ────────────────────────────────
            AddBudgetRow(
                sym: $"u6-{pointIndex}",
                source: "Độ đồng đều",
                rawValue: r.DeltaDd,   // CT6
                unit: "°C",
                divisorText: "√3",
                divisorValue: Math.Sqrt(3),
                ci: 1.0,
                ui: r.Ubk3);    // CT16

            // ── Separator + Tổng hợp ─────────────────────────────────────────
            AddSummaryBudgetRow("u_ch",
                $"Độ KĐB chuẩn liên hợp — điểm {pointIndex} (CT12)",
                r.Uc,
                Color.FromArgb(220, 235, 255));

            AddSummaryBudgetRow("u_bk",
                $"Độ KĐB tủ nhiệt liên hợp — điểm {pointIndex} (CT18)",
                r.Ubk,
                Color.FromArgb(220, 235, 255));

            AddSummaryBudgetRow("U",
                $"Độ KĐB mở rộng — điểm {pointIndex}, k=2, P=95% (CT19)",
                r.UFinal,
                Color.FromArgb(200, 255, 200),
                prefix: "±",
                bold: true,
                foreColor: Color.DarkGreen);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

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

            Add("BudSym", "Ký hiệu", 55);
            Add("BudSource", "Nguồn gây ra ĐKĐBĐ", 200);
            Add("BudVal", "Giá trị", 70);
            Add("BudUnit", "Đơn vị", 45);
            Add("BudDiv", "Hệ số chia", 70);
            Add("BudCi", "Ci", 40);
            Add("BudUi", "ĐKĐB chuẩn (ui)", 80);

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
            // ui truyền vào được ưu tiên nếu đã tính sẵn, fallback tính lại
            double displayUi = double.IsNaN(ui) ? safeUi : ui;

            int idx = _gridBudget.Rows.Add(
                sym,
                source,
                double.IsNaN(rawValue) || rawValue == 0 ? "—" : rawValue.ToString("F4"),
                unit,
                divisorText,
                ci.ToString("F1"),
                displayUi == 0 ? "—" : displayUi.ToString("F4"));

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
                "—", "°C", "—", "—",
                $"{prefix}{value:F4}");

            var style = _gridBudget.Rows[idx].DefaultCellStyle;
            style.BackColor = backColor;
            style.Font = new Font(_gridBudget.Font,
                                  bold ? FontStyle.Bold : FontStyle.Regular);
            if (foreColor.HasValue)
                style.ForeColor = foreColor.Value;
        }

        private void LoadStoredCalculationParameters(CalibrationResultRow row)
        {
            if (!double.IsNaN(row.DoPhanGiai) && !double.IsInfinity(row.DoPhanGiai))
                txtResA.Text = row.DoPhanGiai.ToString("F2");

            if (!double.IsNaN(row.HeSoPhanGiai) && !double.IsInfinity(row.HeSoPhanGiai))
                txtResD.Text = row.HeSoPhanGiai.ToString("F2");

            rbUseU.Checked = !string.Equals(row.PhuongPhapB, "Delta", StringComparison.OrdinalIgnoreCase);
            rbUseDelta.Checked = !rbUseU.Checked;

            var traceability = DeserializeStandardTraceability(row.ThongSoChuanJson);
            if (traceability == null)
                return;

            if (traceability.UValues.Length > 0)
                txtUMax.Text = traceability.UValues[0].ToString("F2");

            if (traceability.DeltaValues.Length > 0)
                txtDeltaMax.Text = traceability.DeltaValues[0].ToString("F2");

            for (int i = 0; i < _j && i < traceability.Corrections.Length; i++)
                _corrections[i] = traceability.Corrections[i];

            for (int i = 0; i < _correctionBoxes.Count && i < _corrections.Length; i++)
                _correctionBoxes[i].Text = _corrections[i].ToString("F2");
        }

        /// <summary>
        /// Load dữ liệu từ một điểm đo đã tính để cho phép chỉnh sửa lại.
        /// </summary>
        public void LoadExistingData(CalibrationResultRow row)
        {
            // Cập nhật config trước
            SetConfiguration(
                row.SoKenh > 0 ? row.SoKenh : _j,
                row.SoLanDo > 0 ? row.SoLanDo : _n,
                notifyOwner: false);

            // Điền giá trị đặt
            txtGiaTriDat.Text = row.GiaTriDat.ToString("F1");
            LoadStoredCalculationParameters(row);

            // Nếu không có chi tiết từng lần đo, dùng giá trị trung bình kênh
            if (row.ChiTietLanDos == null || row.ChiTietLanDos.Count == 0)
            {
                _editMode = true;
                btnCalculateAndAdd.Text = "Tính lại và Lưu chỉnh sửa";
                return;
            }

            _suppressGridEvents = true;
            try
            {
                for (int i = 0; i < _n; i++)
                {
                    int lanDo = i + 1;

                    // Điền giá trị từng kênh
                    for (int j = 0; j < _j; j++)
                    {
                        var detail = row.ChiTietLanDos
                            .FirstOrDefault(d => d.LanDo == lanDo && d.Kenh == j + 1);

                        if (detail != null)
                            gridData.Rows[i].Cells[ChannelColumn(j)].Value =
                                detail.GiaTri.ToString("F2");
                    }

                    // Điền chỉ thị tủ — ttn1 và ttn2 đều dùng chi_thi_uut
                    // (giá trị lưu là (ttn1+ttn2)/2; khi load lại,
                    //  điền vào cả hai ô để tính lại cho đúng)
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

            // Tính lại ngay để hiển thị kết quả
            RecalculateAll(showErrors: false);
            _editMode = true;
            btnCalculateAndAdd.Text = "Tính lại và Lưu chỉnh sửa";
        }
    }
}
