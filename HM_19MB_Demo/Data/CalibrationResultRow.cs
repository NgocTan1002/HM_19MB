using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace HM_19MB_Demo.Data
{
    public class CalibrationResultRow
    {
        public int Id { get; set; }
        public int STT { get; set; }

        public double GiaTriDat { get; set; }
        public double GiaTriChiThi { get; set; }

        public double[] ViTri { get; set; } = new double[9];

        public double GiaTriTrungBinh { get; set; }
        public double SoHieuChinh { get; set; }
        public double DoOnDinh { get; set; }
        public double DoDongDeu { get; set; }
        public double DoKhongDamBao { get; set; }

        public double Uch1 { get; set; }
        public double Uch2 { get; set; }
        public double Uch { get; set; }
        public double Ubk1 { get; set; }
        public double Ubk2 { get; set; }
        public double Ubk3 { get; set; }
        public double Ubk4 { get; set; }
        public double Ubk { get; set; }

        public int SoKenh { get; set; }
        public int SoLanDo { get; set; }
        public string PhuongPhapB { get; set; } = "U";

        // Số vị trí thực sự có trong dữ liệu
        public int SoViTriHopLe 
        {
            get
            {
                int count = 0;
                foreach (var v in ViTri)
                {
                    if (!double.IsNaN(v)) count++;
                }
                return count;
            }
        }

        public CalibrationResultRow()
        {
            // Khởi tạo ViTri với NaN
            for (int i = 0; i < ViTri.Length; i++)
            {
                ViTri[i] = double.NaN;
            }
        }
    }

    public static partial class DatabaseService
    {
        public static async Task<int> LaySTTTiepTheoAsync(int phienId)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT fn_lay_stt_tiep_theo(@p_phien_id)", conn);
            cmd.Parameters.AddWithValue("@p_phien_id", phienId);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public static async Task<int> LuuKetQuaHieuChuanAsync(
            int phienId,
            CalibrationResultRow row)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT fn_luu_ket_qua_hieu_chuan(" +
                "@phien_id, @stt," +
                "@gia_tri_dat, @gia_tri_chi_thi," +
                "@vi_tri_1, @vi_tri_2, @vi_tri_3, @vi_tri_4, @vi_tri_5," +
                "@vi_tri_6, @vi_tri_7, @vi_tri_8, @vi_tri_9," +
                "@gia_tri_trung_binh, @so_hieu_chinh," +
                "@do_on_dinh, @do_dong_deu, @do_khong_dam_bao," +
                "@uch1, @uch2, @uch," +
                "@ubk1, @ubk2, @ubk3, @ubk4, @ubk," +
                "@so_kenh, @so_lan_do, @phuong_phap_b)", conn);

            cmd.Parameters.AddWithValue("@phien_id", phienId);
            cmd.Parameters.AddWithValue("@stt", row.STT);
            cmd.Parameters.AddWithValue("@gia_tri_dat", row.GiaTriDat);
            cmd.Parameters.AddWithValue("@gia_tri_chi_thi", row.GiaTriChiThi);

            // 9 vị trí — NaN → DBNull
            for (int i = 0; i < 9; i++)
            {
                object val = double.IsNaN(row.ViTri[i])
                    ? DBNull.Value
                    : (object)row.ViTri[i];
                cmd.Parameters.AddWithValue($"@vi_tri_{i + 1}", val);
            }

            cmd.Parameters.AddWithValue("@gia_tri_trung_binh", row.GiaTriTrungBinh);
            cmd.Parameters.AddWithValue("@so_hieu_chinh", row.SoHieuChinh);
            cmd.Parameters.AddWithValue("@do_on_dinh", row.DoOnDinh);
            cmd.Parameters.AddWithValue("@do_dong_deu", row.DoDongDeu);
            cmd.Parameters.AddWithValue("@do_khong_dam_bao", row.DoKhongDamBao);

            cmd.Parameters.AddWithValue("@uch1", row.Uch1);
            cmd.Parameters.AddWithValue("@uch2", row.Uch2);
            cmd.Parameters.AddWithValue("@uch", row.Uch);
            cmd.Parameters.AddWithValue("@ubk1", row.Ubk1);
            cmd.Parameters.AddWithValue("@ubk2", row.Ubk2);
            cmd.Parameters.AddWithValue("@ubk3", row.Ubk3);
            cmd.Parameters.AddWithValue("@ubk4", row.Ubk4);
            cmd.Parameters.AddWithValue("@ubk", row.Ubk);

            cmd.Parameters.AddWithValue("@so_kenh", row.SoKenh);
            cmd.Parameters.AddWithValue("@so_lan_do", row.SoLanDo);
            cmd.Parameters.AddWithValue("@phuong_phap_b", row.PhuongPhapB);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public static async Task<List<CalibrationResultRow>> LayKetQuaHieuChuanAsync(
    int phienId)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT * FROM fn_lay_ket_qua_hieu_chuan(@p_phien_id)", conn);
            cmd.Parameters.AddWithValue("@p_phien_id", phienId);

            var list = new List<CalibrationResultRow>();
            await using var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync())
            {
                var row = new CalibrationResultRow
                {
                    Id = rdr.GetInt32(0),   // id
                    STT = rdr.GetInt32(1),   // stt
                    GiaTriDat = ReadDouble(rdr, 2), // gia_tri_dat
                    GiaTriChiThi = ReadDouble(rdr, 3), // gia_tri_chi_thi
                    // vi_tri_1..9: cột 4..12
                    GiaTriTrungBinh = ReadDouble(rdr, 13),
                    SoHieuChinh = ReadDouble(rdr, 14),
                    DoOnDinh = ReadDouble(rdr, 15),
                    DoDongDeu = ReadDouble(rdr, 16),
                    DoKhongDamBao = ReadDouble(rdr, 17),
                    Uch1 = ReadDoubleNullable(rdr, 18),
                    Uch2 = ReadDoubleNullable(rdr, 19),
                    Uch = ReadDoubleNullable(rdr, 20),
                    Ubk1 = ReadDoubleNullable(rdr, 21),
                    Ubk2 = ReadDoubleNullable(rdr, 22),
                    Ubk3 = ReadDoubleNullable(rdr, 23),
                    Ubk4 = ReadDoubleNullable(rdr, 24),
                    Ubk = ReadDoubleNullable(rdr, 25),
                    SoKenh = rdr.IsDBNull(26) ? 0 : rdr.GetInt32(26),
                    SoLanDo = rdr.IsDBNull(27) ? 0 : rdr.GetInt32(27),
                    PhuongPhapB = rdr.IsDBNull(28) ? "U" : rdr.GetString(28),
                };

                // Đọc 9 vị trí (cột 4..12)
                for (int i = 0; i < 9; i++)
                    row.ViTri[i] = ReadDoubleNullable(rdr, 4 + i);

                list.Add(row);
            }

            return list;
        }

        public static async Task XoaKetQuaHieuChuanAsync(int phienId, int stt)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT fn_xoa_ket_qua_hieu_chuan(@p_phien_id, @p_stt)", conn);
            cmd.Parameters.AddWithValue("@p_phien_id", phienId);
            cmd.Parameters.AddWithValue("@p_stt", stt);
            await cmd.ExecuteNonQueryAsync();
        }
        private static double ReadDouble(NpgsqlDataReader rdr, int ordinal)
            => rdr.IsDBNull(ordinal) ? 0.0 : Convert.ToDouble(rdr.GetValue(ordinal));

        private static double ReadDoubleNullable(NpgsqlDataReader rdr, int ordinal)
            => rdr.IsDBNull(ordinal) ? double.NaN : Convert.ToDouble(rdr.GetValue(ordinal));
    }
}
