using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace InteractSystem.Drawer
{
    public static class ActionGUIUtil
    {
        public static float currentViewWidth { get { return EditorGUIUtility.currentViewWidth - 100; } }
        public const float smallButtonWidth = 20f;
        public const float middleButtonWidth = 45f;
        public const float bigButtonWidth = 60f;
        public const float padding = 10;
        public static string searchWord;
        public static Dictionary<UnityEngine.Object, Editor> editorDic = new Dictionary<UnityEngine.Object, Editor>();
        public static Dictionary<UnityEngine.Object, SerializedObject> serializedDic = new Dictionary<UnityEngine.Object, SerializedObject>();
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

        public static Editor CreateCachedEditor(UnityEngine.Object objectReferenceValue)
        {
            if (!editorDic.ContainsKey(objectReferenceValue) || editorDic[objectReferenceValue] == null)
            {
                editorDic[objectReferenceValue] = Editor.CreateEditor(objectReferenceValue);
            }
            return editorDic[objectReferenceValue];
        }

        internal static void DrawScriptablesMenu(List<Type> bindingTypes,UnityAction<ScriptableObject> onCreate = null)
        {
            var options = bindingTypes.ConvertAll(x => new GUIContent(x.FullName)).ToArray();
            Debug.Log(options.Length);
            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), options, -1, (data, ops, s) =>
            {
                if (s >= 0)
                {
                    var type = bindingTypes[s];
                    var asset = ScriptableObject.CreateInstance(type);
                    if(onCreate != null){
                        onCreate.Invoke(asset);
                    }
                    ProjectWindowUtil.CreateAsset(asset, "new_" + type.Name + ".asset");
                }
            }, null);

        }

        public static SerializedObject CreateCachedSerializedObject(UnityEngine.Object objectReferenceValue)
        {
            if (!serializedDic.ContainsKey(objectReferenceValue) || serializedDic[objectReferenceValue] == null)
            {
                serializedDic[objectReferenceValue] = new SerializedObject(objectReferenceValue);
            }
            return serializedDic[objectReferenceValue];
        }

        /// <summary>
        /// 在指定区域绘制默认属性
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="position"></param>
        /// <param name="finalPropertyName"></param>
        public static void DrawChildInContent(SerializedProperty serializedProperty, Rect position,List<string> ignorePorps =null, string finalPropertyName = null,int level = 0)
        {
            bool enterChildren = true;
            SerializedProperty endProperty = string.IsNullOrEmpty(finalPropertyName) ? null:  serializedProperty.FindPropertyRelative(finalPropertyName);
            while (serializedProperty.NextVisible(enterChildren))
            {
                if (ignorePorps != null && ignorePorps.Contains(serializedProperty.propertyPath)) continue;

                EditorGUI.indentLevel = serializedProperty.depth + level;
                position.height = EditorGUI.GetPropertyHeight(serializedProperty, null, true);
                EditorGUI.PropertyField(position, serializedProperty, true);
                position.y += position.height + 2f;
                enterChildren = false;

                if (SerializedProperty.EqualContents(serializedProperty, endProperty))
                {
                    break;
                }
            }
        }
        /// <summary>
        /// 计算
        /// </summary>
        internal static float GetSerializedObjectHeight(SerializedObject se,List<string> ignorePorps = null)
        {
            var prop = se.GetIterator();
            var enterChildern = true;
            float hight = 0;
            while (prop.NextVisible(enterChildern))
            {
                enterChildern = false;
                if (ignorePorps != null && ignorePorps.Contains(prop.propertyPath))
                    continue;
                hight += EditorGUI.GetPropertyHeight(prop, null, true);
            }
            return hight ;
        }

        /// <summary>
        /// 手动把脚本绘制出来
        /// </summary>
        /// <param name="script_prop"></param>
        public static void DrawDisableProperty(SerializedProperty script_prop)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(script_prop, true);
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// 绘制一个拖拽区
        /// </summary>
        /// <param name="objRect"></param>
        /// <param name="elementGroup_prop"></param>
        /// <param name="type"></param>
        internal static void AcceptDrawField(Rect objRect, SerializedProperty elementGroup_prop, Type type)
        {
            if (GUI.Button(objRect, "", EditorStyles.objectFieldMiniThumb))
            {
                EditorGUIUtility.PingObject(elementGroup_prop.objectReferenceInstanceIDValue);
            }
            UnityEngine.Object obj = null;
            if (Event.current.type == EventType.DragUpdated && objRect.Contains(Event.current.mousePosition))
            {
                foreach (var item in DragAndDrop.objectReferences)
                {
                    if (item.GetType() == type)
                    {
                        obj = item;
                        break;
                    }
                }
                DragAndDrop.visualMode = obj ? DragAndDropVisualMode.Move : DragAndDropVisualMode.Rejected;
            }

            if (Event.current.type == EventType.DragPerform && objRect.Contains(Event.current.mousePosition) && obj)
            {
                elementGroup_prop.objectReferenceValue = obj;
            }
        }

        /// <summary>
        /// address: ".prefab"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <param name="dragedGameObject"></param>
        public static void UpdateDragedObjects<T>(string address, List<T> dragedGameObject) where T : UnityEngine.Object
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
            var idRect = new Rect(orignalRect.x - padding, orignalRect.y + padding, 20, 20);
            EditorGUI.LabelField(idRect, index.ToString());
            var boxRect = PaddingRect(orignalRect, padding * 0.5f);
            GUI.Box(boxRect, "");
            var rect = PaddingRect(orignalRect);
            return rect;
        }
        public static Rect PaddingRect(Rect orignalRect, float padding = padding)
        {
            var rect = new Rect(orignalRect.x + padding, orignalRect.y + padding, orignalRect.width - padding * 2, orignalRect.height - padding * 2);
            return rect;
        }

        public static Rect GetDragRect()
        {
            var rect = GUILayoutUtility.GetRect(ActionGUIUtil.currentViewWidth, EditorGUIUtility.singleLineHeight);
            rect.y -= EditorGUIUtility.singleLineHeight;
            rect.height += EditorGUIUtility.singleLineHeight;
            return rect;
        }

    }
}