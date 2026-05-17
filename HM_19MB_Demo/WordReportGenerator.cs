using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HM_19MB_Demo.Data;

namespace HM_19MB_Demo
{
    /// <summary>
    /// Xuất báo cáo Word theo mẫu QTHC 1.013:2019.
    /// - Phụ lục A: Biên bản hiệu chuẩn (Bảng A.1)
    /// - Phụ lục B: Giấy chứng nhận hiệu chuẩn (Trang 1 + Trang 2)
    /// Hai file Word riêng, luôn đi cùng nhau.
    /// </summary>
    public static class WordReportGenerator
    {
        // ── Public entry points ──────────────────────────────────────────────

        /// <summary>
        /// Xuất Biên bản hiệu chuẩn (Phụ lục A).
        /// </summary>
        public static async Task<string> ExportBienBanAsync(int phienId, string outputPath)
        {
            var (meta, rows) = await LoadDataAsync(phienId);
            BuildBienBan(outputPath, meta, rows);
            return outputPath;
        }

        /// <summary>
        /// Xuất Giấy chứng nhận hiệu chuẩn (Phụ lục B).
        /// </summary>
        public static async Task<string> ExportGiayChungNhanAsync(int phienId, string outputPath)
        {
            var (meta, rows) = await LoadDataAsync(phienId);
            BuildGiayChungNhan(outputPath, meta, rows);
            return outputPath;
        }

        /// <summary>
        /// Xuất cả hai file. Trả về (pathBienBan, pathGiayChungNhan).
        /// outputDir: thư mục lưu. Tên file tự động.
        /// </summary>
        public static async Task<(string BienBan, string GiayChungNhan)> ExportBothAsync(
            int phienId, string outputDir)
        {
            var (meta, rows) = await LoadDataAsync(phienId);

            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string safe = string.Concat((meta.KyHieu ?? "").Select(c => char.IsLetterOrDigit(c) ? c : '_'));
            if (string.IsNullOrWhiteSpace(safe)) safe = phienId.ToString();

            string pathA = Path.Combine(outputDir, $"BienBan_{safe}_{stamp}.docx");
            string pathB = Path.Combine(outputDir, $"GiayChungNhan_{safe}_{stamp}.docx");

            BuildBienBan(pathA, meta, rows);
            BuildGiayChungNhan(pathB, meta, rows);

            return (pathA, pathB);
        }

        // ── Data loading ─────────────────────────────────────────────────────

        private static async Task<(SessionMetadata meta, List<CalibrationResultRow> rows)>
            LoadDataAsync(int phienId)
        {
            var meta = await DatabaseService.LayPhienAsync(phienId)
                       ?? throw new InvalidOperationException("Không tìm thấy phiên hiệu chuẩn.");
            var rows = await DatabaseService.LayKetQuaHieuChuanAsync(phienId);
            if (rows.Count == 0)
                throw new InvalidOperationException("Chưa có kết quả hiệu chuẩn để xuất báo cáo.");
            return (meta, rows);
        }

        // ════════════════════════════════════════════════════════════════════
        // PHỤ LỤC A — Biên bản hiệu chuẩn
        // ════════════════════════════════════════════════════════════════════

        private static void BuildBienBan(
            string outputPath,
            SessionMetadata meta,
            List<CalibrationResultRow> rows)
        {
            using var doc = WordprocessingDocument.Create(
                outputPath, WordprocessingDocumentType.Document);

            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            SetPageMargins(body);

            // ── Header: 2 cột ĐƠN VỊ | CỘNG HOÀ ────────────────────────────
            var headerTable = MakeTable(new[] { 4500, 4500 });
            var hRow = new TableRow();
            hRow.Append(
                MakeCell("ĐƠN VỊ\n(2 cấp)", bold: true, center: true, width: 4500),
                MakeCell(
                    "CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM\nĐộc lập – Tự do – Hạnh phúc\n" +
                    $"Hà Nội, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}",
                    bold: false, center: true, width: 4500));
            headerTable.AppendChild(hRow);
            body.AppendChild(headerTable);

            AddEmptyLine(body);

            // ── Tiêu đề ──────────────────────────────────────────────────────
            AddCenteredParagraph(body, "BIÊN BẢN HIỆU CHUẨN", bold: true, fontSize: 14);
            AddLeftParagraph(body, $"Số: {DateTime.Now:yyyyMMdd}-{meta.KyHieu}", bold: false, fontSize: 11);
            AddEmptyLine(body);

            // ── Thông tin phiên ───────────────────────────────────────────────
            AddLeftParagraph(body, "Tên phương tiện đo: Tủ nhiệt", bold: false, fontSize: 11);
            AddLeftParagraph(body,
                $"Ký hiệu: {meta.KyHieu}          " +
                $"Số hiệu: {meta.SoHieu}          " +
                $"Số tem hiệu chuẩn: {meta.SoTem}", bold: false, fontSize: 11);
            AddLeftParagraph(body,
                $"Nơi sản xuất: {meta.NoiSanXuat}          " +
                $"Năm sản xuất: {meta.NamSanXuat}", bold: false, fontSize: 11);
            AddLeftParagraph(body, $"Đơn vị sử dụng: {meta.DonViSuDung}", bold: false, fontSize: 11);
            AddLeftParagraph(body, $"Phương pháp thực hiện: {meta.PhuongPhap}", bold: false, fontSize: 11);
            AddEmptyLine(body);
            AddLeftParagraph(body, "Điều kiện hiệu chuẩn:", bold: false, fontSize: 11);
            AddLeftParagraph(body, $"Nhiệt độ môi trường: {meta.NhietDoMoiTruong}", bold: false, fontSize: 11);
            AddLeftParagraph(body, $"Độ ẩm tương đối: {meta.DoAmTuongDoi}", bold: false, fontSize: 11);
            AddLeftParagraph(body, $"Đặc tính kỹ thuật: {meta.DacTinhKyThuat}", bold: false, fontSize: 11);
            AddLeftParagraph(body, $"Các phương tiện đo sử dụng: {meta.ThietBiChuan}", bold: false, fontSize: 11);
            AddLeftParagraph(body,
                $"Đã tiến hành hiệu chuẩn ngày {meta.NgayHieuChuan.Day} " +
                $"tháng {meta.NgayHieuChuan.Month} " +
                $"năm {meta.NgayHieuChuan.Year}", bold: false, fontSize: 11);
            AddEmptyLine(body);

            // ── KẾT QUẢ HIỆU CHUẨN ───────────────────────────────────────────
            AddLeftParagraph(body, "KẾT QUẢ HIỆU CHUẨN", bold: true, fontSize: 12);
            AddEmptyLine(body);
            AddLeftParagraph(body,
                "A.1 Kiểm tra bên ngoài:   ☑ Đạt                    ☐ Không đạt",
                bold: false, fontSize: 11);
            AddLeftParagraph(body,
                "A.2 Kiểm tra kỹ thuật:    ☑ Đạt                    ☐ Không đạt",
                bold: false, fontSize: 11);
            AddLeftParagraph(body, "A.3 Kiểm tra đo lường:", bold: false, fontSize: 11);
            AddEmptyLine(body);
            AddLeftParagraph(body, "Bảng A.1 – Kết quả kiểm tra đo lường", bold: true, fontSize: 11);
            AddEmptyLine(body);

            // ── Bảng A.1 ─────────────────────────────────────────────────────
            body.AppendChild(BuildTableA1(rows));

            AddEmptyLine(body);
            AddEmptyLine(body);

            // ── Chữ ký ───────────────────────────────────────────────────────
            body.AppendChild(BuildSignatureTable("Người soát lại", "Người hiệu chuẩn"));

            mainPart.Document.Save();
        }

        /// <summary>
        /// Bảng A.1: STT | Giá trị đặt | Giá trị chỉ thị | Vị trí 1..k | Trung bình
        /// Số cột "Vị trí" động theo k thực tế của các dòng.
        /// </summary>
        private static Table BuildTableA1(List<CalibrationResultRow> rows)
        {
            int k = rows.Max(r => r.SoKenh > 0 ? r.SoKenh : 1);

            // Tính độ rộng cột (tổng 9000 DXA = A4 nội dung ~16 cm)
            int totalWidth = 9000;
            int sttW = 500;
            int datW = 900;
            int chiThiW = 900;
            int tbW = 900;
            int remaining = totalWidth - sttW - datW - chiThiW - tbW;
            int viTriW = k > 0 ? remaining / k : remaining;

            var colWidths = new List<int> { sttW, datW, chiThiW };
            for (int i = 0; i < k; i++) colWidths.Add(viTriW);
            colWidths.Add(tbW);

            var table = MakeTable(colWidths.ToArray());

            // Header row 1: nhóm cột
            var hRow1 = new TableRow();
            hRow1.Append(MakeHeaderCell("STT", sttW, rowSpan: 2));
            hRow1.Append(MakeHeaderCell("Giá trị đặt, °C", datW, rowSpan: 2));
            hRow1.Append(MakeHeaderCell("Giá trị chỉ thị, °C", chiThiW, rowSpan: 2));
            // Merge "Giá trị đọc trên chuẩn" span k+1 cột
            int readingsWidth = viTriW * k + tbW;
            hRow1.Append(MakeHeaderCell("Giá trị đọc trên chuẩn, °C", readingsWidth, colSpan: k + 1));
            table.AppendChild(hRow1);

            // Header row 2: Vị trí 1..k + Trung bình
            var hRow2 = new TableRow();
            for (int j = 1; j <= k; j++)
                hRow2.Append(MakeHeaderCell($"Vị trí {j}", viTriW));
            hRow2.Append(MakeHeaderCell("Trung bình", tbW));
            table.AppendChild(hRow2);

            // Data rows
            foreach (var row in rows)
            {
                var dr = new TableRow();
                dr.Append(MakeDataCell(row.STT.ToString(), sttW));
                dr.Append(MakeDataCell(row.GiaTriDat.ToString("F1"), datW));
                dr.Append(MakeDataCell(row.GiaTriChiThi.ToString("F1"), chiThiW));
                for (int j = 0; j < k; j++)
                {
                    string val = j < row.Kenh.Length && !double.IsNaN(row.Kenh[j])
                        ? row.Kenh[j].ToString("F2")
                        : "---";
                    dr.Append(MakeDataCell(val, viTriW));
                }
                dr.Append(MakeDataCell(row.GiaTriTrungBinh.ToString("F2"), tbW));
                table.AppendChild(dr);
            }

            return table;
        }

        // ════════════════════════════════════════════════════════════════════
        // PHỤ LỤC B — Giấy chứng nhận hiệu chuẩn
        // ════════════════════════════════════════════════════════════════════

        private static void BuildGiayChungNhan(
            string outputPath,
            SessionMetadata meta,
            List<CalibrationResultRow> rows)
        {
            using var doc = WordprocessingDocument.Create(
                outputPath, WordprocessingDocumentType.Document);

            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            SetPageMargins(body);

            // ════ TRANG 1 ════════════════════════════════════════════════════

            // Header cơ quan
            AddCenteredParagraph(body, "CỤC TIÊU CHUẨN - ĐO LƯỜNG - CHẤT LƯỢNG", bold: true, fontSize: 11);
            AddCenteredParagraph(body, "(Department for Standard, Metrology and Quality)", bold: false, fontSize: 10);
            AddCenteredParagraph(body, "TRUNG TÂM ĐO LƯỜNG", bold: true, fontSize: 11);
            AddCenteredParagraph(body, "(Metrology Center)", bold: false, fontSize: 10);
            AddCenteredParagraph(body, "(ĐK35)", bold: false, fontSize: 10);
            AddEmptyLine(body);
            AddLeftParagraph(body,
                "Địa chỉ (Add): Số 11 Hoàng Sâm - Nghĩa Đô - Cầu Giấy - Hà Nội",
                bold: false, fontSize: 10);
            AddLeftParagraph(body,
                "Điện thoại (Tel) 04.38361108             Fax: 04.37563660",
                bold: false, fontSize: 10);
            AddEmptyLine(body);
            AddEmptyLine(body);

            // Tiêu đề
            AddCenteredParagraph(body, "GIẤY CHỨNG NHẬN HIỆU CHUẨN", bold: true, fontSize: 14);
            AddCenteredParagraph(body, "(Calibration Certificate)", bold: false, fontSize: 12);
            AddEmptyLine(body);

            AddLeftParagraph(body,
                $"Số (No): {DateTime.Now:yyyyMMdd}-{meta.KyHieu}",
                bold: false, fontSize: 11);
            AddEmptyLine(body);

            // Thông tin thiết bị
            AddLeftParagraph(body,
                $"Tên phương tiện đo (Object): Tủ nhiệt",
                bold: false, fontSize: 11);
            AddLeftParagraph(body,
                $"Kiểu (Type): {meta.KyHieu}                    Số (Serial No): {meta.SoHieu}",
                bold: false, fontSize: 11);
            AddLeftParagraph(body,
                $"Nơi sản xuất (Manufacturer): {meta.NoiSanXuat}",
                bold: false, fontSize: 11);
            AddLeftParagraph(body,
                $"Đặc trưng kỹ thuật (Technical Specification): {meta.DacTinhKyThuat}",
                bold: false, fontSize: 11);
            AddLeftParagraph(body,
                $"Cơ sở sử dụng (Customer): {meta.DonViSuDung}",
                bold: false, fontSize: 11);
            AddEmptyLine(body);
            AddLeftParagraph(body,
                $"Phương pháp thực hiện (Method of Calibration): {meta.PhuongPhap}",
                bold: false, fontSize: 11);
            AddLeftParagraph(body,
                $"Điều kiện môi trường (Environmental Conditions): " +
                $"Nhiệt độ: {meta.NhietDoMoiTruong}, Độ ẩm: {meta.DoAmTuongDoi}",
                bold: false, fontSize: 11);
            AddLeftParagraph(body,
                $"Chuẩn được sử dụng (Standards used): {meta.ThietBiChuan}",
                bold: false, fontSize: 11);
            AddEmptyLine(body);
            AddLeftParagraph(body,
                "Kết quả (Results): Xem trang kèm theo",
                bold: false, fontSize: 11);
            AddEmptyLine(body);
            AddLeftParagraph(body,
                $"Ngày hiệu chuẩn (Date of Cal): {meta.NgayHieuChuan:dd/MM/yyyy}",
                bold: false, fontSize: 11);
            AddLeftParagraph(body,
                $"Số tem hiệu chuẩn (No of Cal. Label): {meta.SoTem}",
                bold: false, fontSize: 11);
            AddLeftParagraph(body,
                $"Ngày khuyến nghị hiệu chuẩn tới (Recalibration Recommended): " +
                $"{meta.NgayHieuChuan.AddYears(1):dd/MM/yyyy}",
                bold: false, fontSize: 11);
            AddEmptyLine(body);
            AddEmptyLine(body);

            AddLeftParagraph(body,
                $"Hà Nội, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}",
                bold: false, fontSize: 11);
            AddEmptyLine(body);
            body.AppendChild(BuildSignatureTable(
                "Trưởng phòng thí nghiệm\n(Head of the Cal. Lab.)",
                "GIÁM ĐỐC\n(Director)"));
            AddEmptyLine(body);

            // Footer trang 1
            body.AppendChild(BuildFooterTable("Trang: 1",
                "Không được sao chép rời khi giấy chứng nhận có nhiều trang " +
                "nếu không được sự đồng ý bằng văn bản của Trung tâm Đo lường\n" +
                "(This certificate shall not be reproduced except in full, " +
                "without the written approval of Metrology Center)"));

            // ── Page break ───────────────────────────────────────────────────
            body.AppendChild(CreatePageBreak());

            // ════ TRANG 2 ════════════════════════════════════════════════════

            AddCenteredParagraph(body, "KẾT QUẢ HIỆU CHUẨN", bold: true, fontSize: 14);
            AddCenteredParagraph(body, "(Calibration results)", bold: false, fontSize: 12);
            AddEmptyLine(body);

            body.AppendChild(BuildTableB2(rows));

            AddEmptyLine(body);
            AddCenteredParagraph(body, "(k = 2; P = 95%)", bold: false, fontSize: 10);
            AddEmptyLine(body);

            AddLeftParagraph(body,
                $"Kèm theo giấy chứng nhận hiệu chuẩn số: {DateTime.Now:yyyyMMdd}-{meta.KyHieu}",
                bold: false, fontSize: 10);
            AddLeftParagraph(body, "(Attached to certificate No)", bold: false, fontSize: 9);

            // Footer trang 2
            AddEmptyLine(body);
            body.AppendChild(BuildFooterTable("Trang: 2", ""));

            mainPart.Document.Save();
        }

        /// <summary>
        /// Bảng trang 2 Phụ lục B:
        /// STT | Giá trị đặt | Giá trị chỉ thị | Trung bình chuẩn | Số hiệu chính | Độ ổn định | Độ đồng đều | ĐKĐB mở rộng
        /// </summary>
        private static Table BuildTableB2(List<CalibrationResultRow> rows)
        {
            int[] widths = { 500, 900, 900, 1200, 1000, 1000, 1000, 1500 };
            var table = MakeTable(widths);

            // Header
            var hRow = new TableRow();
            hRow.Append(MakeHeaderCell("STT", widths[0]));
            hRow.Append(MakeHeaderCell("Giá trị đặt, °C", widths[1]));
            hRow.Append(MakeHeaderCell("Giá trị chỉ thị, °C", widths[2]));
            hRow.Append(MakeHeaderCell("Giá trị trung bình\nđọc trên chuẩn, °C", widths[3]));
            hRow.Append(MakeHeaderCell("Số hiệu chính, °C", widths[4]));
            hRow.Append(MakeHeaderCell("Độ ổn định, °C", widths[5]));
            hRow.Append(MakeHeaderCell("Độ đồng đều, °C", widths[6]));
            hRow.Append(MakeHeaderCell("Độ không đảm bảo\nđo mở rộng, °C", widths[7]));
            table.AppendChild(hRow);

            // Data
            foreach (var row in rows)
            {
                var dr = new TableRow();
                dr.Append(MakeDataCell(row.STT.ToString(), widths[0]));
                dr.Append(MakeDataCell(row.GiaTriDat.ToString("F1"), widths[1]));
                dr.Append(MakeDataCell(row.GiaTriChiThi.ToString("F1"), widths[2]));
                dr.Append(MakeDataCell(row.GiaTriTrungBinh.ToString("F2"), widths[3]));
                dr.Append(MakeDataCell(row.SoHieuChinh.ToString("F2"), widths[4]));
                dr.Append(MakeDataCell($"±{row.DoOnDinh:F2}", widths[5]));
                dr.Append(MakeDataCell($"±{row.DoDongDeu:F2}", widths[6]));
                dr.Append(MakeDataCell($"±{row.DoKhongDamBao:F2}", widths[7]));
                table.AppendChild(dr);
            }

            return table;
        }

        // ════════════════════════════════════════════════════════════════════
        // OpenXML helper methods
        // ════════════════════════════════════════════════════════════════════

        private static void SetPageMargins(Body body)
        {
            var sectPr = new SectionProperties(
                new PageSize { Width = 11906, Height = 16838 }, // A4
                new PageMargin
                {
                    Top = 1134, // ~2 cm
                    Bottom = 1134,
                    Left = 1701, // ~3 cm
                    Right = 851   // ~1.5 cm
                });
            body.AppendChild(sectPr);
        }

        private static void AddCenteredParagraph(Body body, string text, bool bold, int fontSize)
        {
            var para = new Paragraph();
            para.AppendChild(new ParagraphProperties(
                new Justification { Val = JustificationValues.Center }));
            foreach (var line in text.Split('\n'))
            {
                var run = new Run();
                var rp = new RunProperties();
                rp.AppendChild(new FontSize { Val = (fontSize * 2).ToString() });
                rp.AppendChild(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
                if (bold) rp.AppendChild(new Bold());
                run.AppendChild(rp);
                run.AppendChild(new Text(line) { Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                para.AppendChild(new Run(new Break()));
            }
            body.AppendChild(para);
        }

        private static void AddLeftParagraph(Body body, string text, bool bold, int fontSize)
        {
            var para = new Paragraph();
            var run = new Run();
            var rp = new RunProperties();
            rp.AppendChild(new FontSize { Val = (fontSize * 2).ToString() });
            rp.AppendChild(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
            if (bold) rp.AppendChild(new Bold());
            run.AppendChild(rp);
            run.AppendChild(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
            para.AppendChild(run);
            body.AppendChild(para);
        }

        private static void AddEmptyLine(Body body)
            => body.AppendChild(new Paragraph());

        private static Paragraph CreatePageBreak()
        {
            var para = new Paragraph();
            para.AppendChild(new Run(new Break { Type = BreakValues.Page }));
            return para;
        }

        // ── Table factories ──────────────────────────────────────────────────

        private static Table MakeTable(int[] columnWidths)
        {
            var tbl = new Table();
            var tblPr = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                    new BottomBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                    new LeftBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                    new RightBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" }
                ),
                new TableWidth { Width = columnWidths.Sum().ToString(), Type = TableWidthUnitValues.Dxa }
            );
            tbl.AppendChild(tblPr);

            var tblGrid = new TableGrid();
            foreach (var w in columnWidths)
                tblGrid.AppendChild(new GridColumn { Width = w.ToString() });
            tbl.AppendChild(tblGrid);

            return tbl;
        }

        private static TableCell MakeCell(
            string text, bool bold, bool center, int width,
            int rowSpan = 1, int colSpan = 1,
            bool headerStyle = false)
        {
            var cell = new TableCell();

            var tcp = new TableCellProperties(
                new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa });

            if (rowSpan > 1)
                tcp.AppendChild(new VerticalMerge { Val = MergedCellValues.Restart });

            if (colSpan > 1)
                tcp.AppendChild(new GridSpan { Val = colSpan });

            if (headerStyle)
                tcp.AppendChild(new Shading
                {
                    Fill = "D9E1F2",
                    Val = ShadingPatternValues.Clear
                });

            tcp.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
            cell.AppendChild(tcp);

            var lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                var para = new Paragraph();
                if (center)
                    para.AppendChild(new ParagraphProperties(
                        new Justification { Val = JustificationValues.Center }));

                var run = new Run();
                var rp = new RunProperties();
                rp.AppendChild(new FontSize { Val = "18" }); // 9pt
                rp.AppendChild(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
                if (bold) rp.AppendChild(new Bold());
                run.AppendChild(rp);
                run.AppendChild(new Text(lines[i]) { Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
            }

            return cell;
        }

        private static TableCell MakeHeaderCell(
            string text, int width, int rowSpan = 1, int colSpan = 1)
            => MakeCell(text, bold: true, center: true, width: width,
                        rowSpan: rowSpan, colSpan: colSpan, headerStyle: true);

        private static TableCell MakeDataCell(string text, int width)
            => MakeCell(text, bold: false, center: true, width: width);

        private static Table BuildSignatureTable(string leftTitle, string rightTitle)
        {
            var tbl = MakeTable(new[] { 4500, 4500 });
            // Remove borders for signature table
            var tblPr = tbl.GetFirstChild<TableProperties>()!;
            tblPr.RemoveAllChildren<TableBorders>();
            tblPr.AppendChild(new TableBorders(
                new TopBorder { Val = BorderValues.None },
                new BottomBorder { Val = BorderValues.None },
                new LeftBorder { Val = BorderValues.None },
                new RightBorder { Val = BorderValues.None },
                new InsideHorizontalBorder { Val = BorderValues.None },
                new InsideVerticalBorder { Val = BorderValues.None }
            ));

            var row = new TableRow();

            // Left cell
            var c1 = new TableCell();
            c1.AppendChild(new TableCellProperties(
                new TableCellWidth { Width = "4500", Type = TableWidthUnitValues.Dxa }));
            c1.AppendChild(MakeSigPara(leftTitle));
            row.Append(c1);

            // Right cell
            var c2 = new TableCell();
            c2.AppendChild(new TableCellProperties(
                new TableCellWidth { Width = "4500", Type = TableWidthUnitValues.Dxa }));
            c2.AppendChild(MakeSigPara(rightTitle));
            row.Append(c2);

            tbl.AppendChild(row);
            return tbl;
        }

        private static Paragraph MakeSigPara(string text)
        {
            var para = new Paragraph();
            para.AppendChild(new ParagraphProperties(
                new Justification { Val = JustificationValues.Center }));
            foreach (var line in text.Split('\n'))
            {
                var run = new Run();
                var rp = new RunProperties();
                rp.AppendChild(new FontSize { Val = "22" }); // 11pt
                rp.AppendChild(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
                run.AppendChild(rp);
                run.AppendChild(new Text(line) { Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                para.AppendChild(new Run(new Break()));
            }
            return para;
        }

        private static Table BuildFooterTable(string pageText, string noteText)
        {
            var tbl = MakeTable(new[] { 2000, 7000 });
            var row = new TableRow();
            row.Append(MakeCell(pageText, bold: false, center: true, width: 2000));
            row.Append(MakeCell(noteText, bold: false, center: false, width: 7000));
            tbl.AppendChild(row);
            return tbl;
        }
    }
}