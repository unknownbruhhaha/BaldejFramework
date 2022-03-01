using System.Globalization;
using GameEngineAPI;
using GameEngineAPI.Render;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace GameEngineAPI.Components
{
    public class Mesh : Component
    {
        public string componentID { get => "Mesh"; }

        public GameObject? owner { get; set; }

        public List<MeshData> frames = new List<MeshData>();

        public int currentFrame;

        public bool animationLoop;
        public int animationStartFrame;
        public int animationEndFrame;

        public Texture tex;
        public Shader shader;

        public Mesh(string objPath, string objectName, string texturePath = "Palette.png", int animationStart = 1, int animationEnd = 1, string vertShaderPath = @"Shaders\standardVertShader.shader", string fragShaderPath = @"Shaders\standardFragShader.shader")
        {
            // creating texture
            //tex = new Render.Texture(texturePath);

            // reading .obj file
            string currentObjectName = "";

            for (int i = animationStart; i <= animationEnd; i++)
            {
                string iString = "_" + i.ToString().PadLeft(6, '0');
                string file = File.ReadAllText(Path.Combine(AssetManager.AssetsPath, objPath + iString + ".obj"));

                using (StringReader reader = new StringReader(file))
                {
                    string line;
                    List<VertData> tv = new List<VertData>();
                    List<VertData> tvt = new List<VertData>();
                    List<uint> tInd = new List<uint>();

                    shader = new Shader(Path.Combine(AssetManager.AssetsPath, vertShaderPath), Path.Combine(AssetManager.AssetsPath + fragShaderPath));

                    List<float> v = new List<float>();
                    List<uint> ind = new List<uint>();
                    List<float> tc = new List<float>();

                    while ((line = reader.ReadLine()) != null)
                    {
                        // getting current object
                        if (line.StartsWith("o "))
                        {
                            currentObjectName = line.Remove(0, 2);
                        }

                        // getting vertex pos
                        else if (line.StartsWith("v ") && currentObjectName == objectName)
                        {
                            VertData data = new VertData(
                                float.Parse(line.Remove(0, 2).Split(' ')[2], CultureInfo.InvariantCulture.NumberFormat),
                                float.Parse(line.Remove(0, 2).Split(' ')[1], CultureInfo.InvariantCulture.NumberFormat),
                                float.Parse(line.Remove(0, 2).Split(' ')[0], CultureInfo.InvariantCulture.NumberFormat)
                            );
                            tv.Add(data);
                        }
                        // getting vertex texture coords
                        else if (line.StartsWith("vt ") && currentObjectName == objectName)
                        {
                            VertData data = new VertData(
                                float.Parse(line.Remove(0, 3).Split(' ')[0], CultureInfo.InvariantCulture.NumberFormat),
                                float.Parse(line.Remove(0, 3).Split(' ')[1], CultureInfo.InvariantCulture.NumberFormat)
                            );
                            tvt.Add(data);
                        }

                        // getting indices
                        else if (line.StartsWith("f ") && currentObjectName == objectName)
                        {
                            // Getting indices and adding them to the main list
                            /// adding ind #1
                            int indVert1 = int.Parse(line.Remove(0, 2).Split(' ')[0].Split('/')[0], CultureInfo.InvariantCulture.NumberFormat); // getting current vert ind
                            ind.Add(Convert.ToUInt32(indVert1)); // adding it
                            int indTex1 = int.Parse(line.Remove(0, 2).Split(' ')[0].Split('/')[1], CultureInfo.InvariantCulture.NumberFormat); // getting current vert texture coords ind
                            /// adding ind #2
                            int indVert2 = int.Parse(line.Remove(0, 2).Split(' ')[1].Split('/')[0], CultureInfo.InvariantCulture.NumberFormat); // getting current vert ind
                            ind.Add(Convert.ToUInt32(indVert2)); // adding it
                            int indTex2 = int.Parse(line.Remove(0, 2).Split(' ')[1].Split('/')[1], CultureInfo.InvariantCulture.NumberFormat); // getting current vert texture coords ind
                            /// adding ind #3
                            int indVert3 = int.Parse(line.Remove(0, 2).Split(' ')[2].Split('/')[0], CultureInfo.InvariantCulture.NumberFormat); // getting current vert ind
                            ind.Add(Convert.ToUInt32(indVert3)); // adding it
                            int indTex3 = int.Parse(line.Remove(0, 2).Split(' ')[1].Split('/')[1], CultureInfo.InvariantCulture.NumberFormat); // getting current vert texture coords ind

                            // Adding verts to the main list
                            /// adding vert #1
                            v.Add(tv[indVert1 - 1].X);
                            v.Add(tv[indVert1 - 1].Y);
                            v.Add(tv[indVert1 - 1].Z);
                            /// adding vert #2
                            v.Add(tv[indVert2 - 1].X);
                            v.Add(tv[indVert2 - 1].Y);
                            v.Add(tv[indVert2 - 1].Z);
                            /// adding vert #3
                            v.Add(tv[indVert3 - 1].X);
                            v.Add(tv[indVert3 - 1].Y);
                            v.Add(tv[indVert3 - 1].Z);

                            //Adding texture coordinates to the main list
                            /// adding vert #1
                            tc.Add(tvt[indTex1 - 1].X);
                            tc.Add(tvt[indTex1 - 1].Y);
                            /// adding vert #2
                            tc.Add(tvt[indTex2 - 1].X);
                            tc.Add(tvt[indTex2 - 1].Y);
                            /// adding vert #3
                            tc.Add(tvt[indTex3 - 1].X);
                            tc.Add(tvt[indTex3 - 1].Y);
                        }
                    }

                    frames.Add(new MeshData(v.ToArray(), ind.ToArray(), tc.ToArray()));
                }
            }

            animationStartFrame = animationStart;
            animationEndFrame = animationEnd;

            Console.WriteLine("mesh component init complete!");
        }

        public void OnUpdate()
        {
            //animation
            if (currentFrame < animationEndFrame - 1)
            {
                currentFrame++;
            }
            if (currentFrame >= animationEndFrame - 1 && animationLoop)
            {
                currentFrame = animationStartFrame;
            }


            MeshData data = new MeshData(frames[currentFrame].vertices, frames[currentFrame].indices, frames[currentFrame].textureCoordinates);

            Transform transform = (Transform)owner.GetComponent("Transform");
            Vector3 pos = transform.Position;

            List<float> verts = new List<float>();
            uint[] indices = data.indices;
            float[] textureCoords = data.textureCoordinates;

            int coord = 0;

            for (int i = 0; i < data.vertices.Length; i++)
            {
                if (coord == 0)
                {
                    verts.Add(data.vertices[i] + pos.X);
                }
                else if (coord == 1)
                {
                    verts.Add(data.vertices[i] + pos.Y);
                }
                else if (coord == 2)
                {
                    verts.Add(data.vertices[i] + pos.Z);
                }

                coord++;

                if (coord >= 3)
                {
                    coord = 0;
                }
            }

            Render.Render.meshes.Add(new RenderMeshData(verts.ToArray(), indices.ToArray(), shader, textureCoords));
        }

        public void StartAnimation(int startFrame, int stopFrame, bool loop = true)
        {
            currentFrame = startFrame;
            animationStartFrame = startFrame;
            animationEndFrame = stopFrame;
            animationLoop = loop;
        }
    }
}
