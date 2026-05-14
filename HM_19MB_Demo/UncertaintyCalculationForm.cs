using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Text;

namespace HM_19MB_Demo
{
    /// <summary>
    /// Form tính toán độ không đảm bảo đo với UI động
    /// Có thể chỉnh sửa trong Visual Studio Designer
    /// </summary>
    public partial class UncertaintyCalculationForm : Form
    {
        private int _j = 3;  // Số kênh/đầu đo
        private int _n = 10; // Số lần đo

        public UncertaintyCalculationForm()
        {
            InitializeComponent();
            WireEvents();
            ApplyConfiguration();
        }

        private void WireEvents()
        {
            btnApplyConfig.Click += BtnApplyConfig_Click;
            btnCalculate.Click += BtnCalculate_Click;
            btnSaveToDb.Click += BtnSaveToDb_Click;
            
            gridMeasurements.CellValueChanged += GridMeasurements_CellValueChanged;
            gridStandards.CellValueChanged += GridStandards_CellValueChanged;
            gridIndicator.CellValueChanged += GridIndicator_CellValueChanged;
            
            rbUseU.CheckedChanged += (s, e) => RecalculateAll();
            rbUseDelta.CheckedChanged += (s, e) => RecalculateAll();
        }

        private void BtnApplyConfig_Click(object? sender, EventArgs e)
        {
            _j = (int)numChannels.Value;
            _n = (int)numMeasurements.Value;
            ApplyConfiguration();
        }

        private void ApplyConfiguration()
        {
            // Tạo lại bảng đo
            gridMeasurements.Columns.Clear();
            gridMeasurements.Rows.Clear();

            // Cột đầu tiên: Lần đo
            gridMeasurements.Columns.Add("MeasurementNo", "Lần đo");
            gridMeasurements.Columns[0].ReadOnly = true;
            gridMeasurements.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;

            // Các cột kênh
            for (int j = 1; j <= _j; j++)
            {
                gridMeasurements.Columns.Add($"Channel{j}", $"Kênh đo {j}");
            }

            // Thêm các hàng dữ liệu
            for (int i = 1; i <= _n; i++)
            {
                var row = new DataGridViewRow();
                row.CreateCells(gridMeasurements);
                row.Cells[0].Value = $"Lần {i}";
                for (int j = 1; j <= _j; j++)
                {
                    row.Cells[j].Value = "0.0";
                }
                gridMeasurements.Rows.Add(row);
            }

            // Thêm hàng tổng kết (t̄j, Sj, uch1,j)
            AddSummaryRow("Trung bình (t̄j)", Color.LightYellow);
            AddSummaryRow("Độ lệch chuẩn (Sj)", Color.LightCyan);
            AddSummaryRow("uch1,j", Color.LightGreen);

            // Tạo lại bảng thiết bị chuẩn (transpose: dòng = U/∂, cột = Kênh)
            gridStandards.Columns.Clear();
            gridStandards.Rows.Clear();

            // Cột đầu tiên: Tên thông số
            gridStandards.Columns.Add("Parameter", "Thông số");
            gridStandards.Columns[0].ReadOnly = true;
            gridStandards.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
            gridStandards.Columns[0].Width = 120;

            // Các cột kênh
            for (int j = 1; j <= _j; j++)
            {
                gridStandards.Columns.Add($"Channel{j}", $"Kênh đo {j}");
            }

            // Cột Max
            gridStandards.Columns.Add("Max", "Max");
            var maxColumn = gridStandards.Columns["Max"];
            maxColumn.ReadOnly = true;
            maxColumn.DefaultCellStyle.Font = new Font(gridStandards.Font, FontStyle.Bold);

            // Dòng 0: U
            var rowU = new DataGridViewRow();
            rowU.CreateCells(gridStandards);
            rowU.Cells[0].Value = "U (°C)";
            for (int j = 1; j <= _j; j++)
            {
                rowU.Cells[j].Value = "0.0";
            }
            rowU.Cells[_j + 1].Value = "0.0"; // Cột Max
            gridStandards.Rows.Add(rowU);

            // Dòng 1: ∂
            var rowDelta = new DataGridViewRow();
            rowDelta.CreateCells(gridStandards);
            rowDelta.Cells[0].Value = "∂ (°C)";
            for (int j = 1; j <= _j; j++)
            {
                rowDelta.Cells[j].Value = "0.0";
            }
            rowDelta.Cells[_j + 1].Value = "0.0"; // Cột Max
            gridStandards.Rows.Add(rowDelta);

            // Dòng 2: ∂t_j (số hiệu chính)
            var rowCorrection = new DataGridViewRow();
            rowCorrection.CreateCells(gridStandards);
            rowCorrection.Cells[0].Value = "∂t_j (°C)";
            for (int j = 1; j <= _j; j++) rowCorrection.Cells[j].Value = "0.0";
            rowCorrection.Cells[_j + 1].Value = ""; // Không có Max cho ∂t_j
            rowCorrection.Cells[_j + 1].ReadOnly = true;
            gridStandards.Rows.Add(rowCorrection);


            // --- Khởi tạo gridIndicator ---
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

            // Dòng 2: t̄_tn (tự tính, readonly)
            var rowTtnMean = new DataGridViewRow();
            rowTtnMean.CreateCells(gridIndicator);
            rowTtnMean.Cells[0].Value = "t̄_tn (°C)";
            rowTtnMean.DefaultCellStyle.BackColor = Color.LightYellow;
            rowTtnMean.ReadOnly = true;
            gridIndicator.Rows.Add(rowTtnMean);
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

        private void GridMeasurements_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 1) return;
            if (e.RowIndex >= _n) return; // Không tính cho hàng tổng kết

            RecalculateAll();
        }

        private void GridStandards_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 1) return;
            if (e.RowIndex >= 3) return; // Chỉ cho sửa 3 dòng đầu (U, ∂, ∂t_j)
            if (e.ColumnIndex == _j + 1) return; // Không cho sửa cột Max

            RecalculateAll();
        }

        private void GridIndicator_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 1) return;
            if (e.RowIndex >= 2) return; // dòng 2 (t̄_tn) readonly
            RecalculateAll();
        }

        private void RecalculateAll()
        {
            try
            {
                // Tính toán cho từng kênh
                for (int j = 0; j < _j; j++)
                {
                    double[] values = new double[_n];
                    for (int i = 0; i < _n; i++)
                    {
                        if (double.TryParse(gridMeasurements.Rows[i].Cells[j + 1].Value?.ToString(), out double val))
                        {
                            values[i] = val;
                        }
                    }

                    double mean = UncertaintyCalculator.CalculateMean(values);
                    double stdDev = UncertaintyCalculator.CalculateStandardDeviation(values, mean);
                    double uch1j = UncertaintyCalculator.CalculateTypeAUncertainty(stdDev, _n);

                    // Cập nhật vào bảng
                    gridMeasurements.Rows[_n].Cells[j + 1].Value = mean.ToString("F4");
                    gridMeasurements.Rows[_n + 1].Cells[j + 1].Value = stdDev.ToString("F4");
                    gridMeasurements.Rows[_n + 2].Cells[j + 1].Value = uch1j.ToString("F4");
                }

                // Đọc ∂t_j từ gridStandards dòng 2
                double[] corrections = new double[_j];
                for (int j = 0; j < _j; j++)
                {
                    if (double.TryParse(gridStandards.Rows[2].Cells[j + 1].Value?.ToString(), out double corr))
                        corrections[j] = corr;
                }

                // Lấy dữ liệu đo dạng 2 chiều
                double[,] measurementData = new double[_n, _j];
                for (int i = 0; i < _n; i++)
                    for (int j = 0; j < _j; j++)
                        if (double.TryParse(gridMeasurements.Rows[i].Cells[j + 1].Value?.ToString(), out double val))
                            measurementData[i, j] = val;

                // Tính t̄_ch (công thức 1 & 2)
                var (tch, channelCorrectedMeans) = UncertaintyCalculator
                    .CalculateCorrectedTemperature(measurementData, corrections);

                lblTchResult.Text = $"t̄_ch = {tch:F4} °C";

                // Tính uch1 tổng hợp
                double[] uch1jValues = new double[_j];
                for (int j = 0; j < _j; j++)
                {
                    if (double.TryParse(gridMeasurements.Rows[_n + 2].Cells[j + 1].Value?.ToString(), out double val))
                    {
                        uch1jValues[j] = val;
                    }
                }
                double uch1 = UncertaintyCalculator.CalculateCombinedTypeA(uch1jValues);
                lblUch1Result.Text = $"uch1 = {uch1:F4} °C";
                lblUch1Final.Text = $"uch1 = {uch1:F4} °C";

                // Tính Max(U) và Max(∂)
                double[] uValues = new double[_j];
                double[] deltaValues = new double[_j];
                for (int j = 0; j < _j; j++)
                {
                    // Dòng 0 = U, Dòng 1 = ∂
                    if (double.TryParse(gridStandards.Rows[0].Cells[j + 1].Value?.ToString(), out double u))
                        uValues[j] = u;
                    if (double.TryParse(gridStandards.Rows[1].Cells[j + 1].Value?.ToString(), out double delta))
                        deltaValues[j] = delta;
                }

                double uMax = UncertaintyCalculator.FindMax(uValues);
                double deltaMax = UncertaintyCalculator.FindMax(deltaValues);
                
                // Cập nhật vào cột Max (cột cuối cùng = _j + 1)
                gridStandards.Rows[0].Cells[_j + 1].Value = uMax.ToString("F4"); // Max của U
                gridStandards.Rows[1].Cells[_j + 1].Value = deltaMax.ToString("F4"); // Max của ∂

                // Cập nhật dòng Max (dòng 3)
                gridStandards.Rows[3].Cells[_j + 1].Value = Math.Max(uMax, deltaMax).ToString("F4");

                // Tính uch2
                double uch2 = rbUseU.Checked
                    ? UncertaintyCalculator.CalculateTypeBFromU(uMax)
                    : UncertaintyCalculator.CalculateTypeBFromDelta(deltaMax);

                string method = rbUseU.Checked ? "U" : "∂";
                lblUch2Result.Text = $"uch2 = {uch2:F4} °C (từ {method})";
                lblUch2Final.Text = $"uch2 = {uch2:F4} °C";

                // Tính uc (công thức 12)
                double uc = UncertaintyCalculator.CalculateCombinedUncertainty(uch1, uch2);
                lblUcFinal.Text = $"uc = {uc:F4} °C";

                // --- Khối A: δt_od và δt_dd (công thức 5 & 6) ---
                double deltaOd = UncertaintyCalculator.CalculateStability(measurementData);
                lblDeltaOd.Text = $"δt_od = ±{deltaOd:F4} °C";

                double deltaDD = UncertaintyCalculator.CalculateUniformity(channelCorrectedMeans);
                lblDeltaDd.Text = $"δt_dd = ±{deltaDD:F4} °C";

                // --- Khối B: t̄_tn, Δt (công thức 3 & 4) ---
                double[] ttn1 = new double[_n];
                double[] ttn2 = new double[_n];
                for (int i = 0; i < _n; i++)
                {
                    double.TryParse(gridIndicator.Rows[0].Cells[i + 1].Value?.ToString(), out ttn1[i]);
                    double.TryParse(gridIndicator.Rows[1].Cells[i + 1].Value?.ToString(), out ttn2[i]);
                }

                double ttn = UncertaintyCalculator.CalculateMeanIndicatorTemperature(ttn1, ttn2);
                for (int i = 0; i < _n; i++)
                    gridIndicator.Rows[2].Cells[i + 1].Value = ((ttn1[i] + ttn2[i]) / 2.0).ToString("F4");
                lblTtnResult.Text = $"t̄_tn = {ttn:F4} °C";

                double deltaT = UncertaintyCalculator.CalculateCorrection(tch, ttn);
                lblDeltaT.Text = $"Δt = {deltaT:F4} °C";

                // --- Khối C: ubk1..ubk4, ubk (công thức 13-18) ---
                double ubk1 = UncertaintyCalculator.CalculateIndicatorTypeA(measurementData);
                double ubk2 = UncertaintyCalculator.CalculateUbk2(deltaOd);
                double ubk3 = UncertaintyCalculator.CalculateUbk3(deltaDD);
                double A = (double)numResolutionA.Value;
                double d = (double)numResolutionD.Value;
                double ubk4 = UncertaintyCalculator.CalculateUbk4(A, d);
                double ubk = UncertaintyCalculator.CalculateCombinedUbk(ubk1, ubk2, ubk3, ubk4);
                lblUbk1.Text = $"ubk1 = {ubk1:F4} °C";
                lblUbk2.Text = $"ubk2 = {ubk2:F4} °C";
                lblUbk3.Text = $"ubk3 = {ubk3:F4} °C";
                lblUbk4.Text = $"ubk4 = {ubk4:F4} °C";
                lblUbkResult.Text = $"ubk = {ubk:F4} °C";

                // --- Khối D: U mở rộng cuối (công thức 19) ---
                double U_final = UncertaintyCalculator.CalculateFinalExpandedUncertainty(uc, ubk);
                lblUFinal.Text = $"U (k=2, P=95%) = ±{U_final:F4} °C";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tính toán: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCalculate_Click(object? sender, EventArgs e)
        {
            RecalculateAll();
            MessageBox.Show("Đã tính toán xong!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnImportCSV_Click(object? sender, EventArgs e)
        {
            using var openDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Import dữ liệu từ CSV"
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
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
        }

        private void ImportFromCSV(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            if (lines.Length < 2) return;

            // Dòng đầu: j,n
            var config = lines[0].Split(',');
            if (config.Length >= 2)
            {
                _j = int.Parse(config[0]);
                _n = int.Parse(config[1]);
                numChannels.Value = _j;
                numMeasurements.Value = _n;
                ApplyConfiguration();
            }

            // Các dòng tiếp theo: dữ liệu đo
            for (int i = 1; i < lines.Length && i <= _n; i++)
            {
                var values = lines[i].Split(',');
                for (int j = 0; j < values.Length && j < _j; j++)
                {
                    gridMeasurements.Rows[i - 1].Cells[j + 1].Value = values[j];
                }
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

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
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
        }

        private void ExportToCSV(string filePath)
        {
            var sb = new StringBuilder();

            // Dòng 1: Cấu hình
            sb.AppendLine($"{_j},{_n}");

            // Dòng 2: Header
            sb.Append("Lần đo");
            for (int j = 1; j <= _j; j++)
            {
                sb.Append($",Kênh {j}");
            }
            sb.AppendLine();

            // Dữ liệu đo
            for (int i = 0; i < _n; i++)
            {
                sb.Append($"Lần {i + 1}");
                for (int j = 1; j <= _j; j++)
                {
                    sb.Append($",{gridMeasurements.Rows[i].Cells[j].Value}");
                }
                sb.AppendLine();
            }

            // Kết quả
            sb.AppendLine();
            sb.AppendLine("Kết quả tính toán:");
            sb.AppendLine($"uch1,{lblUch1Final.Text}");
            sb.AppendLine($"uch2,{lblUch2Final.Text}");
            sb.AppendLine($"uc,{lblUcFinal.Text}");
            sb.AppendLine($"U,{lblUFinal.Text}");

            File.WriteAllText(filePath, sb.ToString());
        }

        private async void BtnSaveToDb_Click(object? sender, EventArgs e)
        {
            try
            {
                // TODO: Implement database saving
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
