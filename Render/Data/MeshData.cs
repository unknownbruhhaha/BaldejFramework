namespace GameEngineAPI.Render
{
    public class MeshData
    {
        public float[] vertices;
        public uint[] indices;
        public float[] textureCoordinates;

        public MeshData(float[] verts, uint[] ind, float[] texCoords)
        {
            vertices = verts;
            indices = ind;
            textureCoordinates = texCoords;
        }
    }
}
