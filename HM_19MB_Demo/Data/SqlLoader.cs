using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace HM_19MB_Demo.Data
{
    internal static class SqlLoader
    {
        private static readonly ConcurrentDictionary<string, string> _cache = new();

        private const string ResourcePrefix = "HM_19MB_Demo.Sql.";

        public static string Load(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tên file SQL không được để trống.", nameof(name));

            return _cache.GetOrAdd(name, ReadFromAssembly);
        }

        private static string ReadFromAssembly(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = ResourcePrefix + name + ".sql";

            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException(
                    $"Không tìm thấy embedded resource: '{resourceName}'.\n" +
                    $"Kiểm tra file tồn tại trong thư mục Sql/ và Build Action = Embedded Resource.");

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static void ClearCache() => _cache.Clear();
    }
}
