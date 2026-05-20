using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using HM_19MB_Demo.Data;

namespace HM_19MB_Demo
{
    internal static class ExcelExporter
    {
        public static async Task ExportWordAsync(
            SessionMetadata meta,
            List<CalibrationResultRow> calibRows,
            IWin32Window owner)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "Word Documents (*.docx)|*.docx",
                FileName = $"GiayChungNhan_{meta.SoHieu}_{meta.NgayHieuChuan:yyyyMMdd}.docx"
            };

            if (dialog.ShowDialog(owner) != DialogResult.OK)
                return;

            string templatePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources", "Templates", "GiayChungNhanHieuChuan.docx");

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Không tìm thấy template: {templatePath}");

            var values = new Dictionary<string, object>
            {
                ["TenThietBi"]       = meta.TenThietBi,
                ["KyHieu"]           = meta.KyHieu,
                ["SoHieu"]           = meta.SoHieu,
                ["SoTem"]            = meta.SoTem,
                ["NoiSanXuat"]       = meta.NoiSanXuat,
                ["DacTinhKyThuat"]   = meta.DacTinhKyThuat,
                ["DonViSuDung"]      = meta.DonViSuDung,
                ["PhuongPhap"]       = meta.PhuongPhap,
                ["NhietDoMoiTruong"] = meta.NhietDoMoiTruong,
                ["DoAmTuongDoi"]     = meta.DoAmTuongDoi,
                ["ThietBiChuan"]     = meta.ThietBiChuan,
                ["NgayHieuChuan"]    = meta.NgayHieuChuan.ToString("dd/MM/yyyy"),
                ["BangKetQua"]       = calibRows.Select((r, i) =>
                    new Dictionary<string, object>
                    {
                        ["STT"]          = i + 1,
                        ["GiaTriDat"]    = r.GiaTriDat.ToString("F1"),
                        ["GiaTriChiThi"] = r.GiaTriChiThi.ToString("F2"),
                        ["TrungBinh"]    = r.GiaTriTrungBinh.ToString("F2"),
                        ["SoHieuChinh"]  = r.SoHieuChinh.ToString("F2"),
                        ["DoOnDinh"]     = r.DoOnDinh.ToString("F2"),
                        ["DoDongDeu"]    = r.DoDongDeu.ToString("F2"),
                        ["DKDB"]         = $"±{r.DoKhongDamBao:F2}",
                    }).ToList()
            };

            await Task.Run(() =>
                MiniSoftware.MiniWord.SaveAsByTemplate(
                    dialog.FileName, templatePath, values));

            ToastNotification.ShowSuccess($"Đã xuất: {Path.GetFileName(dialog.FileName)}");
        }

        public static async Task ExportExcelAsync(
            SessionMetadata meta,
            List<CalibrationResultRow> calibRows,
            int kenhCount,
            IWin32Window owner)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"BienBan_{meta.SoHieu}_{meta.NgayHieuChuan:yyyyMMdd}.xlsx"
            };

            if (dialog.ShowDialog(owner) != DialogResult.OK)
                return;

            string templatePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources", "Templates", "Tu_nhiet_V3.xlsx");

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Không tìm thấy template: {templatePath}");

            File.Copy(templatePath, dialog.FileName, overwrite: true);

            await Task.Run(() => WriteExcelData(dialog.FileName, meta, calibRows, kenhCount));

            ToastNotification.ShowSuccess($"Đã xuất: {Path.GetFileName(dialog.FileName)}");
        }

        private static void WriteExcelData(
            string filePath,
            SessionMetadata meta,
            List<CalibrationResultRow> calibRows,
            int kenhCount)
        {
            using var doc = SpreadsheetDocument.Open(filePath, isEditable: true);
            var workbookPart = doc.WorkbookPart;
            if (workbookPart == null) return;

            string sheetName = kenhCount <= 3 ? "3 Pos" : "5 Pos";
            if (kenhCount > 5)
                AppLogger.Warning("ExcelExporter",
                    $"kenhCount={kenhCount} > 5, dùng sheet '5 Pos', chỉ ghi 5 kênh đầu.");

            var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
            if (sheet == null || sheet.Id == null) return;

            var wsp = (WorksheetPart)workbookPart.GetPartById(sheet.Id);

            SetCell(wsp, "C6",  meta.TenThietBi);
            SetCell(wsp, "H7",  meta.SoHieu);
            SetCell(wsp, "C8",  meta.NoiSanXuat);
            SetCell(wsp, "H8",  meta.KyHieu);
            SetCell(wsp, "C9",  meta.DacTinhKyThuat);
            SetCell(wsp, "C13", meta.DonViSuDung);
            SetCell(wsp, "D12", meta.PhuongPhap);
            SetCell(wsp, "C16", meta.NhietDoMoiTruong);
            SetCell(wsp, "G16", meta.DoAmTuongDoi);
            SetCell(wsp, "C17", meta.ThietBiChuan);
            SetCell(wsp, "C19", meta.NgayHieuChuan.ToString("dd/MM/yyyy"));
            SetCell(wsp, "H19", meta.SoTem);

            int maxPoints = Math.Min(calibRows.Count, 5);
            if (calibRows.Count > 5)
                AppLogger.Warning("ExcelExporter",
                    $"Có {calibRows.Count} điểm kiểm tra, template chỉ chứa 5. Các điểm từ 6 trở đi bị bỏ qua.");

            string[] dataCols = kenhCount <= 3
                ? new[] { "L", "M", "N" }
                : new[] { "L", "M", "N", "O", "P" };

            for (int p = 0; p < maxPoints; p++)
            {
                var row = calibRows[p];
                int startRow = 4 + p * 6;

                SetCell(wsp, $"K{startRow}", row.GiaTriDat.ToString("F1"));

                if (row.ChiTietLanDos == null || row.ChiTietLanDos.Count == 0)
                    continue;

                var byLanDo = row.ChiTietLanDos
                    .GroupBy(d => d.LanDo)
                    .OrderBy(g => g.Key)
                    .ToList();

                foreach (var lanDoGroup in byLanDo)
                {
                    int lanDoIdx = lanDoGroup.Key - 1;
                    if (lanDoIdx < 0 || lanDoIdx >= 5) continue;
                    int excelRow = startRow + lanDoIdx;

                    foreach (var chiTiet in lanDoGroup.OrderBy(d => d.Kenh))
                    {
                        int kenhIdx = chiTiet.Kenh - 1;
                        if (kenhIdx < 0 || kenhIdx >= dataCols.Length) continue;
                        string cellRef = $"{dataCols[kenhIdx]}{excelRow}";
                        SetCell(wsp, cellRef, chiTiet.GiaTri.ToString("F2"));
                    }
                }
            }

            doc.Save();
        }

        private static void SetCell(WorksheetPart wsp, string cellRef, string value)
        {
            var worksheet = wsp.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            if (sheetData == null) return;

            string columnName = new string(cellRef.Where(char.IsLetter).ToArray());
            if (!uint.TryParse(new string(cellRef.Where(char.IsDigit).ToArray()), out uint rowIndex))
                return;

            Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            if (row == null)
            {
                row = new Row { RowIndex = rowIndex };
                var refRow = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex > rowIndex);
                sheetData.InsertBefore(row, refRow);
            }

            Cell cell = row.Elements<Cell>().FirstOrDefault(c => c.CellReference?.Value == cellRef);
            if (cell == null)
            {
                cell = new Cell { CellReference = cellRef };
                var refCell = row.Elements<Cell>().FirstOrDefault(c => string.Compare(c.CellReference?.Value, cellRef, true) > 0);
                row.InsertBefore(cell, refCell);
            }

            cell.DataType = CellValues.InlineString;
            cell.InlineString = new InlineString { Text = new Text(value ?? string.Empty) };
        }
    }
}
