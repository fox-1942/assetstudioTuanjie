using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEditor.OpenHarmony;

using AssetStudio;
using System;
using System.IO;

namespace AssetStudio
{

    public class BuildModifier : IPostGenerateOpenHarmonyProject//IPreprocessBuildWithReport, IProcessSceneWithReport, IPreprocessShaders, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostGenerateOpenHarmonyProject(string path)
        {

            AssetsManager assetsManager = new AssetsManager();
            //assetsManager.LoadFiles("D:\\Projects\\simplevulkanhap\\entry-default-signed\\resources\\rawfile\\Data\\tuanjie default resources");

            assetsManager.LoadFiles(path);

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
                        Console.WriteLine("- Asset converted.");
                    }
                }
            }

            Debug.Log("OH path: " + path);
        }

        //public void OnPreprocessBuild(BuildReport report)
        //{
        //    Debug.Log("Before Build");
        //}

        //public void OnProcessScene(Scene scene, BuildReport report)
        //{
        //    Debug.Log("Scene: " + scene.name);
        //}

        // public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        // {
        //     Debug.Log("Shader: " + shader.name);
        // }

        //public void OnPostprocessBuild(BuildReport report)
        //{
        //    Debug.Log("After Build");
        //}
    }
}