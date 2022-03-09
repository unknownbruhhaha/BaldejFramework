using GameEngineAPI;
using GameEngineAPI.Components;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace GameEngineAPI.Render
{
    public static class Render
    {
        public static GameWindow window;
        public static Camera camera;
        public static float[] SkyColor = { 0.5f, 0.9f, 1f };

        public static float deltaTime;

        public static object game;
        public static Type gameType;

        public static void RunRender()
        {
            // creating window
            NativeWindowSettings nws = new NativeWindowSettings();
            nws.Title = "123";
            window = new GameWindow(GameWindowSettings.Default, nws);

            // first we run Load func
            window.Load += Window_Load;

            // lambda func for Resize
            window.Resize += (ResizeEventArgs obj) =>
            {
                GL.Viewport(0, 0, obj.Width, obj.Height);
                camera.AspectRatio = window.Size.X / (float)window.Size.Y;
            };

            // then Update, then we starting Render
            window.UpdateFrame += Window_UpdateFrame;
            window.RenderFrame += Window_RenderFrame;

            // let's start our window! 
            window.Run();
        }

        private static void Window_Load()
        {
            GL.Enable(EnableCap.Texture2D);
            camera = new Camera(Vector3.UnitZ * 3, window.Size.X / (float)window.Size.Y);
            gameType.GetMethod("Start").Invoke(game, null);
        }

        private static void Window_UpdateFrame(FrameEventArgs obj)
        {
            SceneManager.Update();
            Phys.Update();
            deltaTime = float.Parse(obj.Time.ToString());
            gameType.GetMethod("Update").Invoke(game, Array.Empty<object>());
        }

        private unsafe static void Window_RenderFrame(FrameEventArgs obj)
        {
            GL.ClearColor(SkyColor[0], SkyColor[1], SkyColor[2], 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            SceneManager.Render();

            window.SwapBuffers();
        }
    }
}

