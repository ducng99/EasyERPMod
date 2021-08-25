using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ERPLoader
{
    public static class Utils
    {
        public static bool FileExists(Regex re, string folderPath)
        {
            var files = Directory.EnumerateFiles(folderPath);

            foreach (var file in files)
            {
                if (re.IsMatch(file))
                {
                    return true;
                }
            }

            return false;
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

        public static bool ContainParentFolder(string path, Regex folderToFind)
        {
            string currentPath;
            DirectoryInfo dirInfo = new(path);

            do
            {
                currentPath = dirInfo.FullName;
                dirInfo = Directory.GetParent(currentPath);

                if (dirInfo != null && folderToFind.IsMatch(dirInfo.Name))
                {
                    return true;
                }
            } while (dirInfo != null);

            return false;
        }
    }
}
