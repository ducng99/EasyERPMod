using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;

namespace EasyERPExplorer.Renderer
{
    public class Window : GameWindow
    {
        ImGuiController _controller;
        public static List<ImGuiDrawWindow> DrawWindows { get; private set; } = new();
        public static Window Instance { get; private set; }

        public Window() : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = new Vector2i(1600, 900), APIVersion = new Version(4, 5) })
        {
            Instance = this;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            string version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(2);
            Title = "EasyERPExplorer v" + version;

            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            // Update the opengl viewport
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            // Tell ImGui of the new size
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _controller.Update(this, (float)e.Time);

            GL.ClearColor(new Color4(10, 10, 10, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            foreach (var window in DrawWindows)
            {
                window.Draw();
            }

            _controller.Render();

            RenderUtils.CheckGLError("End of frame");

            SwapBuffers();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);


            _controller.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _controller.MouseScroll(e.Offset);
        }
    }
}
