
using UnityEngine;
using System.IO;

[RequireComponent(typeof(Camera))]
public class Screenshotter : MonoBehaviour {

    enum AALevel { none,x2,x4,x8 };


    [SerializeField] System.Environment.SpecialFolder editorBundleLocation = System.Environment.SpecialFolder.MyDocuments;
    [SerializeField] string subdirectory = "/Paperticket Studios/CarePlaysVR/";
    [Space(10)]
    //[SerializeField] RenderTexture outputMap = null;
    //[SerializeField] AALevel antiAliasing = AALevel.x2;
    [SerializeField] Vector2 textureSize = new Vector2(1920, 1080);
    [Space(10)]
    [SerializeField] TextureFormat textureFormat = TextureFormat.ARGB32;
    [SerializeField] bool linearSpace = true;



    void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            TakeScreenshot();
        }
    }


    public void TakeScreenshot() {

        // Create a new render texture (necessary for Linear space)
        RenderTexture outputMap = new RenderTexture(Mathf.FloorToInt(textureSize.x), Mathf.FloorToInt(textureSize.y), 32);
        outputMap.name = "ScreenshotterRenderTexture";
        outputMap.enableRandomWrite = true;
        outputMap.filterMode = FilterMode.Bilinear;
        //outputMap.antiAliasing = Math.Max(1, (int)Math.Pow(2, (int)antiAliasing));
        outputMap.Create();

        // Force a render to the target texture.
        GetComponent<Camera>().targetTexture = outputMap;
        GetComponent<Camera>().Render();

        // Texture.ReadPixels reads from whatever texture is active. Ours needs to
        // be active. But let's remember the old one so we can restore it later.
        RenderTexture oldRenderTexture = RenderTexture.active;
        RenderTexture.active = outputMap;

        // Grab ALL of the pixels.
        Texture2D raster = new Texture2D(outputMap.width, outputMap.height, textureFormat, false, linearSpace);
        raster.ReadPixels(new Rect(0, 0, outputMap.width, outputMap.height), 0, 0);
        raster.Apply();

        // Write them to disk. Change the path and type as you see fit.
        string filePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + subdirectory + "cpvrscreenshot" + System.DateTime.Now.ToString().Replace(" ","").Replace("/","").Replace(":","") + ".png";
        File.WriteAllBytes(filePath, raster.EncodeToPNG());

        // Restore previous settings.
        GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = oldRenderTexture;

        Debug.Log("[Screenshotter] Screenshot saved to '" + filePath + "'");
    }
}