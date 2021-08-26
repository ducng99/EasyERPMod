using System.Drawing;

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

        public static byte[] ImgToBytes(Bitmap img)
        {
            System.Collections.Generic.List<byte> bytes = new();
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    var color = img.GetPixel(x, y);
                    bytes.Add(color.R);
                    bytes.Add(color.G);
                    bytes.Add(color.B);
                    bytes.Add(color.A);
                }
            }
            
            return bytes.ToArray();
        }
    }
}
