using System.Numerics;

namespace EasyERPExplorer.Renderer
{
    public abstract class ImGuiDrawWindow
    {
        public static readonly Vector2 Padding = new(10, 10);
        public bool IsOpen = true;

        public abstract void Draw();
    }
}
