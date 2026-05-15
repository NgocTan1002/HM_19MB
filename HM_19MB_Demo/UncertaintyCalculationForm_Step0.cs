using HM_19MB_Demo.Data;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace HM_19MB_Demo
{
    /// <summary>
    /// Partial class bổ sung:
    ///   • Constructor mới nhận kenhCount, phienId, callback
    ///   • Step 0: ô nhập "Giá trị đặt" và "Giá trị chỉ thị" (người dùng nhập thủ công)
    ///   • Nút "Thêm vào bảng" (chỉ enable sau khi bấm Tính toán thành công)
    /// </summary>
    public partial class UncertaintyCalculationForm : Form
    {
        // ── State bổ sung ─────────────────────────────────────────────────
        private int? _phienId;
        private Action<CalibrationResultRow>? _onResultAdded;
        private bool _calculationDone = false;

        // ── Controls Step 0 ───────────────────────────────────────────────
        private Panel _step0Panel = null!;
        private TextBox _txtGiaTriDat = null!;
        private TextBox _txtGiaTriChiThi = null!;
        private Button _btnAddToTable = null!;

        // ── Constructor mới — dùng khi mở từ Form1 ───────────────────────

        /// <summary>
        /// Constructor cho phép Form1 truyền số kênh, phiên hiệu chuẩn và callback.
        /// </summary>
        /// <param name="kenhCount">Số kênh k (3/5/9/10)</param>
        /// <param name="phienId">ID phiên hiệu chuẩn (null nếu chưa có session)</param>
        /// <param name="onResultAdded">Callback nhận CalibrationResultRow khi người dùng bấm "Thêm vào bảng"</param>
        public UncertaintyCalculationForm(
            int kenhCount,
            int? phienId,
            Action<CalibrationResultRow> onResultAdded)
        {
            _phienId = phienId;
            _onResultAdded = onResultAdded;

            InitializeComponent();

            // Đặt số kênh theo tham số
            _j = kenhCount;
            numChannels.Value = kenhCount;

            ReplaceLabelWithMath();
            WireEvents();
            ApplyConfiguration();

            // Thêm Step 0 và nút "Thêm vào bảng" vào UI
            InsertStep0Panel();
            InsertAddToTableButton();

            // Form non-modal: không phải dialog
            StartPosition = FormStartPosition.Manual;
            Location = new System.Drawing.Point(
                System.Windows.Forms.Screen.PrimaryScreen!.WorkingArea.Right - Width - 20,
                System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Top + 20);

            Text = $"Tính toán độ không đảm bảo đo — {kenhCount} kênh" +
                   (phienId.HasValue ? $" (Phiên #{phienId})" : "");
        }

        // ── Chèn Step 0 vào đầu mainLayout ───────────────────────────────

        private void InsertStep0Panel()
        {
            _step0Panel = new Panel
            {
                BackColor = Color.FromArgb(255, 248, 220),
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
            };

            var lblTitle = new Label
            {
                Text = "Bước 0 — Nhập giá trị tại điểm kiểm tra",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10),
            };

            var lblDat = new Label
            {
                Text = "Giá trị đặt (°C):",
                AutoSize = true,
                Location = new Point(10, 50),
                TextAlign = ContentAlignment.MiddleLeft,
            };

            _txtGiaTriDat = new TextBox
            {
                Location = new Point(150, 47),
                Size = new Size(100, 27),
                Text = "0.0",
            };

            var lblChiThi = new Label
            {
                Text = "Giá trị chỉ thị t̄_tn (°C):",
                AutoSize = true,
                Location = new Point(280, 50),
                TextAlign = ContentAlignment.MiddleLeft,
            };

            _txtGiaTriChiThi = new TextBox
            {
                Location = new Point(460, 47),
                Size = new Size(100, 27),
                Text = "0.0",
            };

            var lblNote = new Label
            {
                Text = "Lưu ý: Giá trị chỉ thị là t̄_tn — đọc thực tế từ màn hình tủ, KHÔNG tự tính.",
                AutoSize = true,
                Location = new Point(10, 80),
                ForeColor = Color.DarkRed,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
            };

            _step0Panel.Controls.Add(lblTitle);
            _step0Panel.Controls.Add(lblDat);
            _step0Panel.Controls.Add(_txtGiaTriDat);
            _step0Panel.Controls.Add(lblChiThi);
            _step0Panel.Controls.Add(_txtGiaTriChiThi);
            _step0Panel.Controls.Add(lblNote);

            // Chèn Step 0 vào đầu mainLayout (row 0 mới)
            mainLayout.RowCount++;
            mainLayout.RowStyles.Insert(0, new RowStyle(SizeType.Absolute, 100F));

            // Dịch chuyển tất cả control hiện tại xuống 1 row
            foreach (Control ctrl in mainLayout.Controls)
            {
                int row = mainLayout.GetRow(ctrl);
                mainLayout.SetRow(ctrl, row + 1);
            }

            mainLayout.Controls.Add(_step0Panel, 0, 0);

            // Tăng chiều cao form để có chỗ cho Step 0
            Height += 100;
        }

        // ── Chèn nút "Thêm vào bảng" vào bottomPanel ─────────────────────

        private void InsertAddToTableButton()
        {
            _btnAddToTable = new Button
            {
                Text = "✔ Thêm vào bảng",
                Size = new Size(140, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Enabled = false, // Chỉ enable sau khi đã Tính toán thành công
                Cursor = Cursors.Hand,
            };
            _btnAddToTable.FlatAppearance.BorderSize = 0;

            // Đặt vào bottomPanel, bên trái nút "Tính toán"
            _btnAddToTable.Location = new Point(
                btnCalculate.Left - _btnAddToTable.Width - 10,
                btnCalculate.Top);
            _btnAddToTable.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            bottomPanel.Controls.Add(_btnAddToTable);
            _btnAddToTable.Click += BtnAddToTable_Click;
        }

        // ── Override BtnCalculate để enable nút "Thêm vào bảng" ──────────

        /// <summary>
        /// Gọi sau RecalculateAll thành công — enable nút "Thêm vào bảng".
        /// Được wire trong WireEvents() thay cho btnCalculate.Click trực tiếp.
        /// </summary>
        private void OnCalculationSucceeded()
        {
            _calculationDone = true;
            _btnAddToTable.Enabled = true;
            _btnAddToTable.BackColor = Color.FromArgb(0, 123, 255);
        }

        // ── Xử lý nút "Thêm vào bảng" ────────────────────────────────────

        private void BtnAddToTable_Click(object? sender, EventArgs e)
        {
            if (!_calculationDone)
            {
                MessageBox.Show("Vui lòng bấm 'Tính toán' trước khi thêm vào bảng.",
                    "Chưa tính toán", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Parse giá trị đặt và chỉ thị
            if (!double.TryParse(_txtGiaTriDat.Text.Replace(',', '.'),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out double giaTriDat))
            {
                MessageBox.Show("Giá trị đặt không hợp lệ.", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtGiaTriDat.Focus();
                return;
            }

            if (!double.TryParse(_txtGiaTriChiThi.Text.Replace(',', '.'),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out double giaTriChiThi))
            {
                MessageBox.Show("Giá trị chỉ thị không hợp lệ.", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtGiaTriChiThi.Focus();
                return;
            }

            // Thu thập kết quả từ các MathLabel
            var row = BuildCalibrationResultRow(giaTriDat, giaTriChiThi);

            // Gọi callback về Form1
            _onResultAdded?.Invoke(row);

            // Reset trạng thái để chuẩn bị điểm tiếp theo
            _calculationDone = false;
            _btnAddToTable.Enabled = false;
            _btnAddToTable.BackColor = Color.Gray;

            // Thông báo thành công
            ToastNotification.ShowSuccess(
                $"Đã thêm điểm đặt {giaTriDat:F1}°C vào bảng kết quả.");
        }

        // ── Tạo CalibrationResultRow từ kết quả đã tính ──────────────────

        private CalibrationResultRow BuildCalibrationResultRow(
            double giaTriDat, double giaTriChiThi)
        {
            var row = new CalibrationResultRow
            {
                GiaTriDat = giaTriDat,
                GiaTriChiThi = giaTriChiThi,
                SoKenh = _j,
                SoLanDo = _n,
                PhuongPhapB = rbUseU.Checked ? "U" : "Delta",
            };

            // Đọc giá trị trung bình từng kênh (t̄_j hiệu chỉnh)
            for (int j = 0; j < _j && j < 10; j++)
            {
                if (gridMeasurements.Rows.Count > _n &&
                    double.TryParse(
                        gridMeasurements.Rows[_n].Cells[j + 1].Value?.ToString(),
                        NumberStyles.Float, CultureInfo.InvariantCulture, out double mean))
                {
                    // Cộng số hiệu chính ∂t_j
                    double correction = 0;
                    if (gridStandards.Rows.Count > 2)
                        double.TryParse(
                            gridStandards.Rows[2].Cells[j + 1].Value?.ToString(),
                            NumberStyles.Float, CultureInfo.InvariantCulture, out correction);

                    row.Kenh[j] = mean + correction;
                }
            }

            // Parse các kết quả từ MathLabel text
            row.GiaTriTrungBinh = ParseFromMathLabel(mlTchResult.MathText);
            row.SoHieuChinh = ParseFromMathLabel(mlDeltaT.MathText);
            row.DoOnDinh = ParseFromMathLabel(mlDeltaOd.MathText);
            row.DoDongDeu = ParseFromMathLabel(mlDeltaDd.MathText);

            row.Uch1 = ParseFromMathLabel(mlUch1Final.MathText);
            row.Uch2 = ParseFromMathLabel(mlUch2Final.MathText);
            row.Uch = ParseFromMathLabel(mlUcFinal.MathText);
            row.Ubk1 = ParseFromMathLabel(mlUbk1.MathText);
            row.Ubk2 = ParseFromMathLabel(mlUbk2.MathText);
            row.Ubk3 = ParseFromMathLabel(mlUbk3.MathText);
            row.Ubk4 = ParseFromMathLabel(mlUbk4.MathText);
            row.Ubk = ParseFromMathLabel(mlUbkResult.MathText);

            // U = ĐKĐB mở rộng — lấy từ mlUFinal, loại bỏ dấu ±
            row.DoKhongDamBao = ParseFromMathLabel(mlUFinal.MathText, removePlusMinus: true);

            return row;
        }

        /// <summary>
        /// Trích số thực cuối cùng từ chuỗi MathText (dạng "... = 1.2345 °C").
        /// </summary>
        private static double ParseFromMathLabel(string mathText, bool removePlusMinus = false)
        {
            if (string.IsNullOrEmpty(mathText)) return 0;

            // Loại bỏ ký tự ±
            if (removePlusMinus)
                mathText = mathText.Replace("±", "").Replace("_pm_", "");

            // Tìm "= " rồi lấy số tiếp theo
            int eqIdx = mathText.LastIndexOf('=');
            if (eqIdx < 0) return 0;

            string after = mathText.Substring(eqIdx + 1).Trim();

            // Lấy token đầu tiên (trước khoảng trắng hoặc °)
            string token = after.Split(new[] { ' ', '°', '\n' },
                StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";

            if (double.TryParse(token, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out double val))
                return val;

            return 0;
        }

        // ── Wire thêm sự kiện tính toán ───────────────────────────────────

        /// <summary>
        /// Thêm hook vào btnCalculate để bật nút "Thêm vào bảng" sau khi tính thành công.
        /// Gọi ở cuối WireEvents() trong UncertaintyCalculationForm.cs gốc.
        /// </summary>
        private void WireCalculationSuccessHook()
        {
            // Hook vào sau BtnCalculate_Click — dùng cách wrap
            btnCalculate.Click -= BtnCalculate_Click_Original;
            btnCalculate.Click += BtnCalculate_Click_Wrapped;
        }

        private void BtnCalculate_Click_Original(object? sender, EventArgs e)
            => BtnCalculate_Click(sender, e);

        private void BtnCalculate_Click_Wrapped(object? sender, EventArgs e)
        {
            BtnCalculate_Click(sender, e);
            // Nếu không throw → tính toán thành công
            OnCalculationSucceeded();
        }
    }
}