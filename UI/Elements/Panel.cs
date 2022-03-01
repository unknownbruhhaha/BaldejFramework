using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace GameEngineAPI.UI.Elements
{
    public class Panel : UiElement
    {

        // rectangle panel vertices
        public float[] vertices =
        {
            -0.5f, 0.5f, 10.0f,
            -0.5f, -0.5f, 10.0f,
             0.5f,  0.5f, 10.0f,
             0.5f,  -0.5f, 10.0f
        };

        public string Shader =
            @"version 400

            out vec4 pixelColor;

            void main()
            {
	            pixelColor = vec4(1, 1, 1, 1);
            }";

        public int vertexBufferHandle;

        public override void Render()
        {

        }

        public Panel(UiRenderer renderer, Vector2 size, Vector2i position, String pixelShader = "")
        {

            // calculating & setting vertices position using size
            vertices[0] = size.X / -2;
            vertices[1] = size.Y / 2;
            vertices[2] = 0;

            vertices[3] = size.X / -2;
            vertices[4] = size.Y / -2;
            vertices[5] = 0;

            vertices[6] = size.X / 2;
            vertices[7] = size.Y / 2;
            vertices[8] = 0;

            vertices[9] = size.X / 2;
            vertices[10] = size.Y / -2;
            vertices[11] = 0;

            // setting pixelShader
            if (pixelShader != "")
            {
                Shader = pixelShader;
            }

            // adding to ui elements list

            renderer.elements.Add(this);
        }
    }
}
