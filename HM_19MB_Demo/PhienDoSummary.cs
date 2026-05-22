using System;

namespace HM_19MB_Demo
{
    /// <summary>
    /// Thông tin tóm tắt một phiên đo (dùng để hiển thị danh sách).
    /// </summary>
    public class PhienDoSummary
    {
        public int Id { get; set; }
        public string TenThietBi { get; set; } = "";
        public string KyHieu { get; set; } = "";
        public string SoHieu { get; set; } = "";
        public string DonViSuDung { get; set; } = "";
        public DateTime NgayHieuChuan { get; set; }
        public int SoDiemKiemTra { get; set; }
        public int SoLanDoTho { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
