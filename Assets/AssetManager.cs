using System.IO;

namespace GameEngineAPI.Assets
{
    public static class AssetManager
    {
        public static string AssetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\");
        
        public static List<Asset> Assets = new List<Asset>();

        public static Asset LoadAsset(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (Asset)binaryFormatter.Deserialize(stream);
            }
        }
    }
}
