using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildAssetBundles : Editor {


    [MenuItem("Paperticket/Build Asset Bundles")]
    public static void BuildBundles() {

        BuildAssetBundleOptions buildAssetBundleOptions;
        string assetBundleDirectory = null;

        /// ANDROID BUILD TARGET
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {

            // Define (or create) a path for the asset bundles to build into
            assetBundleDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Paperticket Studios/PROJECTS/BUILDS/CarePlaysVR/Android Asset Bundles/";
            //assetBundleDirectory = "D:/Paperticket Studios/PROJECTS/BUILDS/CarePlaysVR/APKs/Asset Bundles/";

            // Use uncompressed asset bundle option on android to allow videos to be read
            buildAssetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;

        /// PC BUILD TARGET
        } else {

            // Define (or create) a path for the asset bundles to build into
            assetBundleDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Paperticket Studios/PROJECTS/BUILDS/CarePlaysVR/PC Asset Bundles/";
            //assetBundleDirectory = "D:/Paperticket Studios/PROJECTS/BUILDS/CarePlaysVR/EXEs/Asset Bundles/";

            // Otherwise (PC presumably) use chunk based compression for smaller file size
            buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;

        }

        // Make sure the directory above exists
        if (!System.IO.Directory.Exists(assetBundleDirectory)) {
            System.IO.Directory.CreateDirectory(assetBundleDirectory);
        }
        

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
