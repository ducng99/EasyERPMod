using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ERPLoader
{
    class Utils
    {
        private static readonly Dictionary<string, string[]> FilesCache = new();

        public static bool FileExists(Regex re, string folderPath, out string fileName)
        {
            bool found = false;
            fileName = string.Empty;

            var files = FilesCache.ContainsKey(folderPath) ? FilesCache[folderPath] : Directory.GetFiles(folderPath);

            try
            {
                foreach (var file in files)
                {
                    var match = re.Match(file);
                    if (match.Success)
                    {
                        found = true;
                        fileName = match.Value;
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
    }
}
