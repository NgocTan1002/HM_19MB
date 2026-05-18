using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string path = @"c:\Users\Admin\Documents\Demo_CanDienTu\WeighingKeli\HM_19MB_Demo\HM_19MB_Demo\BienBanHieuChuan.docx";
        using (var archive = ZipFile.OpenRead(path))
        {
            var entry = archive.GetEntry("word/document.xml");
            using (var stream = entry.Open())
            using (var reader = new StreamReader(stream))
            {
                string xml = reader.ReadToEnd();
                // strip tags
                string text = Regex.Replace(xml, "<.*?>", "");
                foreach (Match m in Regex.Matches(text, @"\{.*?\}"))
                {
                    Console.WriteLine(m.Value);
                }
            }
        }
    }
}
