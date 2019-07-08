using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle  {

#if UNITY_EDITOR

    [MenuItem("Advanture/BuildAsset Bundle/Win")]
    static void BuildABsWin()
    {
        SwitchPlatfform(RuntimePlatform.WindowsEditor);
        if (!Directory.Exists("AssetBundles/Win"))
            Directory.CreateDirectory("AssetBundles/Win");
        BuildPipeline.BuildAssetBundles("AssetBundles/Win", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
    [MenuItem("Advanture/BuildAsset Bundle/MacOS")]
    static void BuildABsMacOS()
    {
        SwitchPlatfform(RuntimePlatform.WindowsEditor);
        if (!Directory.Exists("AssetBundles/MacOS"))
            Directory.CreateDirectory("AssetBundles/MacOS");
        BuildPipeline.BuildAssetBundles("AssetBundles/MacOS", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
    }

    [MenuItem("Advanture/BuildAsset Bundle/WebGL")]
    static void BuildABsWeb()
    {
        SwitchPlatfform(RuntimePlatform.WebGLPlayer);
        BuildPipeline.BuildAssetBundles("AssetBundles/WebGL", BuildAssetBundleOptions.None, BuildTarget.WebGL);
    }

    [MenuItem("Advanture/BuildAsset Bundle/Android")]
    static void BuildABsAndroid()
    {
        SwitchPlatfform(RuntimePlatform.Android);
        if (!Directory.Exists("AssetBundles/Android"))
            Directory.CreateDirectory("AssetBundles/Android");
        BuildPipeline.BuildAssetBundles("AssetBundles/Android", BuildAssetBundleOptions.None, BuildTarget.Android);
    }
    [MenuItem("Advanture/BuildAsset Bundle/IOS")]
    static void BuildABsIOS()
    {
        SwitchPlatfform(RuntimePlatform.IPhonePlayer);
        if (!Directory.Exists("AssetBundles/IOS"))
            Directory.CreateDirectory("AssetBundles/IOS");
        BuildPipeline.BuildAssetBundles("AssetBundles/IOS", BuildAssetBundleOptions.None, BuildTarget.iOS);
    }

    static void SwitchPlatfform(RuntimePlatform platform)
    {
        if(Application.platform == platform)
            return;

        switch (platform)
        {
            case RuntimePlatform.Android:
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.Android);
                break;
            case RuntimePlatform.WindowsEditor:
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                break;
            case RuntimePlatform.OSXEditor:
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
                break;
            case RuntimePlatform.WebGLPlayer:
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.WebGL);
                break;
            case RuntimePlatform.IPhonePlayer:
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.iOS);
                break;
        }
    }

#endif
}
