using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;
namespace SpriteSheetProject
{
    public class SpriteSheetPacker : MonoBehaviour
    {
        public static void GenerateSS(string path, string pathSave, string spriteName)
        {
            GenerateSpriteSheet(path, pathSave, spriteName);
        }
        public static void GenerateAnim()
        {
            GenerateSpriteAnimation();
        }
        public static void GenerateAnim(string pathSpriteSave, string pathAnimationSave)
        {
            GenerateSpriteAnimation(pathSpriteSave, pathAnimationSave);
        }
        public static void GenerateAnim(string pathSpriteSave, string pathAnimationSave, int animFPS)
        {
            GenerateSpriteAnimation(pathSpriteSave, pathAnimationSave, animFPS);
        }

        private static void GenerateSpriteSheet(string path, string pathSave, string spriteName)
        {
            var files = Directory.GetFiles(path, "*.png");
            if (files == null || files.Length == 0) return;
            var textures = new Texture2D[files.Length];
            for (int i = 0; i < textures.Length; i++)
                textures[i] = ImportTexture(files[i]);
            if (textures == null || textures.Length == 0) return;
            var spriteSize = new Vector2(textures[0].width, textures[0].height);
            int sqrLength = (int)Mathf.Sqrt(textures.Length);
            int columnRelation = (int)(spriteSize.y / spriteSize.x);
            int rowRelation = (int)(spriteSize.x / spriteSize.y);
            int columnCount = 0;
            int rowCount = 0;
            if (columnRelation > rowRelation)
            {
                columnCount = Mathf.RoundToInt(columnRelation * sqrLength);
                rowCount = (int)(textures.Length / (float)columnCount) + 1;
            }
            else
            {
                rowCount = Mathf.RoundToInt(rowRelation * sqrLength);
                columnCount = (int)(textures.Length / (float)rowCount) + 1;
            }
            if (rowCount * columnCount < textures.Length)
            {
                if (rowCount >= columnCount)
                    rowCount++;
                else
                    columnCount++;
            }
            var resultTexture = new Texture2D((int)spriteSize.x * columnCount, (int)spriteSize.y * rowCount);
            List<SpriteMetaData> spriteMetaDatas = new List<SpriteMetaData>();
            for (int indexYSprite = 0; indexYSprite < rowCount; indexYSprite++)
            {
                for (int indexXSprite = 0; indexXSprite < columnCount; indexXSprite++)
                {
                    int indexSprite = indexYSprite * columnCount + indexXSprite;
                    int x = (int)(indexXSprite * spriteSize.x);
                    int y = (int)(indexYSprite * spriteSize.y);
                    Color[] pixels = null;
                    if (indexSprite < textures.Length)
                    {
                        pixels = textures[indexSprite].GetPixels();
                        SpriteMetaData smd = new SpriteMetaData();
                        smd.pivot = new Vector2(0.5f, 0.5f);
                        smd.alignment = 9;
                        smd.name = spriteName + "_" + indexSprite;
                        smd.rect = new Rect(x, y, spriteSize.x, spriteSize.y);

                        spriteMetaDatas.Add(smd);
                    }
                    else
                    {
                        pixels = new Color[(int)(spriteSize.x * spriteSize.y)];
                    }
                    resultTexture.SetPixels(x, y, (int)spriteSize.x, (int)spriteSize.y, pixels);
                }
            }
            resultTexture.Apply();
            var bytes = resultTexture.EncodeToPNG();
            File.WriteAllBytes(pathSave, bytes);
            AssetDatabase.Refresh();
            var assetPath = pathSave.Replace(Application.dataPath, "Assets");
            var assetTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            var ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            ti.isReadable = true;
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Multiple;
            ti.spritesheet = spriteMetaDatas.ToArray();
            ti.SaveAndReimport();
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }
        public static Texture2D ImportTexture(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(64, 64, TextureFormat.ARGB32, false);
            texture.LoadImage(data);
            return texture;
        }

        [MenuItem("Assets/Create animation", true)]
        private static bool GenerateSpriteAnimationValidation()
        {
            return Selection.activeObject is Texture2D;
        }
        [MenuItem("Assets/Create animation")]
        private static void GenerateSpriteAnimation()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var animationPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + "_animation.anim");
            GenerateSpriteAnimation(path, animationPath);
        }
        static void GenerateSpriteAnimation(string pathSpriteSave, string pathAnimationSave)
        {
            GenerateSpriteAnimation(pathSpriteSave, pathAnimationSave, ConstantVariables.DEFAULT_FPS);
        }
        static void GenerateSpriteAnimation(string pathSpriteSave, string pathAnimationSave, int animFPS)
        {
            var spritesPath = pathSpriteSave.Replace(Application.dataPath, "Assets");
            var sourceSprites = AssetDatabase.LoadAllAssetsAtPath(spritesPath);
            if (sourceSprites.Length < 2)
                return;

            var sprites = new Sprite[sourceSprites.Length - 1];
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = (Sprite)sourceSprites[i + 1];
            }
            AnimationClip animClip = new AnimationClip();
            animClip.frameRate = animFPS;
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
            var animationPath = pathAnimationSave.Replace(Application.dataPath, "Assets");
            AssetDatabase.CreateAsset(animClip, animationPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}