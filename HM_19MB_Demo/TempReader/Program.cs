using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

class Program
{
    static void Main()
    {
        string path1 = @"C:\Users\Admin\Documents\Demo_CanDienTu\WeighingKeli\HM_19MB_Demo\HM_19MB_Demo\BienBanHieuChuan.docx";
        string path2 = @"C:\Users\Admin\Documents\Demo_CanDienTu\WeighingKeli\HM_19MB_Demo\HM_19MB_Demo\bin\Debug\net10.0-windows\Resources\Templates\BienBanHieuChuan.docx";
        
        CheckFile(path1);
        CheckFile(path2);
    }
    
    static void CheckFile(string path)
    {
        Console.WriteLine($"Checking {path}");
        if (!File.Exists(path))
        {
            Console.WriteLine("File not found.");
            return;
        }
        try
        {
            using (var doc = WordprocessingDocument.Open(path, false))
            {
                var body = doc.MainDocumentPart.Document.Body;
                string text = body.InnerText;
                Console.WriteLine("Text preview:");
                Console.WriteLine(text.Substring(0, Math.Min(500, text.Length)));
                Console.WriteLine("Contains '{{': " + text.Contains("{{"));
                Console.WriteLine("Contains '{': " + text.Contains("{"));
                Console.WriteLine("Contains '[': " + text.Contains("["));
                Console.WriteLine("-----------------------");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
