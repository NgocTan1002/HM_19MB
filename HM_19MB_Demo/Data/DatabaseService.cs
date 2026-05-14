using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.Json;
using System.Threading.Tasks;
using HM_19MB_Demo.Data;
using Npgsql;

namespace HM_19MB_Demo.Data
{
    public class SessionMetadata
    {
        public string TenThietBi { get; set; } = "";
        public string KyHieu { get; set; } = "";
        public string SoHieu { get; set; } = "";
        public string SoTem { get; set; } = "";
        public string NoiSanXuat { get; set; } = "";
        public string NamSanXuat { get; set; } = "";
        public string DonViSuDung { get; set; } = "";
        public string PhuongPhap { get; set; } = "";
        public DateTime NgayHieuChuan { get; set; } = DateTime.Today;
        public string NhietDoMoiTruong { get; set; } = "";
        public string DoAmTuongDoi { get; set; } = "";
        public string DacTinhKyThuat { get; set; } = "";
        public string ThietBiChuan { get; set; } = "";
    }
    public static class DatabaseService
    {
        private static string ConnectionString =>
            ConfigurationManager.AppSettings["PostgresConnectionString"]
            ?? throw new InvalidOperationException(
                "Thiếu 'PostgresConnectionString' trong app.config.");

        // Tạo bảng và đăng ký function nếu chưa tồn tại
        private const int SCHEMA_VERSION = 2;
        public static async Task EnsureSchemaAsync()
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            // Tạo bảng lưu trữ phiên bản schema
            await using (var cmd = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS cai_dat_he_thong (
                    khoa        VARCHAR(50)  PRIMARY KEY,
                    gia_tri     TEXT         NOT NULL,
                    cap_nhat    TIMESTAMP    NOT NULL DEFAULT NOW()
                )", conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            // Đọc phiên bản hiện tại
            int dbVersion = 0;
            await using (var cmd = new NpgsqlCommand(@"
                SELECT gia_tri FROM cai_dat_he_thong
                WHERE khoa = 'phien_ban_schema'", conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                if (result is string s) int.TryParse(s, out dbVersion);
            }
            if (dbVersion != SCHEMA_VERSION)
            {
                await using var cmd = new NpgsqlCommand(SqlLoader.Load("schema"), conn);
                await cmd.ExecuteNonQueryAsync();
            }

            await using (var cmd = new NpgsqlCommand(SqlLoader.Load("functions"), conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // Ghi lại phiên bản mới
            await using (var cmd = new NpgsqlCommand(@"
                INSERT INTO cai_dat_he_thong (khoa, gia_tri, cap_nhat)
                VALUES ('phien_ban_schema', @v, NOW())
                ON CONFLICT (khoa) DO UPDATE
                    SET gia_tri  = EXCLUDED.gia_tri,
                        cap_nhat = EXCLUDED.cap_nhat", conn))
            {
                cmd.Parameters.AddWithValue("@v", SCHEMA_VERSION.ToString());
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // Tạo phiên hiệu chuẩn mới
        public static async Task<int> TaoPhienMoiAsync(SessionMetadata meta)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT fn_tao_phien(" +
                "@p_ten_thiet_bi::varchar, @p_ky_hieu::varchar, @p_so_hieu::varchar, @p_so_tem::varchar," +
                "@p_noi_san_xuat::varchar, @p_nam_san_xuat::varchar, @p_don_vi_su_dung::varchar, @p_phuong_phap::varchar," +
                "@p_ngay_hieu_chuan::date," +
                "@p_nhiet_do_moi_truong::varchar, @p_do_am_tuong_doi::varchar," +
                "@p_dac_tinh_ky_thuat::text, @p_thiet_bi_chuan::text)", conn);

            cmd.Parameters.AddWithValue("@p_ten_thiet_bi", meta.TenThietBi);
            cmd.Parameters.AddWithValue("@p_ky_hieu", meta.KyHieu);
            cmd.Parameters.AddWithValue("@p_so_hieu", meta.SoHieu);
            cmd.Parameters.AddWithValue("@p_so_tem", meta.SoTem);
            cmd.Parameters.AddWithValue("@p_noi_san_xuat", meta.NoiSanXuat);
            cmd.Parameters.AddWithValue("@p_nam_san_xuat", meta.NamSanXuat);
            cmd.Parameters.AddWithValue("@p_don_vi_su_dung", meta.DonViSuDung);
            cmd.Parameters.AddWithValue("@p_phuong_phap", meta.PhuongPhap);
            cmd.Parameters.AddWithValue("@p_ngay_hieu_chuan", meta.NgayHieuChuan);
            cmd.Parameters.AddWithValue("@p_nhiet_do_moi_truong", meta.NhietDoMoiTruong);
            cmd.Parameters.AddWithValue("@p_do_am_tuong_doi", meta.DoAmTuongDoi);
            cmd.Parameters.AddWithValue("@p_dac_tinh_ky_thuat", meta.DacTinhKyThuat);
            cmd.Parameters.AddWithValue("@p_thiet_bi_chuan", meta.ThietBiChuan);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        // Lưu 1 block đo
        public static async Task<int> LuuKetQuaDoAsync(int phienId, MeasurementBlock block)
        {
            string soLieuJson = BuildSoLieuJson(block);

            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT fn_luu_ket_qua_do(" +
                "@p_phien_id, @p_thoi_gian_do," +
                "@p_nhiet_do_tb, @p_do_am_tb," +
                "@p_do_dong_deu_nhiet, @p_do_dong_deu_am," +
                "@p_do_on_dinh_raw, @p_so_lieu_json::jsonb)", conn);

            cmd.Parameters.AddWithValue("@p_phien_id", phienId);
            cmd.Parameters.AddWithValue("@p_thoi_gian_do", block.Timestamp);
            cmd.Parameters.AddWithValue("@p_nhiet_do_tb", block.AvgTemperature);
            cmd.Parameters.AddWithValue("@p_do_am_tb", block.AvgHumidity);
            cmd.Parameters.AddWithValue("@p_do_dong_deu_nhiet", block.UniformityTemp);
            cmd.Parameters.AddWithValue("@p_do_dong_deu_am", block.UniformityHumidity);
            cmd.Parameters.AddWithValue("@p_do_on_dinh_raw", block.StabilityRaw);
            cmd.Parameters.AddWithValue("@p_so_lieu_json", soLieuJson);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        // ĐỌC DỮ LIỆU

        // Lấy metadata phiên hiệu chuẩn
        public static async Task<SessionMetadata?> LayPhienAsync(int phienId)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT * FROM fn_lay_phien(@p_phien_id)", conn);
            cmd.Parameters.AddWithValue("@p_phien_id", phienId);

            await using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync()) return null;

            return new SessionMetadata
            {
                TenThietBi = rdr.GetString(0),
                KyHieu = rdr.GetString(1),
                SoHieu = rdr.GetString(2),
                SoTem = rdr.GetString(3),
                NoiSanXuat = rdr.GetString(4),
                NamSanXuat = rdr.GetString(5),
                DonViSuDung = rdr.GetString(6),
                PhuongPhap = rdr.GetString(7),
                NgayHieuChuan = rdr.GetDateTime(8),
                NhietDoMoiTruong = rdr.GetString(9),
                DoAmTuongDoi = rdr.GetString(10),
                DacTinhKyThuat = rdr.GetString(11),
                ThietBiChuan = rdr.GetString(12),
            };
        }

        // Lấy kết quả đo của 1 phiên
        public static async Task<List<KetQuaDo>> LayKetQuaTheoPhienAsync(int phienId)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT * FROM fn_lay_ket_qua_theo_phien(@p_phien_id)", conn);
            cmd.Parameters.AddWithValue("@p_phien_id", phienId);

            var danhSach = new List<KetQuaDo>();
            KetQuaDo? current = null;
            int lastId = -1;

            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                int id = rdr.GetInt32(0);
                if (id != lastId)
                {
                    if (current != null) danhSach.Add(current);
                    current = new KetQuaDo
                    {
                        Id = id,
                        ThoiGianDo = rdr.GetDateTime(1),
                        NhietDoTb = ReadSingle(rdr, 2),
                        DoAmTb = ReadSingle(rdr, 3),
                        DoDongDeuNhiet = ReadSingle(rdr, 4),
                        DoDongDeuAm = ReadSingle(rdr, 5),
                        DoOnDinhRaw = rdr.GetString(6),
                    };
                    lastId = id;
                }

                current!.SoLieuDauDo.Add(new SoLieuDauDo
                {
                    SoDauDo = rdr.GetInt16(7),
                    NhietDo = ReadSingle(rdr, 8),
                    DoAm = ReadSingle(rdr, 9),
                });
            }
            if (current != null) danhSach.Add(current);

            return danhSach;
        }

        private static float ReadSingle(NpgsqlDataReader reader, int ordinal)
            => Convert.ToSingle(reader.GetValue(ordinal));

        // Chuyển dữ liệu đầu đo trong MeasurementBlock thành chuỗi JSON
        private static string BuildSoLieuJson(MeasurementBlock block)
        {
            var items = new List<object>();
            for (int i = 0; i < block.ProbeCount && i < 10; i++)
            {
                if (float.IsNaN(block.ProbeTemperatures[i]))
                    continue;

                items.Add(new
                {
                    so_dau_do = i + 1,
                    nhiet_do = Math.Round(block.ProbeTemperatures[i], 2),
                    do_am = float.IsNaN(block.ProbeHumidities[i])
                    ? (double?)null
                    : Math.Round(block.ProbeHumidities[i], 2),
                });
            }
            return JsonSerializer.Serialize(items);
        }

        [Obsolete("Dùng TaoPhienMoiAsync thay thế.")]
        public static Task<int> InsertSessionAsync(SessionMetadata meta)
            => TaoPhienMoiAsync(meta);

        [Obsolete("Dùng LuuKetQuaDoAsync thay thế.")]
        public static Task<int> InsertMeasurementRecordAsync(int sessionId, MeasurementBlock block)
            => LuuKetQuaDoAsync(sessionId, block);
    }
}
