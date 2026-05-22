using HM_19MB_Demo.Data;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HM_19MB_Demo
{
    /// <summary>
    /// Form quản lý phiên đo: xem danh sách, mở lại phiên cũ, tạo phiên mới, xóa phiên.
    /// 
    /// Cách dùng:
    ///   using var form = new SessionManagerForm();
    ///   if (form.ShowDialog(this) == DialogResult.OK)
    ///   {
    ///       if (form.IsNewSession)
    ///           // tạo phiên mới, không load gì
    ///       else
    ///           // load phiên: form.SelectedSessionId
    ///   }
    /// </summary>
    public partial class SessionManagerForm : Form
    {
        // ── Kết quả ──────────────────────────────────────────────────────────
        public int? SelectedSessionId { get; private set; }
        public bool IsNewSession { get; private set; }

        private List<PhienDoSummary> _allSessions = new();

        public SessionManagerForm()
        {
            InitializeComponent();
        }

        private async void SessionManagerForm_Load(object? sender, EventArgs e)
        {
            await LoadSessionsAsync();
        }

        private void SessionManagerForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && _btnMoPhien.Enabled)
                BtnMoPhien_Click(sender, e);
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            FilterGrid(_txtSearch.Text);
        }

        // ── Load dữ liệu ────────────────────────────────────────────────────
        private async Task LoadSessionsAsync()
        {
            _lblLoading.Visible = true;
            _grid.Visible = false;
            _btnMoPhien.Enabled = false;
            _btnXoa.Enabled = false;

            try
            {
                await DatabaseService.EnsureSchemaAsync();
                _allSessions = await LayDanhSachPhienAsync();
                PopulateGrid(_allSessions);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách phiên:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _lblLoading.Visible = false;
                _grid.Visible = true;
            }
        }

        private void PopulateGrid(List<PhienDoSummary> sessions)
        {
            _grid.Rows.Clear();

            if (sessions.Count == 0)
            {
                _lblLoading.Text = "Chưa có phiên đo nào. Nhấn 'Tạo phiên mới' để bắt đầu.";
                _lblLoading.Visible = true;
                _grid.Visible = false;
                return;
            }

            _lblLoading.Visible = false;
            _grid.Visible = true;

            foreach (var s in sessions)
            {
                int idx = _grid.Rows.Add(
                    s.Id,
                    s.TenThietBi,
                    s.KyHieu,
                    s.SoHieu,
                    s.DonViSuDung,
                    s.NgayHieuChuan.ToString("dd/MM/yyyy"),
                    s.SoDiemKiemTra > 0 ? s.SoDiemKiemTra.ToString() : "—",
                    s.SoLanDoTho > 0 ? s.SoLanDoTho.ToString() : "—",
                    s.NgayTao.ToString("dd/MM/yyyy HH:mm")
                );

                // Màu nền khác nhau theo trạng thái hoàn thành
                if (s.SoDiemKiemTra > 0)
                {
                    _grid.Rows[idx].Cells["ColDiem"].Style.ForeColor = Color.DarkGreen;
                    _grid.Rows[idx].Cells["ColDiem"].Style.Font =
                        new Font(_grid.Font, FontStyle.Bold);
                }
            }
        }

        private void FilterGrid(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                PopulateGrid(_allSessions);
                return;
            }

            string kw = keyword.ToLower(CultureInfo.InvariantCulture);
            var filtered = _allSessions.FindAll(s =>
                s.TenThietBi.ToLower().Contains(kw) ||
                s.KyHieu.ToLower().Contains(kw) ||
                s.SoHieu.ToLower().Contains(kw) ||
                s.DonViSuDung.ToLower().Contains(kw) ||
                s.NgayHieuChuan.ToString("dd/MM/yyyy").Contains(kw));

            PopulateGrid(filtered);
        }

        // ── Sự kiện grid ────────────────────────────────────────────────────
        private void Grid_SelectionChanged(object? sender, EventArgs e)
        {
            bool hasSelection = _grid.SelectedRows.Count > 0;
            _btnMoPhien.Enabled = hasSelection;
            _btnXoa.Enabled = hasSelection;
        }

        private void Grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                BtnMoPhien_Click(sender, e);
        }

        // ── Nút Mở phiên ────────────────────────────────────────────────────
        private void BtnMoPhien_Click(object? sender, EventArgs e)
        {
            if (_grid.SelectedRows.Count == 0) return;

            var row = _grid.SelectedRows[0];
            if (row.Cells["ColId"].Value is int id)
            {
                SelectedSessionId = id;
                IsNewSession = false;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        // ── Nút Tạo phiên mới ────────────────────────────────────────────────
        private void BtnTaoMoi_Click(object? sender, EventArgs e)
        {
            SelectedSessionId = null;
            IsNewSession = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        // ── Nút Xóa phiên ────────────────────────────────────────────────────
        private async void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (_grid.SelectedRows.Count == 0) return;

            var row = _grid.SelectedRows[0];
            if (row.Cells["ColId"].Value is not int id) return;

            string tenThietBi = row.Cells["ColTen"].Value?.ToString() ?? "";
            string soHieu = row.Cells["ColSoHieu"].Value?.ToString() ?? "";
            int soDiem = int.TryParse(row.Cells["ColDiem"].Value?.ToString(), out int d) ? d : 0;

            string warning = soDiem > 0
                ? $"\n⚠️  Phiên này có {soDiem} điểm kiểm tra sẽ bị xóa vĩnh viễn!"
                : "";

            var confirm = MessageBox.Show(
                $"Xóa phiên đo:\n  Thiết bị: {tenThietBi}\n  Số hiệu: {soHieu}{warning}\n\nThao tác này không thể hoàn tác!",
                "Xác nhận xóa phiên",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;

            try
            {
                await XoaPhienAsync(id);
                await LoadSessionsAsync();
                ToastNotification.ShowSuccess("Đã xóa phiên đo.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa phiên:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Database helpers ─────────────────────────────────────────────────
        private static string ConnectionString =>
            ConfigurationManager.AppSettings["PostgresConnectionString"]
            ?? throw new InvalidOperationException("Thiếu PostgresConnectionString trong app.config.");

        public static async Task<List<PhienDoSummary>> LayDanhSachPhienAsync()
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("SELECT * FROM fn_lay_danh_sach_phien()", conn);
            var list = new List<PhienDoSummary>();

            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add(new PhienDoSummary
                {
                    Id = rdr.GetInt32(0),
                    TenThietBi = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                    KyHieu = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                    SoHieu = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                    DonViSuDung = rdr.IsDBNull(4) ? "" : rdr.GetString(4),
                    NgayHieuChuan = rdr.IsDBNull(5) ? DateTime.Today : rdr.GetDateTime(5),
                    SoDiemKiemTra = rdr.IsDBNull(6) ? 0 : Convert.ToInt32(rdr.GetValue(6)),
                    SoLanDoTho = rdr.IsDBNull(7) ? 0 : Convert.ToInt32(rdr.GetValue(7)),
                    NgayTao = rdr.IsDBNull(8) ? DateTime.Now : rdr.GetDateTime(8),
                });
            }

            return list;
        }

        private static async Task XoaPhienAsync(int phienId)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand("SELECT fn_xoa_phien(@id)", conn);
            cmd.Parameters.AddWithValue("@id", phienId);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
