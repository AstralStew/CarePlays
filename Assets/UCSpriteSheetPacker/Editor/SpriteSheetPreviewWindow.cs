using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace SpriteSheetProject
{
    public class AnimationData
    {
        public List<Editor> spriteEditors;
        public float startTime;
        public float stopTime;
        public int frameRate;
        public int offset;
        private Vector2 latestSize;
        private Dictionary<Editor, Texture> latestPreviewTextures;
        public AnimationData(AnimationClip animationClip)
        {
            var animationClipSettings = AnimationUtility.GetAnimationClipSettings(animationClip);
            spriteEditors = GetSpriteEditors(GetSprites(animationClip));
            startTime = animationClipSettings.startTime;
            stopTime = animationClipSettings.stopTime;
            frameRate = Mathf.FloorToInt(animationClip.frameRate);
        }
        public static Sprite[] GetSprites(AnimationClip animationClip)
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
                }
            }
            return sprites;
        }
        private List<Editor> GetSpriteEditors(params Sprite[] sprites)
        {
            var type = Assembly.Load("UnityEditor.dll").GetType("UnityEditor.SpriteInspector");
            var editors = new List<Editor>();
            foreach (var sprite in sprites)
            {
                Editor _editor = null;
                Editor.CreateCachedEditor(sprite, type, ref _editor);
                if (_editor != null)
                    editors.Add(_editor);
            }

            return editors;
        }
        public Texture GetPreviewTexture(Rect previewRect, Sprite sprite)
        {
            if (IsDirty(previewRect))
                RebuildPreviewTextures(previewRect);
            var spriteEditor = GetSpriteEditor(sprite);
            return latestPreviewTextures[spriteEditor];
        }
        private void RebuildPreviewTextures(Rect previewRect)
        {
            latestPreviewTextures = new Dictionary<Editor, Texture>(spriteEditors.Capacity);
            latestSize = new Vector2(previewRect.width, previewRect.height);
            for (int i = 0; i < spriteEditors.Count; i++)
            {
                var editor = spriteEditors[i];
                var previewTexture = editor.RenderStaticPreview("", null, (int)previewRect.width,
                    (int)previewRect.height);
                previewTexture.name = string.Format("({1,2}/{2,2}) {0}", editor.target.name, i + 1,
                    spriteEditors.Count);
                latestPreviewTextures.Add(editor, previewTexture);
            }
        }
        private bool IsDirty(Rect previewRect)
        {
            return !(Math.Abs(latestSize.x - previewRect.width) < 0.1 && Math.Abs(latestSize.y - previewRect.height) < 0.1);
        }

        private Editor GetSpriteEditor(Sprite sprite)
        {
            return spriteEditors.FirstOrDefault(e => e.target == sprite);
        }
    }
    public class SpriteSheetPreviewWindow : IDisposable
    {

        AnimationClip clip;
        AnimationData clipData;

        bool isPlaying;
        public static bool IsPlaying { get { return IsPlaying; } }
        Material defaultMaterial;
        float currentTime;
        double lastFrameEditorTime;
        static public int imitatedFPS;
        static public int actualFPS;
        public SpriteSheetPreviewWindow(AnimationClip clip)
        {
            this.clip = clip;
            this.clipData = new AnimationData(clip);
            defaultMaterial = new Material(Shader.Find("Sprites/Default"));
            imitatedFPS = (int)clip.frameRate;
            actualFPS = (int)clip.frameRate; 
            EditorApplication.update += Update;
            Play();
        }
        public void Dispose()
        {
            EditorApplication.update -= Update;
        }
        float GetCurrentTime(float startTime, float stopTime)
        {
            var _currentTime = Mathf.Repeat(currentTime, stopTime);
            _currentTime = Mathf.Clamp(_currentTime, startTime, stopTime);
            return _currentTime;
        }
        void Play()
        {
            isPlaying = true;
            lastFrameEditorTime = EditorApplication.timeSinceStartup;
        }
        void Pause()
        {
            isPlaying = false;
        }
        Sprite GetCurrentSprite(AnimationData d)
        {
            var currentIndex = Mathf.FloorToInt(GetCurrentTime(d.startTime, d.stopTime) * d.frameRate);
            currentIndex += d.offset;
            currentIndex = (int)Mathf.Repeat(currentIndex, d.spriteEditors.Count);
            return (Sprite)d.spriteEditors[currentIndex].target;
        }
        Texture GetCurrentPreviewTexture(Rect previewRect, AnimationClip target)
        {
            var currentSprite = GetCurrentSprite(clipData);
            return clipData.GetPreviewTexture(previewRect, currentSprite) ?? AssetPreview.GetAssetPreview(target);
        }
        void NextSprite()
        {
            clipData.offset++;
        }
        void PrevSprite()
        {
            clipData.offset--;
        }
        

        void Update()
        {
            if (isPlaying)
            {
                var timeSinceStartup = EditorApplication.timeSinceStartup;
                var deltaTime = timeSinceStartup - lastFrameEditorTime;
                lastFrameEditorTime = timeSinceStartup;
                if (actualFPS == -1)
                {
                    actualFPS = ConstantVariables.DEFAULT_FPS;
                }
                if (imitatedFPS == -1)
                {
                    imitatedFPS = ConstantVariables.DEFAULT_FPS;
                }
                currentTime += (float)deltaTime / actualFPS * imitatedFPS;
            }
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical("ShurikenEffectBg");
            DrawPreview();
            GUILayout.Space(ConstantVariables.WINDOW_BORDER);
            GUILayout.BeginHorizontal();
            //previous sprite
            if (GUILayout.Button(Contents.prevButtonContent, Contents.buttonStyle))
            {
                Pause();
                PrevSprite();
            }
            //"play-pause button"
            var playButtonContent = isPlaying ? Contents.pauseButtonContent : Contents.playButtonContent;
            EditorGUI.BeginChangeCheck();
            var m_isPlaying = GUILayout.Toggle(isPlaying, playButtonContent, Contents.buttonStyle);
            if (EditorGUI.EndChangeCheck())
            {
                if (m_isPlaying)
                    Play();
                else
                    Pause();
            }
            //next sprite
            if (GUILayout.Button(Contents.nextButtonContent, Contents.buttonStyle))
            {
                Pause();
                NextSprite();
            }
            GUILayout.EndHorizontal();
            DrawSlider();
            if (GUILayout.Button("Apply", Contents.applyButtonStyle) && actualFPS != imitatedFPS)
            {
                SpriteSheetWindow.OnApplyButton();
            }
            GUILayout.EndVertical();
        }
        void DrawPreview()
        {
            GUILayout.BeginVertical();
            var rect = new Rect(ConstantVariables.WINDOW_BORDER, ConstantVariables.WINDOW_BORDER + ConstantVariables.BUTTON_HEIGHT,
            ConstantVariables.WINDOW_MIN_WIDTH, ConstantVariables.WINDOW_MIN_WIDTH);
            GUILayout.Label(SpriteSheetWindow.GetAnimationName(), "SettingsHeader");
            GUILayout.Space(ConstantVariables.WINDOW_MIN_WIDTH);
            EditorGUI.DrawPreviewTexture(rect, GetCurrentPreviewTexture(rect, clip), defaultMaterial, ScaleMode.ScaleToFit);
            GUILayout.EndVertical();
        }
        void DrawSlider()
        {
            int minSliderValue = 1;
            int maxSliderValue = 240;
            int sliderLabelWidth = 70;
            int textFieldAlign = 5;
            GUILayout.BeginHorizontal("SettingsHeader");
            GUILayout.Label("Samples","SettingsHeader", GUILayout.Width(sliderLabelWidth));
            GUILayout.BeginVertical();
            GUILayout.Space(textFieldAlign);
            imitatedFPS = EditorGUILayout.IntSlider(imitatedFPS, minSliderValue, maxSliderValue);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}
