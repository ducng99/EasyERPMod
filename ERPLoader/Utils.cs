using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ERPLoader
{
    static class Utils
    {
        private static readonly Dictionary<string, string[]> FilesCache = new();

        public static bool FileExists(Regex re, string folderPath)
        {
            bool found = false;

            var files = FilesCache.ContainsKey(folderPath) ? FilesCache[folderPath] : Directory.GetFiles(folderPath);

            try
            {
                foreach (var file in files)
                {
                    if (re.IsMatch(file))
                    {
                        found = true;
                        break;
                    }
                }
            }
            catch
            {
                found = false;
            }

            return found;
        }

        // I miss python "-" * length
        public static string Multiply(this string a, int times)
        {
            return string.Concat(System.Linq.Enumerable.Repeat(a, times));
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;

            public Utf8StringWriter()
                : base()
            {
            }
        }

        public static MemoryStream GetStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
    }
}
