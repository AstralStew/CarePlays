using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;
using System.Linq;
namespace SpriteSheetProject
{
    public static class ConstantVariables
    {
        public const int BUTTON_HEIGHT = 40;
        public const int WINDOW_BORDER = 5;
        public const int APPLY_HEIGHT = BUTTON_HEIGHT / 2;
        public const int WINDOW_MIN_WIDTH = 256;
        public const int WINDOW_MIN_HEIGHT = WINDOW_MIN_WIDTH + (BUTTON_HEIGHT * 3) + APPLY_HEIGHT + (WINDOW_BORDER * 2);
        public const int DEFAULT_FPS = 60;
    }

    public static class Contents
    {
        public static GUIStyle buttonStyle = new GUIStyle("preButton");
        public static GUIStyle applyButtonStyle = new GUIStyle("preButton");
        public static GUIContent createOnlySS = new GUIContent(EditorGUIUtility.IconContent("MeshFilter Icon"));
        public static GUIContent createSSandA = new GUIContent(EditorGUIUtility.IconContent("CollabCreate Icon"));
        public static GUIContent openFiles = new GUIContent(EditorGUIUtility.IconContent("d_CollabChanges Icon"));
        public static GUIContent playButtonContent = EditorGUIUtility.IconContent("PlayButton");
        public static GUIContent pauseButtonContent = EditorGUIUtility.IconContent("PauseButton");
        public static GUIContent prevButtonContent = EditorGUIUtility.IconContent("Animation.PrevKey");
        public static GUIContent nextButtonContent = EditorGUIUtility.IconContent("Animation.NextKey");
    }

    public class SpriteSheetWindow : EditorWindow
    {
        static SpriteSheetWindow _instance;

        [MenuItem("Tools/UC Sprite Sheet Packer/Main Window", false, 0)]
        public static void ShowWindow()
        {
            _instance = (SpriteSheetWindow)GetWindow(typeof(SpriteSheetWindow), false, "Sprite Sheet Packer");
            _instance.minSize = new Vector2(ConstantVariables.WINDOW_MIN_WIDTH + (ConstantVariables.WINDOW_BORDER * 2), ConstantVariables.WINDOW_MIN_HEIGHT);
            _instance.Show();
        }
        [MenuItem("Tools/UC Sprite Sheet Packer/Create Animation and Sprite Sheet", false, 11)]
        public static void ClickedAnimAndSS()
        {
            if (_instance == null)
            {
                ShowWindow();
            }
            _instance.OnCreateAnimAndSS();
            _instance.OnEnable();
        }
        [MenuItem("Tools/UC Sprite Sheet Packer/Create Sprite Sheet", false, 12)]
        public static void ClickedSS()
        {
            CreateSpriteSheet();
        }
        [MenuItem("Tools/UC Sprite Sheet Packer/Open Animation File", false, 30)]
        public static void ClickedOpenAnim()
        {
            if (_instance == null)
            {
                ShowWindow();
            }
            _instance.OnOpenAnim();
            _instance.OnEnable();
        }
       
        SpriteSheetPreviewWindow previewWindow;
        public bool animationCreated;
        public bool animationOpened;
        public bool timeControlCreated;
        static bool pathNotSelected;
        static string animationPath;
        static string animationName;

        //main functions
        public static void CreateSpriteSheet()
        {
            var format = "png";
            var path = EditorUtility.OpenFolderPanel("Select Folder With Sprites", Application.dataPath, "");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            var fileName = Path.GetFileName(path);
            var saveFolder = EditorUtility.OpenFolderPanel("Select Save Folder", Application.dataPath, "");
            if (string.IsNullOrEmpty(saveFolder))
            {
                return;
            }
            var fileSheet = saveFolder + "/" + fileName + "." + format;
            if (File.Exists(fileSheet))
            {
                var option = EditorUtility.DisplayDialogComplex("Overwrite file " + fileName + "." + format + "?", "File " + fileName + "." + format + " already exists. Do you want to overwrite?", "Overwrite", "Skip", "Save with different name");
                switch (option)
                {
                    //overwrite
                    case 0:
                        SpriteSheetPacker.GenerateSS(path, fileSheet, fileName);
                        break;
                    //skip
                    case 1:
                        break;
                    //save with different name
                    case 2:
                        var i = 1;
                        var differentFileName = fileName + "_" + i + "." + format;
                        var differentFileSheet = saveFolder + "/" + differentFileName;
                        while (File.Exists(differentFileSheet))
                        {
                            i++;
                            differentFileName = fileName + "_" + i + "." + format;
                            differentFileSheet = saveFolder + "/" + differentFileName;
                        }
                        SpriteSheetPacker.GenerateSS(path, differentFileSheet, fileName);
                        Debug.Log("Your sprite sheet has been saved as " + differentFileName);
                        break;
                }
            }
            else
            {
                SpriteSheetPacker.GenerateSS(path, fileSheet, fileName);
            }
        }

        public static void CreateSpriteAnimation()
        {
            pathNotSelected = false;

            var format_1 = "png";
            var format_2 = "anim";
            var path = EditorUtility.OpenFolderPanel("Select Folder With Sprites", Application.dataPath, "");
            if (string.IsNullOrEmpty(path))
            {
                pathNotSelected = true;
                return;
            }
            var fileName = Path.GetFileName(path);
            var saveFolder = EditorUtility.OpenFolderPanel("Select Save Folder", Application.dataPath, "");
            if (string.IsNullOrEmpty(saveFolder))
            {
                return;
            }
            var fileSheet = saveFolder + "/" + fileName + "." + format_1;
            var fileAnimation = saveFolder + "/" + fileName + "." + format_2;

            if (File.Exists(fileSheet) || File.Exists(fileAnimation))
            {
                int option = 1;
                if (File.Exists(fileSheet))
                {
                    option = EditorUtility.DisplayDialogComplex("Overwrite file " + fileName + "." + format_1 + "?", "File " + fileName + "." + format_1 + " already exists. Do you want to overwrite?", "Overwrite", "Skip", "Save with different name");
                }
                else if (File.Exists(fileAnimation))
                {
                    option = EditorUtility.DisplayDialogComplex("Overwrite file " + fileName + "." + format_2 + "?", "File " + fileName + "." + format_2 + " already exists. Do you want to overwrite?", "Overwrite", "Skip", "Save with different name");
                }
                switch (option)
                {
                    //overwrite
                    case 0:
                        SpriteSheetPacker.GenerateSS(path, fileSheet, fileName);
                        SpriteSheetPacker.GenerateAnim(fileSheet, fileAnimation);
                        break;
                    //skip
                    case 1:
                        break;
                    //save with different name
                    case 2:
                        var i = 1;
                        var differentFileSheet = saveFolder + "/" + fileName + "_" + i + "." + format_1;
                        var differentFileAnimation = saveFolder + "/" + fileName + "_" + i + "." + format_2;
                        while (File.Exists(differentFileSheet) || File.Exists(differentFileAnimation))
                        {
                            i++;
                            differentFileSheet = saveFolder + "/" + fileName + "_" + i + "." + format_1;
                            differentFileAnimation = saveFolder + "/" + fileName + "_" + i + "." + format_2;
                        }
                        animationPath = differentFileAnimation;
                        SpriteSheetPacker.GenerateSS(path, differentFileSheet, fileName);
                        SpriteSheetPacker.GenerateAnim(differentFileSheet, differentFileAnimation);
                        Debug.Log("Your sprite sheet has been saved as " + fileName + i + "." + format_1);
                        Debug.Log("Your animation has been saved as " + fileName + i + "." + format_2);
                        break;
                }
            }
            else
            {
                animationPath = fileAnimation;
                SpriteSheetPacker.GenerateSS(path, fileSheet, fileName);
                SpriteSheetPacker.GenerateAnim(fileSheet, fileAnimation);
            }
        }
        static void ApplyFPS(string _animationPath, int fps)
        {
            var path = _animationPath.Replace(Application.dataPath, "Assets");
            var animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (animClip != null)
            {
                var editorCurveBinding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
                var objectReferenceKeyframes = AnimationUtility.GetObjectReferenceCurve(animClip, editorCurveBinding);
                if (objectReferenceKeyframes != null)
                {
                    var sprites = new Sprite[0];
                    sprites = objectReferenceKeyframes
                    .Select(objectReferenceKeyframe => objectReferenceKeyframe.value)
                    .OfType<Sprite>().ToArray();
                    animClip.frameRate = fps;
                    EditorCurveBinding spriteBinding = new EditorCurveBinding();
                    spriteBinding.type = typeof(SpriteRenderer);
                    spriteBinding.path = "";
                    spriteBinding.propertyName = "m_Sprite";
                    float deltaFrameTime = 1 / animClip.frameRate;
                    ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Length];
                    for (int i = 0; i < (sprites.Length); i++)
                    {
                        spriteKeyFrames[i] = new ObjectReferenceKeyframe();
                        spriteKeyFrames[i].time = i * deltaFrameTime;
                        spriteKeyFrames[i].value = sprites[i];
                    }
                    AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
        public static bool HasSprites(AnimationClip animationClip)
        {
            var sprites = new Sprite[0];
            if (animationClip != null)
            {
                var editorCurveBinding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
                var objectReferenceKeyframes = AnimationUtility.GetObjectReferenceCurve(animationClip, editorCurveBinding);
                if (objectReferenceKeyframes != null)
                {
                    sprites = objectReferenceKeyframes
                    .Select(objectReferenceKeyframe => objectReferenceKeyframe.value)
                    .OfType<Sprite>().ToArray();
                    if (sprites.Length != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static string GetAnimationName()
        {
            return animationName;
        }
        //event functions
        void OnEnable()
        {
            if (_instance == null)
                _instance = this;
            CreatePreviewWindow();
        }
        void CreatePreviewWindow()
        {
            if (animationPath != null)
            {
                var path = animationPath.Replace(Application.dataPath, "Assets");
                animationName = Path.GetFileName(path);
                var animation = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (HasSprites(animation))
                {
                    if (_instance.previewWindow != null)
                    {
                        _instance.previewWindow.Dispose();
                        previewWindow = null;
                    }
                    _instance.previewWindow = new SpriteSheetPreviewWindow(animation);
                    timeControlCreated = true;
                }
                else
                {
                    OnDisable();
                }
            }
        }
        void OnDisable()
        {
            previewWindow = null;
            timeControlCreated = false;
            animationCreated = false;
            animationOpened = false;
        }
        private void OnDestroy()
        {
            _instance = null;
        }
        void OnSelectionChange()
        {
            if(Selection.activeObject is AnimationClip)
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                var animation = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                string fullPath = path.Replace("Assets", Application.dataPath);
                var tempAnimationPath = path;
                animationName = Path.GetFileName(path);
                animationPath = tempAnimationPath.Replace("Assets", Application.dataPath);
                animationOpened = true;
                CreatePreviewWindow();
                Repaint();
            }
        }
        void OnCreateAnimAndSS()
        {
            if (animationCreated)
            {
                CreateSpriteAnimation();
                if (pathNotSelected == false)
                {
                    animationCreated = true;
                    OnEnable();
                }
            }
            else
            {
                CreateSpriteAnimation();
                if (pathNotSelected == false)
                {
                    animationCreated = true;
                    OnEnable();
                }
            }
        }
        void OnOpenAnim()
        {
            animationPath = EditorUtility.OpenFilePanel("Open Animation File", Application.dataPath, "anim");
            if (string.IsNullOrEmpty(animationPath))
                return;
            OnEnable();
            animationOpened = true;
        }
        public static void OnApplyButton()
        {
            if (!string.IsNullOrEmpty(animationPath))
            {
                ApplyFPS(animationPath, SpriteSheetPreviewWindow.imitatedFPS);
            }
        }

        //gui
        void OnGUI()
        {
            Contents.buttonStyle.fixedHeight = ConstantVariables.BUTTON_HEIGHT;
            Contents.applyButtonStyle.fixedHeight = ConstantVariables.APPLY_HEIGHT;
            Contents.createSSandA.tooltip = "Create Animation and Sprite Sheet";
            Contents.openFiles.tooltip = "Open Animation File";
            Contents.createOnlySS.tooltip = "Create Sprite Sheet Only";
            GUILayout.BeginHorizontal();
            //"Create Spritesheet and Animation button"
            if (GUILayout.Button(Contents.createSSandA, Contents.buttonStyle, GUILayout.Height(ConstantVariables.BUTTON_HEIGHT), GUILayout.Width((position.width) / 3)))
            {
                OnCreateAnimAndSS();
            }
            //"Open Animation button"
            if (GUILayout.Button(Contents.openFiles, Contents.buttonStyle, GUILayout.Height(ConstantVariables.BUTTON_HEIGHT), GUILayout.Width((position.width) / 3)))
            {
                OnOpenAnim();
            }
            //"Create Spritesheet Only button"
            if (GUILayout.Button(Contents.createOnlySS, Contents.buttonStyle, GUILayout.Height(ConstantVariables.BUTTON_HEIGHT), GUILayout.Width((position.width) / 3 + 1)))
            {
                CreateSpriteSheet();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (previewWindow != null)
            {
                previewWindow.OnGUI();
                Repaint();
            }
            GUILayout.EndHorizontal();
        }
    }
}
