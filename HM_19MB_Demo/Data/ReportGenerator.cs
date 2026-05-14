using HM_19MB_Demo.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HM_19MB_Demo
{
    // Tạo báo cáo hiệu chuẩn từ dữ liệu trong database.
    public static class ReportGenerator
    {

        public static async Task<string> ExportToExcelAsync(int phienId, string outputPath)
        {
            var meta = await DatabaseService.LayPhienAsync(phienId)
                          ?? throw new InvalidOperationException("Không tìm thấy phiên hiệu chuẩn.");
            var ketQua = await DatabaseService.LayKetQuaTheoPhienAsync(phienId);

            if (ketQua.Count == 0)
                throw new InvalidOperationException("Không có dữ liệu đo để xuất báo cáo.");

            using var writer = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8);

            await GhiHeaderMetadata(writer, meta);

            await writer.WriteLineAsync("KẾT QUẢ ĐO");
            await writer.WriteLineAsync("");
            await writer.WriteLineAsync(
                "Thời gian,Đầu đo,Nhiệt độ (°C),Độ ẩm (%)," +
                "Trung bình nhiệt độ,Trung bình độ ẩm," +
                "Độ đồng đều nhiệt độ,Độ đồng đều độ ẩm,Độ ổn định");

            foreach (var kq in ketQua)
            {
                for (int i = 0; i < kq.SoLieuDauDo.Count; i++)
                {
                    var dd = kq.SoLieuDauDo[i];
                    if (i == 0)
                        await writer.WriteLineAsync(
                            $"{kq.ThoiGianDo:yyyy-MM-dd HH:mm:ss}," +
                            $"{dd.SoDauDo},{dd.NhietDo:F1},{dd.DoAm:F1}," +
                            $"{kq.NhietDoTb:F1},{kq.DoAmTb:F1}," +
                            $"{kq.DoDongDeuNhiet:F1},{kq.DoDongDeuAm:F1},{kq.DoOnDinhRaw}");
                    else
                        await writer.WriteLineAsync(
                            $"{kq.ThoiGianDo:yyyy-MM-dd HH:mm:ss}," +
                            $"{dd.SoDauDo},{dd.NhietDo:F1},{dd.DoAm:F1},,,,,");
                }
            }

            // Thống kê — LINQ giữ nguyên vì dữ liệu đã có trong bộ nhớ
            await writer.WriteLineAsync("");
            await writer.WriteLineAsync("THỐNG KÊ TỔNG HỢP");
            await writer.WriteLineAsync("");
            await writer.WriteLineAsync(
                "Đầu đo,Nhiệt độ TB,Nhiệt độ Min,Nhiệt độ Max," +
                "Độ ẩm TB,Độ ẩm Min,Độ ẩm Max,Số lần đo");

            foreach (var stat in TinhThongKeDauDo(ketQua))
            {
                await writer.WriteLineAsync(
                    $"{stat.SoDauDo}," +
                    $"{stat.NhietDoTb:F2},{stat.NhietDoMin:F2},{stat.NhietDoMax:F2}," +
                    $"{stat.DoAmTb:F2},{stat.DoAmMin:F2},{stat.DoAmMax:F2},{stat.SoLanDo}");
            }

            await writer.WriteLineAsync("");
            await writer.WriteLineAsync($"Tổng số lần đo:,{ketQua.Count}");
            await writer.WriteLineAsync(
                $"Thời gian bắt đầu:,{ketQua.First().ThoiGianDo:yyyy-MM-dd HH:mm:ss}");
            await writer.WriteLineAsync(
                $"Thời gian kết thúc:,{ketQua.Last().ThoiGianDo:yyyy-MM-dd HH:mm:ss}");

            return outputPath;
        }

        // ── Thống kê — LINQ trong C# ──────────────────────────────────────────

        /// Tính AVG/MIN/MAX cho từng đầu đo từ danh sách kết quả đã load.
        public static List<ThongKeDauDo> TinhThongKeDauDo(List<KetQuaDo> danhSach)
        {
            return danhSach
                .SelectMany(kq => kq.SoLieuDauDo)
                .GroupBy(dd => dd.SoDauDo)
                .OrderBy(g => g.Key)
                .Select(g => new ThongKeDauDo
                {
                    SoDauDo = g.Key,
                    NhietDoTb = g.Average(d => d.NhietDo),
                    NhietDoMin = g.Min(d => d.NhietDo),
                    NhietDoMax = g.Max(d => d.NhietDo),
                    DoAmTb = g.Average(d => d.DoAm),
                    DoAmMin = g.Min(d => d.DoAm),
                    DoAmMax = g.Max(d => d.DoAm),
                    SoLanDo = g.Count(),
                })
                .ToList();
        }

        private static async Task GhiHeaderMetadata(StreamWriter writer, SessionMetadata m)
        {
            await writer.WriteLineAsync("BIÊN BẢN HIỆU CHUẨN THIẾT BỊ");
            await writer.WriteLineAsync("");
            await writer.WriteLineAsync($"Tên phương tiện đo:,{m.TenThietBi}");
            await writer.WriteLineAsync($"Ký hiệu:,{m.KyHieu}");
            await writer.WriteLineAsync($"Số hiệu:,{m.SoHieu}");
            await writer.WriteLineAsync($"Số tem hiệu chuẩn:,{m.SoTem}");
            await writer.WriteLineAsync($"Nơi sản xuất:,{m.NoiSanXuat}");
            await writer.WriteLineAsync($"Năm sản xuất:,{m.NamSanXuat}");
            await writer.WriteLineAsync($"Đơn vị sử dụng:,{m.DonViSuDung}");
            await writer.WriteLineAsync($"Phương pháp thực hiện:,{m.PhuongPhap}");
            await writer.WriteLineAsync($"Ngày hiệu chuẩn:,{m.NgayHieuChuan:dd/MM/yyyy}");
            await writer.WriteLineAsync($"Nhiệt độ môi trường:,{m.NhietDoMoiTruong}");
            await writer.WriteLineAsync($"Độ ẩm tương đối:,{m.DoAmTuongDoi}");
            await writer.WriteLineAsync($"Đặc tính kỹ thuật:,{m.DacTinhKyThuat}");
            await writer.WriteLineAsync($"Các phương tiện đo sử dụng:,{m.ThietBiChuan}");
            await writer.WriteLineAsync("");
        }

        [Obsolete("Dùng LayPhienAsync + LayKetQuaTheoPhienAsync thay thế.")]
        public static async Task<SessionReportData> GetSessionDataAsync(int sessionId)
        {
            var meta = await DatabaseService.LayPhienAsync(sessionId) ?? new SessionMetadata();
            var ketQua = await DatabaseService.LayKetQuaTheoPhienAsync(sessionId);

            return new SessionReportData
            {
                Metadata = meta,
                MeasurementRecords = ketQua.Select(kq => new MeasurementRecordData
                {
                    RecordId = kq.Id,
                    ReceivedAt = kq.ThoiGianDo,
                    AvgTemperature = kq.NhietDoTb,
                    AvgHumidity = kq.DoAmTb,
                    UniformityTemp = kq.DoDongDeuNhiet,
                    UniformityHumidity = kq.DoDongDeuAm,
                    StabilityRaw = kq.DoOnDinhRaw,
                    ProbeData = kq.SoLieuDauDo.Select(dd => new ProbeData
                    {
                        ProbeNumber = dd.SoDauDo,
                        Temperature = dd.NhietDo,
                        Humidity = dd.DoAm,
                    }).ToList(),
                }).ToList(),
            };
        }

        [Obsolete("Dùng TinhThongKeDauDo(List<KetQuaDo>) thay thế.")]
        public static List<ProbeStatistics> CalculateProbeStatistics(
            List<MeasurementRecordData> records)
        {
            var mapped = records.Select(r => new KetQuaDo
            {
                SoLieuDauDo = r.ProbeData.Select(p => new SoLieuDauDo
                {
                    SoDauDo = p.ProbeNumber,
                    NhietDo = p.Temperature,
                    DoAm = p.Humidity,
                }).ToList(),
            }).ToList();

            return TinhThongKeDauDo(mapped).Select(s => new ProbeStatistics
            {
                ProbeNumber = s.SoDauDo,
                AvgTemperature = s.NhietDoTb,
                MinTemperature = s.NhietDoMin,
                MaxTemperature = s.NhietDoMax,
                AvgHumidity = s.DoAmTb,
                MinHumidity = s.DoAmMin,
                MaxHumidity = s.DoAmMax,
                Count = s.SoLanDo,
            }).ToList();
        }
    }


    public class DuLieuBaoCao
    {
        public SessionMetadata Metadata { get; set; } = new();
        public List<KetQuaDo> DanhSachKetQua { get; set; } = new();
    }

    public class KetQuaDo
    {
        public int Id { get; set; }
        public DateTime ThoiGianDo { get; set; }
        public float NhietDoTb { get; set; }
        public float DoAmTb { get; set; }
        public float DoDongDeuNhiet { get; set; }
        public float DoDongDeuAm { get; set; }
        public string DoOnDinhRaw { get; set; } = "";
        public List<SoLieuDauDo> SoLieuDauDo { get; set; } = new();
    }

    public class SoLieuDauDo
    {
        public int SoDauDo { get; set; }
        public float NhietDo { get; set; }
        public float DoAm { get; set; }
    }

    public class ThongKeDauDo
    {
        public int SoDauDo { get; set; }
        public double NhietDoTb { get; set; }
        public double NhietDoMin { get; set; }
        public double NhietDoMax { get; set; }
        public double DoAmTb { get; set; }
        public double DoAmMin { get; set; }
        public double DoAmMax { get; set; }
        public int SoLanDo { get; set; }
    }


    [Obsolete("Dùng DuLieuBaoCao thay thế.")]
    public class SessionReportData
    {
        public SessionMetadata Metadata { get; set; } = new();
        public List<MeasurementRecordData> MeasurementRecords { get; set; } = new();
    }

    [Obsolete("Dùng KetQuaDo thay thế.")]
    public class MeasurementRecordData
    {
        public int RecordId { get; set; }
        public DateTime ReceivedAt { get; set; }
        public float AvgTemperature { get; set; }
        public float AvgHumidity { get; set; }
        public float UniformityTemp { get; set; }
        public float UniformityHumidity { get; set; }
        public string StabilityRaw { get; set; } = "";
        public List<ProbeData> ProbeData { get; set; } = new();
    }

    [Obsolete("Dùng SoLieuDauDo thay thế.")]
    public class ProbeData
    {
        public int ProbeNumber { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
    }

    [Obsolete("Dùng ThongKeDauDo thay thế.")]
    public class ProbeStatistics
    {
        public int ProbeNumber { get; set; }
        public double AvgTemperature { get; set; }
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public double AvgHumidity { get; set; }
        public double MinHumidity { get; set; }
        public double MaxHumidity { get; set; }
        public int Count { get; set; }
    }
}