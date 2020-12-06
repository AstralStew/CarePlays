using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildAssetBundles : Editor {

    [MenuItem("Paperticket/Build Asset Bundles/Android")]
    public static void BuildAndroidBundle() {

        Debug.LogWarning("[BuildBundles] Building out Android asset bundles...");

        BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
        string assetBundleDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Paperticket Studios/PROJECTS/BUILDS/CarePlaysVR/Android Asset Bundles/";

        BuildBundles(buildAssetBundleOptions, assetBundleDirectory);
    
    }

    [MenuItem("Paperticket/Build Asset Bundles/PC")]
    public static void BuildPCBundle() {

        Debug.LogWarning("[BuildBundles] Building out PC asset bundles...");

        BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
        string assetBundleDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Paperticket Studios/PROJECTS/BUILDS/CarePlaysVR/PC Asset Bundles/"; 

        BuildBundles(buildAssetBundleOptions, assetBundleDirectory);

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

    //[MenuItem("Paperticket/Build Asset Bundles")]
    public static void BuildBundles( BuildAssetBundleOptions buildAssetBundleOptions , string assetBundleDirectory ) {

        // Delete the directory above if it already exists
        if (System.IO.Directory.Exists(assetBundleDirectory)) 
            System.IO.Directory.Delete(assetBundleDirectory, true);

        // Create the directory above
        System.IO.Directory.CreateDirectory(assetBundleDirectory);


        // Build the asset bundles into the above directory
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, buildAssetBundleOptions, EditorUserBuildSettings.activeBuildTarget);

        // Find the file and rename it
        string filePath = assetBundleDirectory + "videos";
        if (System.IO.File.Exists(filePath)) {
            //System.IO.File.Move(filePath, assetBundleDirectory + "main." + PlayerSettings.Android.bundleVersionCode + ".com.StudioBento.RONE.obb");
            System.IO.File.Move(filePath, assetBundleDirectory + "main." + PlayerSettings.Android.bundleVersionCode + "."+ Application.identifier + ".obb");
            Debug.LogWarning("[BuildBundles] SUCCESS -> Expansion file should be saved as '" + assetBundleDirectory + "main." + PlayerSettings.Android.bundleVersionCode + "." + Application.identifier + ".obb'");
        } else {            
            Debug.LogError("[BuildBundles] ERROR -> Could not find '" + filePath + "'");
        }

    }


}


//// Choose the bundle settings based on the platform

//if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {
//    // Use uncompressed asset bundle option on android to allow videos to be read
//    buildAssetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
//} else {
//    // Otherwise (PC presumably) use chunk based compression for smaller file size
//    buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
//}
