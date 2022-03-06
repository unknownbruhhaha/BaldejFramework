using GameEngineAPI.Render;
using System.Globalization;

namespace GameEngineAPI.Assets
{
    public class ObjMeshAsset : Asset
    {
        Shader meshShader;
        public List<MeshData> Frames = new List<MeshData>();
        public List<AnimationData> Animations = new List<AnimationData>();
        public Shader shader;
        public string AssetType { get => "ObjModelAsset"; }

        public string AssetShortName { get; set; }

        public ObjMeshAsset(string path, string assetShortName, string objectName, int firstFileIndex, int lastFileIndex, List<AnimationData> animations, string vertShaderPath = @"Shaders\standardVertShader.shader", string fragShaderPath = @"Shaders\standardFragShader.shader")
        {
            shader = new Shader(vertShaderPath, fragShaderPath);
            string currentObjectName = "";

            for (int i = firstFileIndex; i <= lastFileIndex; i++)
            {
                string iString = "_" + i.ToString().PadLeft(6, '0');
                string file = File.ReadAllText(Path.Combine(AssetManager.AssetsPath, path + iString + ".obj"));

                using (StringReader reader = new StringReader(file))
                {
                    string line;
                    List<VertData> tv = new List<VertData>();
                    List<VertData> tvt = new List<VertData>();
                    List<uint> tInd = new List<uint>();

                    meshShader = new Shader(Path.Combine(AssetManager.AssetsPath, vertShaderPath), Path.Combine(AssetManager.AssetsPath + fragShaderPath));

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

                    Frames.Add(new MeshData(v.ToArray(), ind.ToArray(), tc.ToArray()));

                    Animations = animations;
                    AssetShortName = assetShortName;
                }
            }
        }

        public AnimationData? GetAnimationByName(string name)
        {
            for (int i = 0; i < Animations.Count; i++)
            {
                if (Animations[i].AnimationName == name)
                {
                    return Animations[i];
                }
            }

            return null;
        }
    }
}
