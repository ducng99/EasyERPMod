using EasyERPExplorer.Renderer;
using ERPLoader;

namespace EasyERPExplorer
{
    class Program
    {
        public static Settings EasyERPSettings = Settings.InitSettings();

        static void Main()
        {
            Window wnd = new Window();
            wnd.Run();
        }
    }
}
