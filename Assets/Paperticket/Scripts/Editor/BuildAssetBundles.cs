using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class BuildAssetBundles : Editor {


    #region Menu items


    [MenuItem("Paperticket/Build Asset Bundles/Android")]
    public static void BuildAndroidBundle() {

        Debug.LogWarning("[BuildBundles] Building out Android asset bundles...");

        BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
        string assetBundleDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Paperticket Studios/PROJECTS/BUILDS/CarePlaysVR/Android Asset Bundles/";

        BuildBundles(buildAssetBundleOptions, assetBundleDirectory, BuildTarget.Android);
    
    }

    [MenuItem("Paperticket/Build Asset Bundles/PC")]
    public static void BuildPCBundle() {

        Debug.LogWarning("[BuildBundles] Building out PC asset bundles...");

        BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
        string assetBundleDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Paperticket Studios/PROJECTS/BUILDS/CarePlaysVR/PC Asset Bundles/"; 
        

        BuildBundles(buildAssetBundleOptions, assetBundleDirectory, BuildTarget.StandaloneWindows);

    }

    [MenuItem("Paperticket/Build Asset Bundles/Both")]
    public static void BuildBothBundles() {

        Debug.LogWarning("[BuildBundles] Building out both Android and PC asset bundles:");

        BuildAndroidBundle();
        BuildPCBundle();

    }




    [MenuItem("Paperticket/Build Asset Bundles/Open Bundle Directory...")]
    public static void OpenBundleDirectory() {

        string assetBundleDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Paperticket Studios/PROJECTS/BUILDS/CarePlaysVR/";

        assetBundleDirectory = assetBundleDirectory.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + assetBundleDirectory);

    }


    #endregion


    #region Internal functions

    public static void BuildBundles( BuildAssetBundleOptions buildAssetBundleOptions , string assetBundleDirectory, BuildTarget buildSettings ) {

        // Delete the directory above if it already exists
        if (Directory.Exists(assetBundleDirectory)) 
            Directory.Delete(assetBundleDirectory, true);

        // Create the directory above
        Directory.CreateDirectory(assetBundleDirectory);


        // Build the asset bundles into the above directory
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, buildAssetBundleOptions, buildSettings);// EditorUserBuildSettings.activeBuildTarget);

        RenameBundlesAsObb(assetBundleDirectory);


    }


    public static void RenameBundlesAsObb( string assetBundleDirectory ) {
        string filePath = "";

        // Iterate through every bundle
        foreach (string bundleName in AssetDatabase.GetAllAssetBundleNames()) {
            filePath = assetBundleDirectory + bundleName;

            if (File.Exists(filePath)) {

                // Rename main bundle to expansion file naming convention, otherwise just add .obb
                if (bundleName == "main") File.Move(filePath, assetBundleDirectory + "main." + PlayerSettings.Android.bundleVersionCode + "." + Application.identifier + ".obb");
                else File.Move(filePath, filePath + ".obb");

                Debug.Log("[BuildBundles] SUCCESS -> Expansion file should be saved as '" + filePath + ".obb'");

            } else {
                Debug.LogError("[BuildBundles] ERROR -> Could not find expansion path '" + filePath + "', cancelling remaining bundle builds!");
                return;
            }
        }

    }

    #endregion


    #region deprecated


    //[MenuItem("Paperticket/Build Asset Bundles/Rename main OBB for PC")]
    public static void RenameMainObbForPC() {

        string assetBundleDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Paperticket Studios/PROJECTS/BUILDS/CarePlaysVR/PC Asset Bundles/";

        string filePath = assetBundleDirectory + "main";
        if (File.Exists(filePath)) {
            File.Move(filePath, assetBundleDirectory + "main." + PlayerSettings.Android.bundleVersionCode + "." + Application.identifier + ".obb");
            Debug.LogWarning("[BuildBundles] SUCCESS -> Expansion file should be saved as '" + assetBundleDirectory + "main." + PlayerSettings.Android.bundleVersionCode + "." + Application.identifier + ".obb'");
        } else {
            Debug.LogError("[BuildBundles] ERROR -> Could not find '" + filePath + "'");
        }

    }

    //[MenuItem("Paperticket/Build Asset Bundles/Rename main OBB for Android")]
    public static void RenameMainObbForAndroid() {

        string assetBundleDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Paperticket Studios/PROJECTS/BUILDS/CarePlaysVR/Android Asset Bundles/";

        string filePath = assetBundleDirectory + "main";
        if (File.Exists(filePath)) {
            File.Move(filePath, assetBundleDirectory + "main." + PlayerSettings.Android.bundleVersionCode + "." + Application.identifier + ".obb");
            Debug.LogWarning("[BuildBundles] SUCCESS -> Expansion file should be saved as '" + assetBundleDirectory + "main." + PlayerSettings.Android.bundleVersionCode + "." + Application.identifier + ".obb'");
        } else {
            Debug.LogError("[BuildBundles] ERROR -> Could not find '" + filePath + "'");
        }

    }

    #endregion


}


//// Choose the bundle settings based on the platform

//if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {
//    // Use uncompressed asset bundle option on android to allow videos to be read
//    buildAssetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
//} else {
//    // Otherwise (PC presumably) use chunk based compression for smaller file size
//    buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
//}
