namespace EasyERPExplorer
{
    static class Utils
    {
        public static string ReplaceAllChars(this string str, char[] characters, char replaceWith)
        {
            string tmp = str;

            foreach (char c in characters)
            {
                tmp = tmp.Replace(c, replaceWith);
            }

            return tmp;
        }
    }
}
