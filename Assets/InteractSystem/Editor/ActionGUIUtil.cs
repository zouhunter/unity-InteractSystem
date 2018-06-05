using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace InteractSystem.Drawer
{
    public static class ActionGUIUtil
    {
        public const float smallButtonWidth = 20f;
        public const float middleButtonWidth = 45f;
        public const float bigButtonWidth = 60f;
        public const float span = 5;
        public static string searchWord;
        public static Color IgnoreColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.red : Color.black;
            }
        }
        public static Color NormalColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.green : Color.white;
            }
        }
        public static Color WarningColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.yellow : Color.white;
            }
        }
        public static Color MatchColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.cyan : Color.white;
            }
        }
        /// <summary>
        /// address: ".prefab"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <param name="dragedGameObject"></param>
        public static void UpdateDragedObjects<T>(string address,List<T> dragedGameObject) where T:UnityEngine.Object
        {
            dragedGameObject.Clear();
            foreach (var item in DragAndDrop.objectReferences)
            {
                if (item is T)
                {
                    dragedGameObject.Add(item as T);
                }
                else if (ProjectWindowUtil.IsFolder(item.GetInstanceID()))
                {
                    var folder = AssetDatabase.GetAssetPath(item);
                    SearchDeep(folder, address, dragedGameObject);
                }
            }
            DragAndDrop.visualMode = dragedGameObject.Count > 0 ? DragAndDropVisualMode.Move : DragAndDropVisualMode.Rejected;
        }

        public static void SearchDeep<T>(string folder, string address, List<T> list) where T : UnityEngine.Object
        {
            var files = System.IO.Directory.GetFiles(folder, "*" + address, System.IO.SearchOption.AllDirectories);
            foreach (var filePath in files)
            {
                var root = System.IO.Path.GetPathRoot(filePath);

                if (filePath.EndsWith(address))
                {
                    var path = filePath.Substring(root.Length);
                    var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                    if (asset != null)
                    {
                        list.Add(asset);
                    }
                }
            }
        }
        public static Rect DrawBoxRect(Rect orignalRect, string index)
        {
            var idRect = new Rect(orignalRect.x - span, orignalRect.y + span, 20, 20);
            EditorGUI.LabelField(idRect, index.ToString());
            var boxRect = PaddingRect(orignalRect, span * 0.5f);
            GUI.Box(boxRect, "");
            var rect = PaddingRect(orignalRect);
            return rect;
        }
        public static Rect PaddingRect(Rect orignalRect, float padding = span)
        {
            var rect = new Rect(orignalRect.x + padding, orignalRect.y + padding, orignalRect.width - padding * 2, orignalRect.height - padding * 2);
            return rect;
        }

        public static Rect GetDragRect()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            rect.y -= EditorGUIUtility.singleLineHeight;
            rect.height += EditorGUIUtility.singleLineHeight;
            return rect;
        }

    }
}