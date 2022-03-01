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

        public static List<RenderMeshData> meshes = new List<RenderMeshData>();
        public static List<RenderMeshData> meshes1 = new List<RenderMeshData>();

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
                //camera.AspectRatio = window.Size.X / (float)window.Size.Y;
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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int i = 0; i < meshes.Count; i++)
            {
                
                //prepairing data
                float[] verts = meshes[i].vertices;
                uint[] indices = meshes[i].indices;
                float[] texCoords = meshes[i].textureCoordinates;

                //fixing indices
                for (int j = 0; j < indices.Length; j++)
                {
                    indices[j] -= 1;
                }

                //prepairing shader
                Shader shader = meshes[i].shader;
                var model = Matrix4.Identity;
                shader.SetMatrix4("model", model);
                shader.SetMatrix4("view", camera.GetViewMatrix());
                shader.SetMatrix4("projection", camera.GetProjectionMatrix());

                //activating shader
                shader.Use();

                //creating OpenGL buffers and things like this yea
                int VAO; // Vertex Array Object - here we're storing all of our buffers
                int VBO; // Vertex Buffer Object - here we're storing vertices
                int EBO; // Element Buffer Object - here we're storing indices(faces)
                int TBO; // Texture Buffer Object (?) - here we're storing texture coordinates

                // creating a Vertex Buffer Object
                VBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);

                // Creating a Vertex Array Object
                VAO = GL.GenVertexArray();
                GL.BindVertexArray(VAO);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                // textures
                /// creating a Texture Buffer Object
                TBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, TBO);
                GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);

                // passing coordinates to a shader
                var texCoordLocation = shader.GetAttribLocation("vertTexCoord");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

                // creating Element Buffer Object
                EBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

                //finaly drawing mesh
                GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

                //cleaning up
                GL.BindVertexArray(0);
                GL.DeleteBuffer(VAO);
                GL.DeleteBuffer(VBO);
                GL.DeleteBuffer(EBO);
                GL.DeleteBuffer(TBO);
            }
            meshes.Clear();

            window.SwapBuffers();
        }
    }
}

