using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace HM_19MB_Demo
{
    /// <summary>
    /// Tạo báo cáo hiệu chuẩn từ dữ liệu trong database
    /// </summary>
    public static class ReportGenerator
    {
        /// <summary>
        /// Xuất báo cáo Excel cho một session
        /// </summary>
        public static async Task<string> ExportToExcelAsync(int sessionId, string outputPath)
        {
            var data = await GetSessionDataAsync(sessionId);
            
            if (data.MeasurementRecords.Count == 0)
            {
                throw new InvalidOperationException("Không có dữ liệu đo để xuất báo cáo.");
            }

            // Tạo file CSV (có thể mở bằng Excel)
            using (var writer = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8))
            {
                // Header - Thông tin metadata
                await writer.WriteLineAsync("BIÊN BẢN HIỆU CHUẨN THIẾT BỊ");
                await writer.WriteLineAsync("");
                await writer.WriteLineAsync($"Tên phương tiện đo:,{data.Metadata.DeviceName}");
                await writer.WriteLineAsync($"Ký hiệu:,{data.Metadata.DeviceCode}");
                await writer.WriteLineAsync($"Số hiệu:,{data.Metadata.DeviceNumber}");
                await writer.WriteLineAsync($"Số tem hiệu chuẩn:,{data.Metadata.SealNumber}");
                await writer.WriteLineAsync($"Nơi sản xuất:,{data.Metadata.Manufacturer}");
                await writer.WriteLineAsync($"Năm sản xuất:,{data.Metadata.ManufactureYear}");
                await writer.WriteLineAsync($"Đơn vị sử dụng:,{data.Metadata.UsingUnit}");
                await writer.WriteLineAsync($"Phương pháp thực hiện:,{data.Metadata.Method}");
                await writer.WriteLineAsync($"Nhiệt độ môi trường:,{data.Metadata.EnvTemperature}");
                await writer.WriteLineAsync($"Độ ẩm tương đối:,{data.Metadata.EnvHumidity}");
                await writer.WriteLineAsync($"Đặc tính kỹ thuật:,{data.Metadata.TechnicalSpecs}");
                await writer.WriteLineAsync($"Các phương tiện đo sử dụng:,{data.Metadata.MeasuringDevices}");
                await writer.WriteLineAsync($"Ngày hiệu chuẩn:,{data.Metadata.CalibrationDay}/{data.Metadata.CalibrationMonth}/{data.Metadata.CalibrationYear}");
                await writer.WriteLineAsync("");
                await writer.WriteLineAsync("KẾT QUẢ ĐO");
                await writer.WriteLineAsync("");

                // Header bảng dữ liệu
                await writer.WriteLineAsync("Thời gian,Đầu đo,Nhiệt độ (°C),Độ ẩm (%),Trung bình nhiệt độ,Trung bình độ ẩm,Độ đồng đều nhiệt độ,Độ đồng đều độ ẩm,Độ ổn định");

                // Dữ liệu đo
                foreach (var record in data.MeasurementRecords)
                {
                    for (int i = 0; i < record.ProbeData.Count; i++)
                    {
                        var probe = record.ProbeData[i];
                        
                        // Chỉ ghi thông tin tổng hợp ở dòng đầu tiên
                        if (i == 0)
                        {
                            await writer.WriteLineAsync(
                                $"{record.ReceivedAt:yyyy-MM-dd HH:mm:ss}," +
                                $"{probe.ProbeNumber}," +
                                $"{probe.Temperature:F1}," +
                                $"{probe.Humidity:F1}," +
                                $"{record.AvgTemperature:F1}," +
                                $"{record.AvgHumidity:F1}," +
                                $"{record.UniformityTemp:F1}," +
                                $"{record.UniformityHumidity:F1}," +
                                $"{record.StabilityRaw}"
                            );
                        }
                        else
                        {
                            await writer.WriteLineAsync(
                                $"{record.ReceivedAt:yyyy-MM-dd HH:mm:ss}," +
                                $"{probe.ProbeNumber}," +
                                $"{probe.Temperature:F1}," +
                                $"{probe.Humidity:F1}," +
                                $",,,,,"
                            );
                        }
                    }
                }

                await writer.WriteLineAsync("");
                await writer.WriteLineAsync("THỐNG KÊ TỔNG HỢP");
                await writer.WriteLineAsync("");

                // Thống kê theo từng đầu đo
                await writer.WriteLineAsync("Đầu đo,Nhiệt độ TB,Nhiệt độ Min,Nhiệt độ Max,Độ ẩm TB,Độ ẩm Min,Độ ẩm Max,Số lần đo");
                
                var stats = CalculateProbeStatistics(data.MeasurementRecords);
                foreach (var stat in stats)
                {
                    await writer.WriteLineAsync(
                        $"{stat.ProbeNumber}," +
                        $"{stat.AvgTemperature:F2}," +
                        $"{stat.MinTemperature:F2}," +
                        $"{stat.MaxTemperature:F2}," +
                        $"{stat.AvgHumidity:F2}," +
                        $"{stat.MinHumidity:F2}," +
                        $"{stat.MaxHumidity:F2}," +
                        $"{stat.Count}"
                    );
                }

                await writer.WriteLineAsync("");
                await writer.WriteLineAsync($"Tổng số lần đo:,{data.MeasurementRecords.Count}");
                await writer.WriteLineAsync($"Thời gian bắt đầu:,{data.MeasurementRecords.First().ReceivedAt:yyyy-MM-dd HH:mm:ss}");
                await writer.WriteLineAsync($"Thời gian kết thúc:,{data.MeasurementRecords.Last().ReceivedAt:yyyy-MM-dd HH:mm:ss}");
            }

            return outputPath;
        }

        /// <summary>
        /// Lấy dữ liệu của một session từ database
        /// </summary>
        public static async Task<SessionReportData> GetSessionDataAsync(int sessionId)
        {
            var connectionString = System.Configuration.ConfigurationManager.AppSettings["PostgresConnectionString"]
                ?? throw new InvalidOperationException("PostgresConnectionString not found.");

            var data = new SessionReportData();

            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            // Lấy metadata
            string sqlMeta = @"
                SELECT device_name, device_code, device_number, seal_number, manufacturer, 
                       manufacture_year, using_unit, method, env_temperature, env_humidity, 
                       technical_specs, measuring_devices, calibration_day, calibration_month, calibration_year
                FROM calibration_sessions 
                WHERE id = @sid";

            await using (var cmd = new NpgsqlCommand(sqlMeta, conn))
            {
                cmd.Parameters.AddWithValue("@sid", sessionId);
                await using var reader = await cmd.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    data.Metadata = new SessionMetadata
                    {
                        DeviceName = reader.GetString(0),
                        DeviceCode = reader.GetString(1),
                        DeviceNumber = reader.GetString(2),
                        SealNumber = reader.GetString(3),
                        Manufacturer = reader.GetString(4),
                        ManufactureYear = reader.GetString(5),
                        UsingUnit = reader.GetString(6),
                        Method = reader.GetString(7),
                        EnvTemperature = reader.GetString(8),
                        EnvHumidity = reader.GetString(9),
                        TechnicalSpecs = reader.GetString(10),
                        MeasuringDevices = reader.GetString(11),
                        CalibrationDay = reader.GetInt32(12),
                        CalibrationMonth = reader.GetInt32(13),
                        CalibrationYear = reader.GetInt32(14)
                    };
                }
            }

            // Lấy measurement records và probe data
            string sqlRecords = @"
                SELECT mr.id, mr.received_at, mr.avg_temperature, mr.avg_humidity, 
                       mr.uniformity_temp, mr.uniformity_humidity, mr.stability_raw,
                       pm.probe_number, pm.temperature, pm.humidity
                FROM measurement_records mr
                JOIN probe_measurements pm ON mr.id = pm.measurement_record_id
                WHERE mr.session_id = @sid
                ORDER BY mr.received_at, pm.probe_number";

            await using (var cmd = new NpgsqlCommand(sqlRecords, conn))
            {
                cmd.Parameters.AddWithValue("@sid", sessionId);
                await using var reader = await cmd.ExecuteReaderAsync();

                MeasurementRecordData? currentRecord = null;
                int lastRecordId = -1;

                while (await reader.ReadAsync())
                {
                    int recordId = reader.GetInt32(0);

                    if (recordId != lastRecordId)
                    {
                        if (currentRecord != null)
                        {
                            data.MeasurementRecords.Add(currentRecord);
                        }

                        currentRecord = new MeasurementRecordData
                        {
                            RecordId = recordId,
                            ReceivedAt = reader.GetDateTime(1),
                            AvgTemperature = reader.GetFloat(2),
                            AvgHumidity = reader.GetFloat(3),
                            UniformityTemp = reader.GetFloat(4),
                            UniformityHumidity = reader.GetFloat(5),
                            StabilityRaw = reader.GetString(6),
                            ProbeData = new List<ProbeData>()
                        };

                        lastRecordId = recordId;
                    }

                    currentRecord?.ProbeData.Add(new ProbeData
                    {
                        ProbeNumber = reader.GetInt32(7),
                        Temperature = reader.GetFloat(8),
                        Humidity = reader.GetFloat(9)
                    });
                }

                if (currentRecord != null)
                {
                    data.MeasurementRecords.Add(currentRecord);
                }
            }

            return data;
        }

        /// <summary>
        /// Tính thống kê cho từng đầu đo
        /// </summary>
        public static List<ProbeStatistics> CalculateProbeStatistics(List<MeasurementRecordData> records)
        {
            var stats = new List<ProbeStatistics>();

            for (int probeNum = 1; probeNum <= 10; probeNum++)
            {
                var probeData = records
                    .SelectMany(r => r.ProbeData)
                    .Where(p => p.ProbeNumber == probeNum)
                    .ToList();

                if (probeData.Count > 0)
                {
                    stats.Add(new ProbeStatistics
                    {
                        ProbeNumber = probeNum,
                        AvgTemperature = probeData.Average(p => p.Temperature),
                        MinTemperature = probeData.Min(p => p.Temperature),
                        MaxTemperature = probeData.Max(p => p.Temperature),
                        AvgHumidity = probeData.Average(p => p.Humidity),
                        MinHumidity = probeData.Min(p => p.Humidity),
                        MaxHumidity = probeData.Max(p => p.Humidity),
                        Count = probeData.Count
                    });
                }
            }

            return stats;
        }
    }

    // ── Data classes ─────────────────────────────────────────────────────────

    public class SessionReportData
    {
        public SessionMetadata Metadata { get; set; } = new SessionMetadata();
        public List<MeasurementRecordData> MeasurementRecords { get; set; } = new List<MeasurementRecordData>();
    }

    public class MeasurementRecordData
    {
        public int RecordId { get; set; }
        public DateTime ReceivedAt { get; set; }
        public float AvgTemperature { get; set; }
        public float AvgHumidity { get; set; }
        public float UniformityTemp { get; set; }
        public float UniformityHumidity { get; set; }
        public string StabilityRaw { get; set; } = "";
        public List<ProbeData> ProbeData { get; set; } = new List<ProbeData>();
    }

    public class ProbeData
    {
        public int ProbeNumber { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
    }

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
