// C# Example
// Builds an asset bundle from the selected objects in the project view.
// Once compiled go to "Menu" -> "Assets" and select one of the choices
// to build the Asset Bundle

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ExportAssetBundles {
    [MenuItem("Assets/资源打包")]//(包含依耐关系)
    static void ExportResource () {
        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        //build.assetBundleName = "Comm_Altas.png";
        //build.assetNames = new string[] { "Assets/Art/UI/Comm/Comm_Altas.png" };
        //builds.Add(build);
        //build = new AssetBundleBuild();
        //build.assetBundleName = "Share_Altas.png";
        //build.assetNames = new string[] { "Assets/Art/UI/Share/Share_Altas.png" };
        //builds.Add(build);
        //build = new AssetBundleBuild();
        //build.assetBundleName = "Setting_Altas.png";
        //build.assetNames = new string[] { "Assets/Art/UI/Setting/Setting_Altas.png" };
        //builds.Add(build);
        //build = new AssetBundleBuild();
        //build.assetBundleName = "msyh.ttf";
        //build.assetNames = new string[] { "Assets/Resources/Fonts/msyh.ttf" };
        //builds.Add(build);
        //build = new AssetBundleBuild();
        //build.assetBundleName = "SettingView.prefab";
        //build.assetNames = new string[] { "Assets/Resources/SettingView.prefab"};
        //builds.Add(build);
        string [] depends = AssetDatabase.GetDependencies("Assets/Resources/ShareView.prefab");
        for (int i = 0; i < depends.Length; i++)
        {
            var build = new AssetBundleBuild();
            string[] paths = depends[i].Split('/');
            build.assetBundleName = paths[paths.Length - 1];
            string[] names = { depends[i] };
            build.assetNames = names;
            builds.Add(build);
        }
        //build = new AssetBundleBuild();
        //build.assetBundleName = "ShareView.prefab";
        //build.assetNames = new string[] { "Assets/Resources/ShareView.prefab" };
        //builds.Add(build);
        BuildPipeline.BuildAssetBundles(@"Assets/StreamingAssets", builds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
        Debug.Log("打包完成");
    }

    [MenuItem("Test/BuildBundleSingle")]
    public static void BuildBundleSingle()
    {
        string dataPath = Application.dataPath;
        int startIndex = dataPath.LastIndexOf("/Assets");
        if (startIndex != -1)
        {
            dataPath = dataPath.Remove(startIndex);
        }
        dataPath = dataPath + "/TestA/";

        List<string> listObjects = new List<string>();
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            string FullPath = AssetDatabase.GetAssetPath(o);
            listObjects.Add(FullPath);
        }

        string[] names = AssetDatabase.GetDependencies(listObjects.ToArray());
        AssetBundleBuild[] builds = new AssetBundleBuild[1];
        builds[0].assetBundleName = "a.unity3d";
        builds[0].assetNames = names;
        BuildPipeline.BuildAssetBundles(dataPath, builds, BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.iOS);
    }

    [MenuItem("Test/BuildBundleSeperate")]
    public static void BuildBundleSeperate()
    {
        string dataPath = Application.dataPath;
        int startIndex = dataPath.LastIndexOf("/Assets");
        if (startIndex != -1)
        {
            dataPath = dataPath.Remove(startIndex);
        }
        dataPath = dataPath + "/TestB/";

        List<string> listObjects = new List<string>();
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            string FullPath = AssetDatabase.GetAssetPath(o);
            listObjects.Add(FullPath);
        }

        string[] names = AssetDatabase.GetDependencies(listObjects.ToArray());
        AssetBundleBuild[] builds = new AssetBundleBuild[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            builds[i].assetBundleName = i + ".unity3d";
            string[] a = { names[i] };
            builds[i].assetNames = a;
        }
        BuildPipeline.BuildAssetBundles(dataPath, builds, BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.iOS);
    }
}