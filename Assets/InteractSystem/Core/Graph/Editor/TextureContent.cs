using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;

namespace InteractSystem.Graph
{
    public class TextureContent : ScriptableObject
    {
        [HideInInspector]
        public List<Texture> textures = new List<Texture>();
        private static Dictionary<string, TextureContent> _instenceDic = new Dictionary<string, TextureContent>();
        public static TextureContent GetInstence(string guid)
        {
            if (!_instenceDic.ContainsKey(guid))
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(path))
                {
                    _instenceDic[guid] = UnityEditor.AssetDatabase.LoadAssetAtPath<TextureContent>(path);
                }
            }
            return _instenceDic.ContainsKey(guid) ? null : _instenceDic[guid];
        }

        public Texture LoadTexture(string name)
        {
            var item = textures.Find(x => x.name == name);
            if (item != null)
            {
                return item;
            }
            return null;
        }
    }

    [CustomEditor(typeof(TextureContent))]
    public class TextureContentDrawer : Editor
    {
        TextureContent content;
        ReorderableList reorderList;
        private void OnEnable()
        {
            content = target as TextureContent;
            reorderList = new ReorderableList(content.textures, typeof(Texture));
            reorderList.drawHeaderCallback = DrawHead;
            reorderList.drawElementCallback = DrawBody;
            reorderList.elementHeight = EditorGUIUtility.singleLineHeight + 20;
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
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            DrawDragField(rect);
        }
        private void DrawDragField(Rect rect)
        {
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                    break;
                case EventType.DragPerform:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        var textures = DragAndDrop.objectReferences.Where(x => x is Texture);
                        foreach (Texture item in textures)
                        {
                            if (!content.textures.Contains(item))
                            {
                                content.textures.Add(item);
                            }
                        }
                    }
                    break;
                case EventType.DragExited:
                    break;
                default:
                    break;
            }
        }

        private void DrawBody(Rect position, int index, bool isActive, bool isFocused)
        {
            var rect = new Rect(position.x + 10, position.y + 10, position.width - 20, position.height - 20);
            var boxRect = new Rect(position.x + 5, position.y + 5, position.width - 10, position.height - 10);
            GUI.Box(boxRect, "");
            var idRect = new Rect(position.x - 10, position.y + 10, 20, 20);
            EditorGUI.LabelField(idRect, index.ToString());
            var item = content.textures[index];
            var labelRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
            if (item != null)
            {
               item.name =  EditorGUI.TextField(labelRect, item.name);
            }
            var pictureRect = new Rect(rect.x + rect.width * 0.4f, rect.y, rect.width * 0.6f, EditorGUIUtility.singleLineHeight);
            content.textures[index] = EditorGUI.ObjectField(pictureRect, item, typeof(Texture), false) as Texture;
        }

        private void DrawHead(Rect rect)
        {
            EditorGUI.LabelField(rect, "图片列表");
        }
    }

}
