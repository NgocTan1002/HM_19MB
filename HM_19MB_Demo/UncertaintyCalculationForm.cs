using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HM_19MB_Demo.Data;
using HM_19MB_Demo.Models;
using HM_19MB_Demo.Services;

namespace HM_19MB_Demo
{
    public partial class UncertaintyCalculationForm : Form
    {
        private int _j = 3;
        private int _n = 10;
        private int _phienId;
        private readonly Func<CalibrationResultRow, Task>? _onResultAdded;
        private readonly Action<int, int>? _onConfigChanged;
        private bool _updatingConfigFromOwner;

        // Lưu kết quả tính toán gần nhất để dùng khi bấm "Thêm vào bảng"
        private CalibrationResultRow? _lastCalculatedResult = null;

        private DataGridView _gridResults = null!;
        private DataGridView _gridBudget = null!;

        // ── MathLabel thay thế các Label công thức ───────────────────────
        private MathLabel mlTchResult = null!;
        private MathLabel mlUch1Result = null!;
        private MathLabel mlUch2Result = null!;
        private MathLabel mlUch1Final = null!;
        private MathLabel mlUch2Final = null!;
        private MathLabel mlUcFinal = null!;
        private MathLabel mlUbk1 = null!;
        private MathLabel mlUbk2 = null!;
        private MathLabel mlUbk3 = null!;
        private MathLabel mlUbk4 = null!;
        private MathLabel mlUbkResult = null!;
        private MathLabel mlTtnResult = null!;
        private MathLabel mlDeltaT = null!;
        private MathLabel mlDeltaOd = null!;
        private MathLabel mlDeltaDd = null!;
        private MathLabel mlUFinal = null!;

        // ── Constructor mặc định (tương thích cũ, dùng cho ShowDialog) ───
        public UncertaintyCalculationForm()
            : this(0, null)
        {
        }

        // ── Constructor mới: nhận phienId và callback ────────────────────
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
            InitResultsGrid();
            InitBudgetGrid();
            ReplaceLabelWithMath();
            WireEvents();
            ApplyConfiguration();

            btnAddToTable.Enabled = false;
        }

        public UncertaintyCalculationForm(int kenhCount, int? phienId, Action<CalibrationResultRow>? onResultAdded)
            : this(kenhCount, 10, phienId, onResultAdded, null)
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

            if (!changed) return;

            ApplyConfiguration();
            _lastCalculatedResult = null;
            btnAddToTable.Enabled = false;

            if (notifyOwner && !_updatingConfigFromOwner)
                _onConfigChanged?.Invoke(_j, _n);
        }

        // ── Khởi tạo MathLabel ───────────────────────────────────────────

        private void ReplaceLabelWithMath()
        {
            // step2Panel
            mlUch1Result = Swap(step2Panel, lblUch1Result, "u SUB{ch1} = (chưa tính)", 11f, Color.DarkBlue);

            // pnlStandards
            mlUch2Result = Swap(pnlStandards, lblUch2Result, "u SUB{ch2} = (chưa tính)", 11f, Color.DarkGreen);

            // resultPanel — hàng 1
            mlUch1Final = Swap(resultPanel, lblUch1Final, "u SUB{ch1} = ---", 11f, Color.Black);
            mlUch2Final = Swap(resultPanel, lblUch2Final, "u SUB{ch2} = ---", 11f, Color.Black);
            mlUcFinal = Swap(resultPanel, lblUcFinal, "u SUB{c} = ---", 11f, Color.DarkBlue);
            mlUbk1 = Swap(resultPanel, lblUbk1, "u SUB{bk1} = ---", 11f, Color.Black);
            mlUbk2 = Swap(resultPanel, lblUbk2, "u SUB{bk2} = ---", 11f, Color.Black);
            mlUbk3 = Swap(resultPanel, lblUbk3, "u SUB{bk3} = ---", 11f, Color.Black);
            mlUbk4 = Swap(resultPanel, lblUbk4, "u SUB{bk4} = ---", 11f, Color.Black);
            mlUbkResult = Swap(resultPanel, lblUbkResult, "u SUB{bk} = ---", 11f, Color.DarkOrange);

            // resultPanel — hàng 2
            mlTchResult = Swap(resultPanel, lblTchResult, "BAR{t}SUB{ch} = ---", 11f, Color.DarkMagenta);
            mlTtnResult = Swap(resultPanel, lblTtnResult, "BAR{t}SUB{tn} = ---", 11f, Color.Black);
            mlDeltaT = Swap(resultPanel, lblDeltaT, "_Delta_t = ---", 11f, Color.Black);
            mlDeltaOd = Swap(resultPanel, lblDeltaOd, "_delta_t SUB{od} = ---", 11f, Color.Black);
            mlDeltaDd = Swap(resultPanel, lblDeltaDd, "_delta_t SUB{dd} = ---", 11f, Color.Black);

            // resultPanel — kết quả cuối
            mlUFinal = Swap(resultPanel, lblUFinal,
                "U = --- °C  (k=2, P=95%)",
                13f, Color.DarkRed);
        }

        private static MathLabel Swap(Control parent, Label original,
                                       string mathText, float fontSize, Color color)
        {
            var ml = new MathLabel
            {
                MathText = mathText,
                BaseFontSize = fontSize,
                ForeColor = color,
                BackColor = Color.Transparent,
                Location = original.Location,
                Size = new Size(original.Width + 80, Math.Max(original.Height + 8, 28)),
                Anchor = original.Anchor,
            };
            original.Visible = false;
            parent.Controls.Add(ml);
            ml.BringToFront();
            return ml;
        }

        // ── Event wiring ─────────────────────────────────────────────────

        private void WireEvents()
        {
            btnApplyConfig.Click += BtnApplyConfig_Click;
            btnCalculate.Click += BtnCalculate_Click;
            btnSaveToDb.Click += BtnSaveToDb_Click;
            btnAddToTable.Click += BtnAddToTable_Click;

            gridMeasurements.CellValueChanged += GridMeasurements_CellValueChanged;
            gridStandards.CellValueChanged += GridStandards_CellValueChanged;
            gridIndicator.CellValueChanged += GridIndicator_CellValueChanged;

            rbUseU.CheckedChanged += (s, e) => RecalculateAll();
            rbUseDelta.CheckedChanged += (s, e) => RecalculateAll();
        }

        private void BtnApplyConfig_Click(object? sender, EventArgs e)
        {
            SetConfiguration((int)numChannels.Value, (int)numMeasurements.Value, notifyOwner: true);
        }

        // ── ApplyConfiguration ───────────────────────────────────────────

        private void ApplyConfiguration()
        {
            // Bảng đo
            gridMeasurements.Columns.Clear();
            gridMeasurements.Rows.Clear();

            gridMeasurements.Columns.Add("MeasurementNo", "Lần đo");
            gridMeasurements.Columns[0].ReadOnly = true;
            gridMeasurements.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;

            for (int j = 1; j <= _j; j++)
                gridMeasurements.Columns.Add($"Channel{j}", $"Kênh đo {j}");

            for (int i = 1; i <= _n; i++)
            {
                var row = new DataGridViewRow();
                row.CreateCells(gridMeasurements);
                row.Cells[0].Value = $"Lần {i}";
                for (int j = 1; j <= _j; j++) row.Cells[j].Value = "0.0";
                gridMeasurements.Rows.Add(row);
            }

            AddSummaryRow("Trung bình (t̄j)", Color.LightYellow);
            AddSummaryRow("Độ lệch chuẩn (Sj)", Color.LightCyan);
            AddSummaryRow("uch1,j", Color.LightGreen);
            DisableGridSorting(gridMeasurements);

            // Bảng chuẩn
            gridStandards.Columns.Clear();
            gridStandards.Rows.Clear();

            gridStandards.Columns.Add("Parameter", "Thông số");
            gridStandards.Columns[0].ReadOnly = true;
            gridStandards.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
            gridStandards.Columns[0].Width = 120;

            for (int j = 1; j <= _j; j++)
                gridStandards.Columns.Add($"Channel{j}", $"Kênh đo {j}");

            gridStandards.Columns.Add("Max", "Max");
            var maxColumn = gridStandards.Columns["Max"]!;
            maxColumn.ReadOnly = true;
            maxColumn.DefaultCellStyle.Font = new Font(gridStandards.Font, FontStyle.Bold);

            // Dòng 0: U
            var rowU = new DataGridViewRow();
            rowU.CreateCells(gridStandards);
            rowU.Cells[0].Value = "U (°C)";
            for (int j = 1; j <= _j; j++) rowU.Cells[j].Value = "0.0";
            rowU.Cells[_j + 1].Value = "0.0";
            gridStandards.Rows.Add(rowU);

            // Dòng 1: ∂
            var rowDelta = new DataGridViewRow();
            rowDelta.CreateCells(gridStandards);
            rowDelta.Cells[0].Value = "∂ (°C)";
            for (int j = 1; j <= _j; j++) rowDelta.Cells[j].Value = "0.0";
            rowDelta.Cells[_j + 1].Value = "0.0";
            gridStandards.Rows.Add(rowDelta);

            // Dòng 2: ∂t_j (số hiệu chính)
            var rowCorrection = new DataGridViewRow();
            rowCorrection.CreateCells(gridStandards);
            rowCorrection.Cells[0].Value = "∂t_j (°C)";
            for (int j = 1; j <= _j; j++) rowCorrection.Cells[j].Value = "0.0";
            rowCorrection.Cells[_j + 1].Value = "";
            rowCorrection.Cells[_j + 1].ReadOnly = true;
            gridStandards.Rows.Add(rowCorrection);
            DisableGridSorting(gridStandards);

            // Bảng chỉ thị
            gridIndicator.Columns.Clear();
            gridIndicator.Rows.Clear();

            gridIndicator.Columns.Add("Parameter", "Thông số");
            gridIndicator.Columns[0].ReadOnly = true;
            gridIndicator.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
            gridIndicator.Columns[0].Width = 130;

            for (int i = 1; i <= _n; i++)
                gridIndicator.Columns.Add($"Measure{i}", $"Lần {i}");

            // Dòng 0: t_tn1i
            var rowTtn1 = new DataGridViewRow();
            rowTtn1.CreateCells(gridIndicator);
            rowTtn1.Cells[0].Value = "t_tn1 (°C)";
            for (int i = 1; i <= _n; i++) rowTtn1.Cells[i].Value = "0.0";
            gridIndicator.Rows.Add(rowTtn1);

            // Dòng 1: t_tn2i
            var rowTtn2 = new DataGridViewRow();
            rowTtn2.CreateCells(gridIndicator);
            rowTtn2.Cells[0].Value = "t_tn2 (°C)";
            for (int i = 1; i <= _n; i++) rowTtn2.Cells[i].Value = "0.0";
            gridIndicator.Rows.Add(rowTtn2);

            // Dòng 2: t̄_tn (readonly, tự tính)
            var rowTtnMean = new DataGridViewRow();
            rowTtnMean.CreateCells(gridIndicator);
            rowTtnMean.Cells[0].Value = "t̄_tn (°C)";
            rowTtnMean.DefaultCellStyle.BackColor = Color.LightYellow;
            rowTtnMean.ReadOnly = true;
            gridIndicator.Rows.Add(rowTtnMean);
            DisableGridSorting(gridIndicator);
        }

        private void AddSummaryRow(string label, Color bgColor)
        {
            var row = new DataGridViewRow();
            row.CreateCells(gridMeasurements);
            row.Cells[0].Value = label;
            row.DefaultCellStyle.BackColor = bgColor;
            row.ReadOnly = true;
            gridMeasurements.Rows.Add(row);
        }

        private static void DisableGridSorting(DataGridView grid)
        {
            foreach (DataGridViewColumn column in grid.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        // ── Cell change handlers ─────────────────────────────────────────

        private void GridMeasurements_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 1 || e.RowIndex >= _n) return;
            // Khi dữ liệu thay đổi, kết quả cũ không còn hợp lệ
            _lastCalculatedResult = null;
            btnAddToTable.Enabled = false;
        }

        private void GridStandards_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 1 || e.RowIndex >= 3) return;
            if (e.ColumnIndex == _j + 1) return;
            _lastCalculatedResult = null;
            btnAddToTable.Enabled = false;
        }

        private void GridIndicator_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 1 || e.RowIndex >= 2) return;
            _lastCalculatedResult = null;
            btnAddToTable.Enabled = false;
        }

        // ── RecalculateAll ───────────────────────────────────────────────

        private void RecalculateAll()
        {
            try
            {
                var input = ReadInputFromGrid();
                var (isValid, error) = input.Validate();
                if (!isValid) { MessageBox.Show($"Dữ liệu không hợp lệ: {error}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                var result = UncertaintyService.Calculate(input);
                UpdateUIFromResult(result);
                _lastCalculatedResult = MapToCalibrationRow(result, input);
                btnAddToTable.Enabled = true;
            }
            catch (Exception ex)
            {
                _lastCalculatedResult = null;
                btnAddToTable.Enabled = false;
                MessageBox.Show($"Lỗi tính toán: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── ReadInputFromGrid — đọc dữ liệu từ grid → UncertaintyInput ──

        private UncertaintyInput ReadInputFromGrid()
        {
            double[,] measurementData = new double[_n, _j];
            for (int i = 0; i < _n; i++)
                for (int j = 0; j < _j; j++)
                    double.TryParse(gridMeasurements.Rows[i].Cells[j + 1].Value?.ToString(), out measurementData[i, j]);

            double[] corrections = new double[_j];
            for (int j = 0; j < _j; j++)
                double.TryParse(gridStandards.Rows[2].Cells[j + 1].Value?.ToString(), out corrections[j]);

            double[] uValues = new double[_j];
            double[] deltaValues = new double[_j];
            for (int j = 0; j < _j; j++)
            {
                double.TryParse(gridStandards.Rows[0].Cells[j + 1].Value?.ToString(), out uValues[j]);
                double.TryParse(gridStandards.Rows[1].Cells[j + 1].Value?.ToString(), out deltaValues[j]);
            }

            double[] ttn1 = new double[_n];
            double[] ttn2 = new double[_n];
            for (int i = 0; i < _n; i++)
            {
                double.TryParse(gridIndicator.Rows[0].Cells[i + 1].Value?.ToString(), out ttn1[i]);
                double.TryParse(gridIndicator.Rows[1].Cells[i + 1].Value?.ToString(), out ttn2[i]);
            }

            double.TryParse(txtGiaTriDat.Text, out double giaTriDat);

            return new UncertaintyInput
            {
                J = _j,
                N = _n,
                MeasurementData = measurementData,
                Corrections = corrections,
                UValues = uValues,
                DeltaValues = deltaValues,
                Ttn1 = ttn1,
                Ttn2 = ttn2,
                ResolutionA = (double)numResolutionA.Value,
                ResolutionD = (double)numResolutionD.Value,
                UseUMethod = rbUseU.Checked,
                GiaTriDat = giaTriDat,
            };
        }

        // ── UpdateUIFromResult — cập nhật MathLabel + grid từ kết quả ────

        private void UpdateUIFromResult(UncertaintyFullResult r)
        {
            // ── gridMeasurements summary rows: t̄j, Sj, uch1,j ───────────
            for (int j = 0; j < _j; j++)
            {
                gridMeasurements.Rows[_n].Cells[j + 1].Value = r.ChannelMeans[j].ToString("F4");
                gridMeasurements.Rows[_n + 1].Cells[j + 1].Value = r.ChannelStdDevs[j].ToString("F4");
                gridMeasurements.Rows[_n + 2].Cells[j + 1].Value = r.ChannelTypeAUncertainties[j].ToString("F4");
            }

            // ── gridStandards Max column ──────────────────────────────────
            gridStandards.Rows[0].Cells[_j + 1].Value = r.StandardResult.UMax.ToString("F4");
            gridStandards.Rows[1].Cells[_j + 1].Value = r.StandardResult.DeltaMax.ToString("F4");

            // ── gridIndicator row [2]: t̄_tn per measurement ──────────────
            for (int i = 0; i < _n; i++)
            {
                double.TryParse(gridIndicator.Rows[0].Cells[i + 1].Value?.ToString(), out double v1);
                double.TryParse(gridIndicator.Rows[1].Cells[i + 1].Value?.ToString(), out double v2);
                gridIndicator.Rows[2].Cells[i + 1].Value = ((v1 + v2) / 2.0).ToString("F4");
            }

            // ── MathLabel — CT(1)(2) ─────────────────────────────────────
            mlTchResult.MathText = $"BAR{{t}}SUB{{ch}} = {r.Tch:F4} °C";

            // ── MathLabel — CT(7) uch1 ───────────────────────────────────
            mlUch1Result.MathText = $"u SUB{{ch1}} = SQRT{{_sum_ u SUB{{ch1,j}}SUP{{2}}}} = {r.Uch1:F4} °C";
            mlUch1Final.MathText = $"u SUB{{ch1}} = {r.Uch1:F4} °C";

            // ── MathLabel — CT(10)/(11) uch2 ─────────────────────────────
            mlUch2Result.MathText = r.MethodUsed == "U"
                ? $"u SUB{{ch2}} = FRAC{{U}}{{2}} = {r.Uch2:F4} °C"
                : $"u SUB{{ch2}} = FRAC{{_delta_}}{{SQRT{{3}}}} = {r.Uch2:F4} °C";
            mlUch2Final.MathText = $"u SUB{{ch2}} = {r.Uch2:F4} °C";

            // ── MathLabel — CT(12) uc ────────────────────────────────────
            mlUcFinal.MathText =
                $"u SUB{{c}} = SQRT{{u SUB{{ch1}}SUP{{2}} + u SUB{{ch2}}SUP{{2}}}} = {r.Uc:F4} °C";

            // ── MathLabel — CT(3)(4) ttn, ΔT ────────────────────────────
            mlTtnResult.MathText = $"BAR{{t}}SUB{{tn}} = {r.Ttn:F4} °C";
            mlDeltaT.MathText =
                $"_Delta_t = BAR{{t}}SUB{{ch}} - BAR{{t}}SUB{{tn}} = {r.DeltaT:F4} °C";

            // ── MathLabel — CT(5)(6) δt_od, δt_dd ───────────────────────
            mlDeltaOd.MathText = $"_delta_t SUB{{od}} = _pm_{r.DeltaOd:F4} °C";
            mlDeltaDd.MathText = $"_delta_t SUB{{dd}} = _pm_{r.DeltaDd:F4} °C";

            // ── MathLabel — CT(13)–(18) ubk ─────────────────────────────
            mlUbk1.MathText = $"u SUB{{bk1}} = FRAC{{S}}{{SQRT{{n}}}} = {r.Ubk1:F4} °C";
            mlUbk2.MathText = $"u SUB{{bk2}} = FRAC{{_delta_t SUB{{od}}}}{{SQRT{{3}}}} = {r.Ubk2:F4} °C";
            mlUbk3.MathText = $"u SUB{{bk3}} = FRAC{{_delta_t SUB{{dd}}}}{{SQRT{{3}}}} = {r.Ubk3:F4} °C";
            mlUbk4.MathText = $"u SUB{{bk4}} = FRAC{{A _cdot_ d}}{{SQRT{{3}}}} = {r.Ubk4:F4} °C";
            mlUbkResult.MathText =
                $"u SUB{{bk}} = SQRT{{u SUB{{bk1}}SUP{{2}} + ... + u SUB{{bk4}}SUP{{2}}}} = {r.Ubk:F4} °C";

            // ── MathLabel — CT(19) U final ───────────────────────────────
            mlUFinal.MathText =
                $"U = 2 _cdot_ SQRT{{u SUB{{c}}SUP{{2}} + u SUB{{bk}}SUP{{2}}}} = _pm_{r.UFinal:F4} °C  (k=2, P=95%)";
        }

        // ── MapToCalibrationRow — chuyển kết quả → CalibrationResultRow ──

        private CalibrationResultRow MapToCalibrationRow(
            UncertaintyFullResult r, UncertaintyInput input)
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
                row.Kenh[j] = r.ChannelMeans[j];

            row.ChiTietLanDos = ExtractChiTietLanDo();

            return row;
        }

        private List<Data.ChiTietLanDo> ExtractChiTietLanDo()
        {
            var list = new List<Data.ChiTietLanDo>();

            for (int i = 0; i < _n; i++)
            {
                double ttn1 = 0, ttn2 = 0;
                double.TryParse(
                    gridIndicator.Rows[0].Cells[i + 1].Value?.ToString(), out ttn1);
                double.TryParse(
                    gridIndicator.Rows[1].Cells[i + 1].Value?.ToString(), out ttn2);
                double chiThi = (ttn1 + ttn2) / 2.0;

                for (int j = 0; j < _j; j++)
                {
                    if (!double.TryParse(
                            gridMeasurements.Rows[i].Cells[j + 1].Value?.ToString(),
                            out double val))
                        continue;

                    list.Add(new Data.ChiTietLanDo
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

        // ── Button handlers ───────────────────────────────────────────────

        private void BtnCalculate_Click(object? sender, EventArgs e) => RecalculateAll();

        private async void BtnAddToTable_Click(object? sender, EventArgs e)
        {
            if (_lastCalculatedResult == null)
            {
                MessageBox.Show("Vui lòng bấm 'Tính toán' trước.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_onResultAdded == null)
            {
                MessageBox.Show("Form không được mở với callback. Vui lòng mở lại từ màn hình chính.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Gọi callback để Form1 xử lý (thêm vào grid + lưu DB)
            await _onResultAdded(_lastCalculatedResult);

            // Lấy lại input + result để cập nhật Tab 2 và Tab 3
            var _input = ReadInputFromGrid();
            var _result = UncertaintyService.Calculate(_input);
            int _stt = _gridResults.Rows.Count + 1;
            AppendResultRow(_result, _input, _stt);
            BuildBudgetTable(_result);
            _tabMain.SelectedTab = _tabResults;

            ToastNotification.ShowSuccess($"Đã thêm điểm kiểm tra (đặt: {_lastCalculatedResult.GiaTriDat:F1}°C) vào bảng.");

            // Disable nút để tránh thêm trùng — người dùng phải tính lại mới được thêm tiếp
            btnAddToTable.Enabled = false;
            _lastCalculatedResult = null;
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
            {
                SetConfiguration(int.Parse(config[0]), int.Parse(config[1]), notifyOwner: true);
            }

            for (int i = 1; i < lines.Length && i <= _n; i++)
            {
                var values = lines[i].Split(',');
                for (int j = 0; j < values.Length && j < _j; j++)
                    gridMeasurements.Rows[i - 1].Cells[j + 1].Value = values[j];
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
            sb.Append("Lần đo");
            for (int j = 1; j <= _j; j++) sb.Append($",Kênh {j}");
            sb.AppendLine();

            for (int i = 0; i < _n; i++)
            {
                sb.Append($"Lần {i + 1}");
                for (int j = 1; j <= _j; j++)
                    sb.Append($",{gridMeasurements.Rows[i].Cells[j].Value}");
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("Kết quả tính toán:");
            sb.AppendLine($"uch1,{mlUch1Final.MathText}");
            sb.AppendLine($"uch2,{mlUch2Final.MathText}");
            sb.AppendLine($"uc,{mlUcFinal.MathText}");
            sb.AppendLine($"U,{mlUFinal.MathText}");

            File.WriteAllText(filePath, sb.ToString());
        }

        private async void BtnSaveToDb_Click(object? sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Chức năng lưu vào database đang được phát triển.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu database: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── InitResultsGrid — Tab 2 ──────────────────────────────────────

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

        // ── InitBudgetGrid — Tab 3 ───────────────────────────────────────

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

        // ── AppendResultRow — thêm 1 dòng vào Tab 2 ─────────────────────

        private void AppendResultRow(UncertaintyFullResult r,
                                     UncertaintyInput input, int stt)
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

        // ── BuildBudgetTable — rebuild Tab 3 ─────────────────────────────

        private void BuildBudgetTable(UncertaintyFullResult r)
        {
            _gridBudget.Rows.Clear();

            // Helper local function
            void AddRow(string sym, string source, double val,
                         double divisor, string unit = "°C", double ci = 1.0)
            {
                double ui = val / divisor;
                _gridBudget.Rows.Add(sym, source,
                    val.ToString("F4"), unit,
                    divisor == 1.0 ? "1" : divisor.ToString("F4"),
                    ci.ToString("F1"),
                    ui.ToString("F4"));
            }

            // Lấy tên phương pháp B từ r.MethodUsed
            string methodB = r.MethodUsed == "U"
                ? "Thiết bị chuẩn (U/2)"
                : "Thiết bị chuẩn (∂/√3)";

            // 6 thành phần chính
            AddRow("u_ch1", "Lặp lại phép đo \u2014 Loại A", r.Uch1, 1.0);
            AddRow("u_ch2", methodB, r.Uch2, 1.0);
            AddRow("u_bk1", "Lặp lại chỉ thị tủ \u2014 Loại A", r.Ubk1, 1.0);
            AddRow("u_bk2", "Độ ổn định tủ nhiệt", r.Ubk2, 1.0);
            AddRow("u_bk3", "Độ đồng đều tủ nhiệt", r.Ubk3, 1.0);
            AddRow("u_bk4", "Độ phân giải thiết bị chỉ thị", r.Ubk4, 1.0);

            // Dòng u_c
            int ucIdx = _gridBudget.Rows.Add(
                "u_c", "Liên hợp chuẩn tổng hợp",
                r.Uc.ToString("F4"), "°C", "\u2014", "\u2014",
                r.Uc.ToString("F4"));
            _gridBudget.Rows[ucIdx].DefaultCellStyle.BackColor =
                Color.FromArgb(220, 235, 255);
            _gridBudget.Rows[ucIdx].DefaultCellStyle.Font =
                new Font(_gridBudget.Font, FontStyle.Bold);

            // Dòng U cuối
            int uIdx = _gridBudget.Rows.Add(
                "U", "Mở rộng \u2014 k=2, P=95%",
                $"±{r.UFinal:F4}", "°C", "\u2014", "2",
                $"±{r.UFinal:F4}");
            _gridBudget.Rows[uIdx].DefaultCellStyle.BackColor =
                Color.FromArgb(200, 255, 200);
            _gridBudget.Rows[uIdx].DefaultCellStyle.Font =
                new Font(_gridBudget.Font, FontStyle.Bold);
            _gridBudget.Rows[uIdx].DefaultCellStyle.ForeColor = Color.DarkGreen;
        }
    }
}
