namespace GameEngineAPI.UI.Elements
{
    public abstract class UiElement
    {
        public int VertexBufferObject;

        public float[] vertices;

        public abstract void Render();
    };
}
