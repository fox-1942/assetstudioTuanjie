using AssetStudio;
using System;
using System.IO;

namespace AssetStudio
{
    static class Program
    {

        static void Main(string[] args)
        {
            AssetsManager assetsManager = new AssetsManager();
            assetsManager.LoadFiles("D:\\Projects\\simplevulkanhap\\entry-default-signed\\resources\\rawfile\\Data\\tuanjie default resources");

            foreach (SerializedFile file in assetsManager.assetsFileList)
            {
                Console.WriteLine("File asset name: " + file.fullName + "\nTuanjie Version: " + file.unityVersion);
                Console.WriteLine("Object count: " + file.Objects.Count);

                foreach (AssetStudio.Object asset in file.Objects)
                {
                    // Going for the assets containing shaders
                    if (asset is Shader shader)
                    {
                        ShaderConverter.Convert(shader);
                        Console.WriteLine("Asset converted.");
                    }
                }
            }
        }
    }
}
