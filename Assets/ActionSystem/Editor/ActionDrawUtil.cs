using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace WorldActionSystem.Drawer
{
    public static class ActionGUIUtil
    {
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
        public static void UpdateDragedGameObjects(List<GameObject> dragedGameObject)
        {
            dragedGameObject.Clear();
            foreach (var item in DragAndDrop.objectReferences)
            {
                if (item is GameObject)
                {
                    dragedGameObject.Add(item as GameObject);
                }
                else if (ProjectWindowUtil.IsFolder(item.GetInstanceID()))
                {
                    var folder = AssetDatabase.GetAssetPath(item);
                    SearchDeep(folder, ".prefab", dragedGameObject);
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
    }
}