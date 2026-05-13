using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace HM_19MB_Demo
{
    /// <summary>
    /// Tạo báo cáo Word theo mẫu QTHC (Giấy chứng nhận hiệu chuẩn)
    /// </summary>
    public static class WordReportGenerator
    {
        public static async Task<string> ExportToWordAsync(int sessionId, string outputPath)
        {
            var data = await ReportGenerator.GetSessionDataAsync(sessionId);

            if (data.MeasurementRecords.Count == 0)
            {
                throw new InvalidOperationException("Không có dữ liệu đo để xuất báo cáo.");
            }

            // Tạo document Word
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Trang 1: Giấy chứng nhận
                CreateCertificatePage(body, data);

                // Page break
                body.AppendChild(CreatePageBreak());

                // Trang 2: Kết quả hiệu chuẩn
                CreateResultsPage(body, data);

                mainPart.Document.Save();
            }

            return outputPath;
        }

        private static void CreateCertificatePage(Body body, SessionReportData data)
        {
            var meta = data.Metadata;

            // Header - Thông tin trung tâm
            AddCenteredParagraph(body, "CỤC TIÊU CHUẨN - ĐO LƯỜNG - CHẤT LƯỢNG", true, 11);
            AddCenteredParagraph(body, "(Department for Standard, Metrology and Quality)", false, 10);
            AddCenteredParagraph(body, "TRUNG TÂM ĐO LƯỜNG", true, 11);
            AddCenteredParagraph(body, "(Metrology Center)", false, 10);
            AddCenteredParagraph(body, "(ĐK35)", false, 10);
            AddEmptyLine(body);

            AddLeftParagraph(body, "Địa chỉ (Add): Số 11 Hoàng Sâm - Nghĩa Đô - Cầu Giấy - Hà Nội", false, 10);
            AddLeftParagraph(body, "Điện thoại (Tel) 04.38361108             Fax: 04.37563660", false, 10);
            AddEmptyLine(body);
            AddEmptyLine(body);

            // Tiêu đề chính
            AddCenteredParagraph(body, "GIẤY CHỨNG NHẬN HIỆU CHUẨN", true, 14);
            AddCenteredParagraph(body, "(Calibration Certificate)", false, 12);
            AddEmptyLine(body);

            AddLeftParagraph(body, $"Số (No): {DateTime.Now:yyyyMMdd}-{data.Metadata.DeviceCode}", false, 11);
            AddEmptyLine(body);

            // Thông tin thiết bị
            AddLeftParagraph(body, $"Tên phương tiện đo (Object): {meta.DeviceName}", false, 11);
            AddLeftParagraph(body, $"Kiểu (Type): {meta.DeviceCode}                    Số (Serial No): {meta.DeviceNumber}", false, 11);
            AddLeftParagraph(body, $"Nơi sản xuất (Manufacturer): {meta.Manufacturer}", false, 11);
            AddLeftParagraph(body, $"Đặc trưng kỹ thuật (Technical Specification): {meta.TechnicalSpecs}", false, 11);
            AddLeftParagraph(body, $"Cơ sở sử dụng (Customer): {meta.UsingUnit}", false, 11);
            AddEmptyLine(body);

            AddLeftParagraph(body, $"Phương pháp thực hiện (Method of Calibration): {meta.Method}", false, 11);
            AddLeftParagraph(body, $"Điều kiện môi trường (Environmental Conditions): Nhiệt độ: {meta.EnvTemperature}, Độ ẩm: {meta.EnvHumidity}", false, 11);
            AddLeftParagraph(body, $"Chuẩn được sử dụng (Standards used): {meta.MeasuringDevices}", false, 11);
            AddEmptyLine(body);

            AddLeftParagraph(body, "Kết quả (Results): Xem trang kèm theo", false, 11);
            AddEmptyLine(body);

            AddLeftParagraph(body, $"Ngày hiệu chuẩn (Date of Cal): {meta.CalibrationDay:D2}/{meta.CalibrationMonth:D2}/{meta.CalibrationYear}", false, 11);
            AddLeftParagraph(body, $"Số tem hiệu chuẩn (No of Cal. Label): {meta.SealNumber}", false, 11);
            AddLeftParagraph(body, $"Ngày khuyến nghị hiệu chuẩn tới (Recalibration Recommended): {meta.CalibrationDay:D2}/{meta.CalibrationMonth:D2}/{meta.CalibrationYear + 1}", false, 11);
            AddEmptyLine(body);
            AddEmptyLine(body);

            // Chữ ký
            AddLeftParagraph(body, $"Hà Nội, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}", false, 11);
            AddEmptyLine(body);

            var sigTable = CreateSignatureTable();
            body.AppendChild(sigTable);

            AddEmptyLine(body);
            AddCenteredParagraph(body, "Trang: 1", false, 10);
            AddLeftParagraph(body, "Không được sao chép rời khi giấy chứng nhận có nhiều trang nếu không được sự đồng ý bằng văn bản của Trung tâm Đo lường", false, 9);
            AddLeftParagraph(body, "(This certificate shall not be reproduced except in full, without the written approval of Metrology Center)", false, 9);
        }

        private static void CreateResultsPage(Body body, SessionReportData data)
        {
            AddCenteredParagraph(body, "KẾT QUẢ HIỆU CHUẨN", true, 14);
            AddCenteredParagraph(body, "(Calibration results)", false, 12);
            AddEmptyLine(body);

            // Tạo bảng kết quả
            Table table = new Table();

            // Table properties
            TableProperties tblProp = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                ),
                new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct }
            );
            table.AppendChild(tblProp);

            // Header row
            TableRow headerRow = new TableRow();
            headerRow.Append(CreateTableCell("STT", true));
            headerRow.Append(CreateTableCell("Giá trị đặt, °C", true));
            headerRow.Append(CreateTableCell("Giá trị chỉ thị, °C", true));
            headerRow.Append(CreateTableCell("Giá trị trung bình đọc trên chuẩn, °C", true));
            headerRow.Append(CreateTableCell("Số hiệu chính, °C", true));
            headerRow.Append(CreateTableCell("Độ ổn định, °C", true));
            headerRow.Append(CreateTableCell("Độ đồng đều, °C", true));
            headerRow.Append(CreateTableCell("Độ không đảm bảo đo mở rộng, °C", true));
            table.Append(headerRow);

            // Data rows - Tính trung bình cho mỗi đầu đo
            var probeStats = ReportGenerator.CalculateProbeStatistics(data.MeasurementRecords);
            int rowNum = 1;

            foreach (var stat in probeStats)
            {
                TableRow dataRow = new TableRow();
                dataRow.Append(CreateTableCell(rowNum.ToString(), false));
                dataRow.Append(CreateTableCell(stat.AvgTemperature.ToString("F1"), false)); // Giá trị đặt
                dataRow.Append(CreateTableCell(stat.AvgTemperature.ToString("F1"), false)); // Giá trị chỉ thị
                dataRow.Append(CreateTableCell(stat.AvgTemperature.ToString("F1"), false)); // Giá trị chuẩn
                dataRow.Append(CreateTableCell("0.0", false)); // Số hiệu chính
                dataRow.Append(CreateTableCell((stat.MaxTemperature - stat.MinTemperature).ToString("F1"), false)); // Độ ổn định
                dataRow.Append(CreateTableCell((stat.MaxTemperature - stat.MinTemperature).ToString("F1"), false)); // Độ đồng đều
                dataRow.Append(CreateTableCell("±0.5", false)); // Độ không đảm bảo
                table.Append(dataRow);
                rowNum++;
            }

            body.AppendChild(table);
            AddEmptyLine(body);
            AddCenteredParagraph(body, "(k = 2; P = 95%)", false, 10);
            AddEmptyLine(body);

            // Vị trí hiệu chuẩn - Sơ đồ đầu đo
            AddCenteredParagraph(body, "Vị trí hiệu chuẩn", true, 11);
            AddCenteredParagraph(body, "(Sơ đồ bố trí 10 đầu đo theo QTHC 1.013:2019)", false, 10);
            AddEmptyLine(body);

            // Footer
            AddLeftParagraph(body, $"Kèm theo giấy chứng nhận hiệu chuẩn số: {DateTime.Now:yyyyMMdd}-{data.Metadata.DeviceCode}", false, 10);
            AddLeftParagraph(body, "(Attached to certificate No)", false, 9);
            AddCenteredParagraph(body, "Trang: 2", false, 10);
        }

        // Helper methods
        private static void AddCenteredParagraph(Body body, string text, bool bold, int fontSize)
        {
            Paragraph para = new Paragraph();
            ParagraphProperties paraProps = new ParagraphProperties(
                new Justification() { Val = JustificationValues.Center }
            );
            para.AppendChild(paraProps);

            Run run = new Run();
            RunProperties runProps = new RunProperties();
            runProps.AppendChild(new FontSize() { Val = (fontSize * 2).ToString() });
            if (bold) runProps.AppendChild(new Bold());
            run.AppendChild(runProps);
            run.AppendChild(new Text(text));
            para.AppendChild(run);
            body.AppendChild(para);
        }

        private static void AddLeftParagraph(Body body, string text, bool bold, int fontSize)
        {
            Paragraph para = new Paragraph();
            Run run = new Run();
            RunProperties runProps = new RunProperties();
            runProps.AppendChild(new FontSize() { Val = (fontSize * 2).ToString() });
            if (bold) runProps.AppendChild(new Bold());
            run.AppendChild(runProps);
            run.AppendChild(new Text(text));
            para.AppendChild(run);
            body.AppendChild(para);
        }

        private static void AddEmptyLine(Body body)
        {
            body.AppendChild(new Paragraph());
        }

        private static Paragraph CreatePageBreak()
        {
            Paragraph para = new Paragraph();
            Run run = new Run();
            run.AppendChild(new Break() { Type = BreakValues.Page });
            para.AppendChild(run);
            return para;
        }

        private static Table CreateSignatureTable()
        {
            Table table = new Table();
            TableProperties tblProp = new TableProperties(
                new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct }
            );
            table.AppendChild(tblProp);

            TableRow row = new TableRow();
            
            TableCell cell1 = new TableCell();
            cell1.Append(new Paragraph(new Run(new Text("Trưởng phòng thí nghiệm"))));
            cell1.Append(new Paragraph(new Run(new Text("(Head of the Cal. Lab.)"))));
            row.Append(cell1);

            TableCell cell2 = new TableCell();
            cell2.Append(new Paragraph(new Run(new Text("GIÁM ĐỐC"))));
            cell2.Append(new Paragraph(new Run(new Text("(Director)"))));
            row.Append(cell2);

            table.Append(row);
            return table;
        }

        private static TableCell CreateTableCell(string text, bool bold)
        {
            TableCell cell = new TableCell();
            Paragraph para = new Paragraph();
            Run run = new Run();
            
            RunProperties runProps = new RunProperties();
            runProps.AppendChild(new FontSize() { Val = "18" }); // 9pt
            if (bold) runProps.AppendChild(new Bold());
            run.AppendChild(runProps);
            run.AppendChild(new Text(text));
            
            para.AppendChild(run);
            cell.AppendChild(para);
            
            return cell;
        }
    }
}
