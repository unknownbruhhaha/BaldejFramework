using System.Globalization;

using GameEngineAPI.Assets;
using GameEngineAPI.Render;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace GameEngineAPI.Components
{
    public class Mesh : Component
    {
        //engine stuff
        public string componentID { get => "Mesh"; }
        public GameObject? owner { get; set; }

        public ObjMeshAsset mesh;

        //animation stuff
        public int currentFrame;
        public bool animationLoop;
        public int animationStartFrame;
        public int animationEndFrame;

        int lastFrame; //temp variable, contains last animation frame to check if currentFrame changed

        //openGL stuff
        int VBO;
        int VAO;
        int EBO;
        int TBO;

        public Texture tex;
        public Shader shader;

        public Mesh(ObjMeshAsset meshAsset, TextureAsset? textureAsset = null, string startAnimation = "Idle", bool animationLoop = false)
        {
            mesh = meshAsset;
            shader = mesh.shader;
            AnimationData animationData = mesh.GetAnimationByName(startAnimation);
            List<MeshData> frames = mesh.Frames;

            // Creating a Vertex Array Object
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            
            // creating a Vertex Buffer Object
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, frames[animationData.AnimationStartFrame].vertices.Length * sizeof(float), frames[animationData.AnimationStartFrame].vertices, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // textures
            /// creating a Texture Buffer Object
            TBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, TBO);
            GL.BufferData(BufferTarget.ArrayBuffer, frames[animationData.AnimationStartFrame].textureCoordinates.Length * sizeof(float), frames[animationData.AnimationStartFrame].textureCoordinates, BufferUsageHint.DynamicDraw);

            // passing coordinates to a shader
            int texCoordLocation = shader.GetAttribLocation("vertTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            // creating Element Buffer Object
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, frames[animationData.AnimationStartFrame].indices.Length * sizeof(uint), frames[animationData.AnimationStartFrame].indices, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            Console.WriteLine("mesh component init complete!");
        }

        public void OnUpdate()
        {
            MeshData currentData = mesh.Frames[currentFrame];

            //playing our animation
            if (currentFrame < animationEndFrame - 1)
            {
                currentFrame++;
                //updating current frame ref
                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, currentData.vertices.Length, currentData.vertices, BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
            }
            if (currentFrame >= animationEndFrame - 1 && animationLoop)
            {
                currentFrame = animationStartFrame;
                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, currentData.vertices.Length, currentData.vertices, BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
            }

            //moving vertices depending on transform component
            Transform transform = (Transform)owner.GetComponent("Transform");
            Vector3 pos = transform.Position; // getting pos from transform component
            //shader.SetVector3("vertPosition", pos); // and moving it to the shader

            // passing camera data to the shader
            var model = Matrix4.Identity;
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", Render.Render.camera.GetViewMatrix());
            shader.SetMatrix4("projection", Render.Render.camera.GetProjectionMatrix());

            /*int coord = 0; //old version
            for (int i = 0; i < mesh.Frames[currentFrame].vertices.Length; i++)
            {
                if (coord == 0) verts[i] = mesh.Frames[currentFrame].vertices[i] + pos.X;
                if (coord == 1) verts[i] = mesh.Frames[currentFrame].vertices[i] + pos.Y;
                if (coord == 2) verts[i] = mesh.Frames[currentFrame].vertices[i] + pos.Z;

                coord++;
                if (coord > 2) coord = 0;
            }*/

            lastFrame = currentFrame;
        }

        void Draw()
        {
            shader.Use();
            GL.BindVertexArray(VAO);

            //Console.WriteLine(mesh.Frames[currentFrame].indices.Length);

            GL.DrawElements(BeginMode.Triangles, mesh.Frames[currentFrame].indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void StartAnimation(int startFrame, int stopFrame, bool loop = true)
        {
            currentFrame = startFrame;
            animationStartFrame = startFrame;
            animationEndFrame = stopFrame;
            animationLoop = loop;
        }

        public void OnRender()
        {
            Draw();
        }
    }
}
