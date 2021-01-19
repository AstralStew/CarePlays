using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class SphereCapture : EditorWindow {
       
    [MenuItem("Paperticket/360 Sphere Capture")]
    static void CaptureSphere() {
                
        // Capture the image and save to a byte array
        byte[] byteArray = I360Render.Capture( 1920, false, null, true );


        // Create the following directory if it does not yet exist
        string outputDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Paperticket Studios/CarePlaysVR/Unity 360 Captures/";
        if (!Directory.Exists(outputDirectory)) {
            Directory.CreateDirectory(outputDirectory);
        }

        // Save the image as a png
        string filePath = outputDirectory + "newfile" ;
        File.WriteAllBytes(filePath, byteArray);


        if (File.Exists(filePath)) {
            File.Move(filePath, outputDirectory + "360UnityCapture_" + System.DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss") + ".png");
            Debug.Log("[SphereCapture] SUCCESS -> Expansion file should be saved as '" + outputDirectory + "360UnityCapture_" + System.DateTime.Now + ".png" + "'");
        } else {
            Debug.LogError("[SphereCapture] ERROR -> Could not find '" + filePath + "'");
        }

    }

}