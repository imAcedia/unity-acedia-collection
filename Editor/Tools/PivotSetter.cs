using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace AcediaEditor
{
    /// TODO PivotSetter: Documentations

    public class PivotSetter : EditorWindow
    {
        private Vector2 newPivot;
        private bool xInPixel = false;
        private bool yInPixel = false;
        private Texture2D[] selectedSprites;

        private Vector2 scroll;

        [MenuItem("Tools/Acedia/Pivot Setter")]
        public static void ShowWindow()
        {
            PivotSetter setter = GetWindow<PivotSetter>("Pivot Setter");
            setter.selectedSprites = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
            setter.SortSelectionByName();

            foreach (Texture2D selectedSprite in setter.selectedSprites)
            {
                TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(selectedSprite)) as TextureImporter;
                if (importer != null && importer.textureType == TextureImporterType.Sprite)
                {
                    setter.newPivot =
                        importer.spriteImportMode == SpriteImportMode.Multiple ?
                        importer.spritesheet[0].pivot :
                        importer.spritePivot;

                    break;
                }
            }
        }

        private void OnSelectionChange()
        {
            selectedSprites = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
            Debug.Log(selectedSprites.Length);
            SortSelectionByName();
            Repaint();
        }

        private void SortSelectionByName()
        {
            System.Array.Sort(selectedSprites, Compare);
            return;

            static int Compare(object x, object y)
            {
                return new CaseInsensitiveComparer().Compare(((Texture2D)x).name, ((Texture2D)y).name);
            }
        }

        private void OnGUI()
        {
            if (selectedSprites.Length <= 0)
            {
                EditorGUILayout.HelpBox(new GUIContent("No Sprite Selected."));
                return;
            }

            newPivot = EditorGUILayout.Vector2Field("New Pivot", newPivot);
            EditorGUILayout.BeginHorizontal();

            Rect btnPos = EditorGUILayout.GetControlRect(true);
            btnPos.min += Vector2.right * 14f;
            xInPixel = GUI.Toggle(btnPos, xInPixel, "In Pixel", "button");

            btnPos = EditorGUILayout.GetControlRect(true);
            yInPixel = GUI.Toggle(btnPos, yInPixel, "In Pixel", "button");

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Apply"))
            {
                foreach (Texture2D selectedSprite in selectedSprites)
                {
                    Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(selectedSprite));
                    ApplyPivot(s, newPivot, xInPixel, yInPixel);
                }

                AssetDatabase.SaveAssets();
                EditorApplication.ExecuteMenuItem("Assets/Reimport");
            }

            scroll = EditorGUILayout.BeginScrollView(scroll);
            StringBuilder builder = new StringBuilder("Editing:\n");
            bool first = true;
            for (int i = 0; i < selectedSprites.Length; i++)
            {
                if (!first) builder.Append(",\n");
                builder.Append(selectedSprites[i].name);
                first = false;
            }

            GUIContent content = new GUIContent(builder.ToString());
            EditorGUILayout.LabelField(content, EditorStyles.helpBox);
            EditorGUILayout.EndScrollView();
        }

        public static void ApplyPivot(Sprite spriteAsset, Vector2 newPivot, bool xInPixel = false, bool yInPixel = false)
        {
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(spriteAsset));
            if (importer.textureType != TextureImporterType.Sprite) return;

            if (importer.spriteImportMode == SpriteImportMode.Multiple)
                ApplyPivotMultiple(importer, newPivot, xInPixel, yInPixel);

            else ApplyPivotSingle(spriteAsset, importer, newPivot, xInPixel, yInPixel);
            EditorUtility.SetDirty(importer);
        }

        private static void ApplyPivotMultiple(TextureImporter importer, Vector2 newPivot, bool xInPixel, bool yInPixel)
        {
            SpriteMetaData[] spritesheet = importer.spritesheet;

            for (int i = 0; i < spritesheet.Length; i++)
            {
                SpriteMetaData sprite = spritesheet[i];

                if (xInPixel) newPivot.x /= sprite.rect.width;
                if (yInPixel) newPivot.y /= sprite.rect.height;

                spritesheet[i].alignment = (int)SpriteAlignment.Custom;
                spritesheet[i].pivot = newPivot;
            }

            EditorUtility.SetDirty(importer);
            importer.spritesheet = spritesheet;
        }

        private static void ApplyPivotSingle(Sprite sprite, TextureImporter importer, Vector2 newPivot, bool xInPixel, bool yInPixel)
        {
            TextureImporterSettings importSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(importSettings);

            if (xInPixel) newPivot.x /= sprite.rect.width;
            if (yInPixel) newPivot.y /= sprite.rect.height;

            importSettings.spriteAlignment = (int)SpriteAlignment.Custom;
            importSettings.spritePivot = newPivot;
            importer.SetTextureSettings(importSettings);
        }
    } 
}
