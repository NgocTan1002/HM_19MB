using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HM_19MB_Demo.Data;

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
                // ── Tính từng kênh: t̄j, Sj, uch1,j ─────────────────────────
                double[] channelMeans = new double[_j];
                for (int j = 0; j < _j; j++)
                {
                    double[] values = new double[_n];
                    for (int i = 0; i < _n; i++)
                        double.TryParse(gridMeasurements.Rows[i].Cells[j + 1].Value?.ToString(), out values[i]);

                    double mean = UncertaintyCalculator.CalculateMean(values);
                    channelMeans[j] = mean;
                    double stdDev = UncertaintyCalculator.CalculateStandardDeviation(values, mean);
                    double uch1j = UncertaintyCalculator.CalculateTypeAUncertainty(stdDev, _n);

                    gridMeasurements.Rows[_n].Cells[j + 1].Value = mean.ToString("F4");
                    gridMeasurements.Rows[_n + 1].Cells[j + 1].Value = stdDev.ToString("F4");
                    gridMeasurements.Rows[_n + 2].Cells[j + 1].Value = uch1j.ToString("F4");
                }

                // ── Đọc ∂t_j ─────────────────────────────────────────────────
                double[] corrections = new double[_j];
                for (int j = 0; j < _j; j++)
                    double.TryParse(gridStandards.Rows[2].Cells[j + 1].Value?.ToString(), out corrections[j]);

                // ── Ma trận dữ liệu đo n×j ────────────────────────────────────
                double[,] measurementData = new double[_n, _j];
                for (int i = 0; i < _n; i++)
                    for (int j = 0; j < _j; j++)
                        double.TryParse(gridMeasurements.Rows[i].Cells[j + 1].Value?.ToString(), out measurementData[i, j]);

                // ── CT(1)(2): t̄_ch, t̄_j hiệu chỉnh ─────────────────────────
                var (tch, channelCorrectedMeans) =
                    UncertaintyCalculator.CalculateCorrectedTemperature(measurementData, corrections);

                mlTchResult.MathText = $"BAR{{t}}SUB{{ch}} = {tch:F4} °C";

                // ── CT(7): uch1 tổng hợp ──────────────────────────────────────
                double[] uch1jValues = new double[_j];
                for (int j = 0; j < _j; j++)
                    double.TryParse(gridMeasurements.Rows[_n + 2].Cells[j + 1].Value?.ToString(), out uch1jValues[j]);

                double uch1 = UncertaintyCalculator.CalculateCombinedTypeA(uch1jValues);
                mlUch1Result.MathText = $"u SUB{{ch1}} = SQRT{{_sum_ u SUB{{ch1,j}}SUP{{2}}}} = {uch1:F4} °C";
                mlUch1Final.MathText = $"u SUB{{ch1}} = {uch1:F4} °C";

                // ── Max(U) và Max(∂) ──────────────────────────────────────────
                double[] uValues = new double[_j];
                double[] deltaValues = new double[_j];
                for (int j = 0; j < _j; j++)
                {
                    double.TryParse(gridStandards.Rows[0].Cells[j + 1].Value?.ToString(), out uValues[j]);
                    double.TryParse(gridStandards.Rows[1].Cells[j + 1].Value?.ToString(), out deltaValues[j]);
                }

                double uMax = UncertaintyCalculator.FindMax(uValues);
                double deltaMax = UncertaintyCalculator.FindMax(deltaValues);

                gridStandards.Rows[0].Cells[_j + 1].Value = uMax.ToString("F4");
                gridStandards.Rows[1].Cells[_j + 1].Value = deltaMax.ToString("F4");

                // ── CT(10)/(11): uch2 ─────────────────────────────────────────
                double uch2 = rbUseU.Checked
                    ? UncertaintyCalculator.CalculateTypeBFromU(uMax)
                    : UncertaintyCalculator.CalculateTypeBFromDelta(deltaMax);

                mlUch2Result.MathText = rbUseU.Checked
                    ? $"u SUB{{ch2}} = FRAC{{U}}{{2}} = {uch2:F4} °C"
                    : $"u SUB{{ch2}} = FRAC{{_delta_}}{{SQRT{{3}}}} = {uch2:F4} °C";
                mlUch2Final.MathText = $"u SUB{{ch2}} = {uch2:F4} °C";

                // ── CT(12): uc ────────────────────────────────────────────────
                double uc = UncertaintyCalculator.CalculateCombinedUncertainty(uch1, uch2);
                mlUcFinal.MathText =
                    $"u SUB{{c}} = SQRT{{u SUB{{ch1}}SUP{{2}} + u SUB{{ch2}}SUP{{2}}}} = {uc:F4} °C";

                // ── CT(5): δt_od ──────────────────────────────────────────────
                double deltaOd = UncertaintyCalculator.CalculateStability(measurementData);
                mlDeltaOd.MathText = $"_delta_t SUB{{od}} = _pm_{deltaOd:F4} °C";

                // ── CT(6): δt_dd ──────────────────────────────────────────────
                double deltaDD = UncertaintyCalculator.CalculateUniformity(channelCorrectedMeans);
                mlDeltaDd.MathText = $"_delta_t SUB{{dd}} = _pm_{deltaDD:F4} °C";

                // ── CT(3): t̄_tn; CT(4): Δt ───────────────────────────────────
                double[] ttn1 = new double[_n];
                double[] ttn2 = new double[_n];
                for (int i = 0; i < _n; i++)
                {
                    double.TryParse(gridIndicator.Rows[0].Cells[i + 1].Value?.ToString(), out ttn1[i]);
                    double.TryParse(gridIndicator.Rows[1].Cells[i + 1].Value?.ToString(), out ttn2[i]);
                }

                double ttn = UncertaintyCalculator.CalculateMeanIndicatorTemperature(ttn1, ttn2);

                // Cập nhật dòng t̄_tn trong grid
                for (int i = 0; i < _n; i++)
                    gridIndicator.Rows[2].Cells[i + 1].Value = ((ttn1[i] + ttn2[i]) / 2.0).ToString("F4");

                mlTtnResult.MathText = $"BAR{{t}}SUB{{tn}} = {ttn:F4} °C";

                double deltaT = UncertaintyCalculator.CalculateCorrection(tch, ttn);
                mlDeltaT.MathText =
                    $"_Delta_t = BAR{{t}}SUB{{ch}} - BAR{{t}}SUB{{tn}} = {deltaT:F4} °C";

                // ── CT(13)(14): ubk1 ─────────────────────────────────────────
                double[] ti = new double[_n];
                for (int i = 0; i < _n; i++)
                    ti[i] = (ttn1[i] + ttn2[i]) / 2.0;

                double ubk1 = UncertaintyCalculator.CalculateIndicatorTypeA(ti);

                // ── CT(15)(16)(17): ubk2, ubk3, ubk4 ─────────────────────────
                double ubk2 = UncertaintyCalculator.CalculateUbk2(deltaOd);
                double ubk3 = UncertaintyCalculator.CalculateUbk3(deltaDD);
                double A = (double)numResolutionA.Value;
                double d = (double)numResolutionD.Value;
                double ubk4 = UncertaintyCalculator.CalculateUbk4(A, d);

                // ── CT(18): ubk ───────────────────────────────────────────────
                double ubk = UncertaintyCalculator.CalculateCombinedUbk(ubk1, ubk2, ubk3, ubk4);

                mlUbk1.MathText = $"u SUB{{bk1}} = FRAC{{S}}{{SQRT{{n}}}} = {ubk1:F4} °C";
                mlUbk2.MathText = $"u SUB{{bk2}} = FRAC{{_delta_t SUB{{od}}}}{{SQRT{{3}}}} = {ubk2:F4} °C";
                mlUbk3.MathText = $"u SUB{{bk3}} = FRAC{{_delta_t SUB{{dd}}}}{{SQRT{{3}}}} = {ubk3:F4} °C";
                mlUbk4.MathText = $"u SUB{{bk4}} = FRAC{{A _cdot_ d}}{{SQRT{{3}}}} = {ubk4:F4} °C";
                mlUbkResult.MathText =
                    $"u SUB{{bk}} = SQRT{{u SUB{{bk1}}SUP{{2}} + ... + u SUB{{bk4}}SUP{{2}}}} = {ubk:F4} °C";

                // ── CT(19): U = 2√(uc² + ubk²) ───────────────────────────────
                double U_final = UncertaintyCalculator.CalculateFinalExpandedUncertainty(uc, ubk);
                mlUFinal.MathText =
                    $"U = 2 _cdot_ SQRT{{u SUB{{c}}SUP{{2}} + u SUB{{bk}}SUP{{2}}}} = _pm_{U_final:F4} °C  (k=2, P=95%)";

                // ── Lưu kết quả vào _lastCalculatedResult ────────────────────
                // GiaTriChiThi tự động lấy từ t̄_tn (CT3)
                double giaTriDat = 0;
                double.TryParse(txtGiaTriDat.Text, out giaTriDat);

                var result = new CalibrationResultRow
                {
                    GiaTriDat = giaTriDat,
                    GiaTriChiThi = ttn,          // tự động lấy từ t̄_tn
                    GiaTriTrungBinh = tch,
                    SoHieuChinh = deltaT,
                    DoOnDinh = deltaOd,
                    DoDongDeu = deltaDD,
                    DoKhongDamBao = U_final,
                    Uch1 = uch1,
                    Uch2 = uch2,
                    Uch = uc,
                    Ubk1 = ubk1,
                    Ubk2 = ubk2,
                    Ubk3 = ubk3,
                    Ubk4 = ubk4,
                    Ubk = ubk,
                    SoKenh = _j,
                    SoLanDo = _n,
                    PhuongPhapB = rbUseU.Checked ? "U" : "Delta",
                };

                // Gán giá trị từng vị trí chuẩn: trung bình thô từng kênh, chưa cộng số hiệu chính.
                // Các giá trị đã hiệu chỉnh vẫn dùng riêng cho tính t_ch, độ đồng đều và số hiệu chính.
                for (int j = 0; j < _j && j < result.Kenh.Length; j++)
                    result.Kenh[j] = channelMeans[j];

                _lastCalculatedResult = result;
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
    }
}
