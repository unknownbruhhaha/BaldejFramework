using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using GameEngineAPI.UI.Elements;

namespace GameEngineAPI.UI
{
    public class UiRenderer
    {
        public Vector2i canvasSize;
        public GameWindow window;
        public List<UiElement> elements = new List<UiElement>();


        public UiRenderer(GameWindow window, Vector2i canvasSize)
        {
            this.window = window;
            this.canvasSize = canvasSize;
        }

        public void RenderUI()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                Console.WriteLine(elements[i]);
                elements[i].Render();
            }
        }
    }
}
