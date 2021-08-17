using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ERPLoader
{
    class Utils
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
    }
}
