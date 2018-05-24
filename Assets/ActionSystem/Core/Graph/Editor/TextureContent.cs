using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace WorldActionSystem.Graph
{
    public class TextureContent : ScriptableObject
    {
        public const string instenceGuid = "bd4d13d14a284ac40996cc8a3741a565";
        [HideInInspector]
        public List<TextureItem> textures = new List<TextureItem>();
        private static TextureContent _instence;
        public static TextureContent Instence
        {
            get
            {
                if (_instence == null)
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(instenceGuid);
                    if (!string.IsNullOrEmpty(path))
                    {
                        _instence = UnityEditor.AssetDatabase.LoadAssetAtPath<TextureContent>(path);
                    }
                }
                return _instence;
            }
        }
        public static Texture LoadTexture(string name)
        {
            if (Instence != null)
            {
                var item = Instence.textures.Find(x => x.name == name);
                if (item != null)
                {
                    return item.texture;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class TextureItem
    {
        public string name;
        public string description;
        public Texture texture;
    }

    [CustomEditor(typeof(TextureContent))]
    public class TextureContentDrawer : Editor
    {
        TextureContent content;
        ReorderableList reorderList;
        private void OnEnable()
        {
            content = target as TextureContent;
            reorderList = new ReorderableList(content.textures, typeof(TextureItem));
            reorderList.drawHeaderCallback = DrawHead;
            reorderList.drawElementCallback = DrawBody;
            reorderList.elementHeight = 4 * EditorGUIUtility.singleLineHeight + 40;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            reorderList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("save!");
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawBody(Rect position, int index, bool isActive, bool isFocused)
        {
            var rect = new Rect(position.x + 20, position.y + 20, position.width - 40, position.height - 40);
            var boxRect = new Rect(position.x + 10, position.y + 10, position.width - 20, position.height - 20);
            GUI.Box(boxRect, "");
            var idRect = new Rect(position.x, position.y + 20, 20, 20);
            EditorGUI.LabelField(idRect, index.ToString());
            var item = content.textures[index];
            var labelRect = new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight);
            item.name = EditorGUI.TextField(labelRect, item.name);
            var descriptionRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width - EditorGUIUtility.singleLineHeight * 4, EditorGUIUtility.singleLineHeight * 3);
            item.description = EditorGUI.TextField(descriptionRect, item.description);
            var pictureRect = new Rect(rect.x + rect.width - 4 * EditorGUIUtility.singleLineHeight, rect.y, 4 * EditorGUIUtility.singleLineHeight, 4 * EditorGUIUtility.singleLineHeight);
            item.texture = EditorGUI.ObjectField(pictureRect, item.texture, typeof(Texture), false) as Texture;
        }

        private void DrawHead(Rect rect)
        {
            EditorGUI.LabelField(rect, "图片列表");
        }
    }

}
