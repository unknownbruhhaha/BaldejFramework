using System.IO;

namespace GameEngineAPI
{
    public static class AssetManager
    {
        public static string AssetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\");
        
        public static void SaveAsset(string path, Asset asset, bool append = false)
        {
            using (Stream stream = File.Open(AssetsPath + path, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, asset);
            }
        }

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
