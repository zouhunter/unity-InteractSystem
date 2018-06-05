using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace WorldActionSystem.Drawer
{
    [CustomPropertyDrawer(typeof(AutoPrefabItem), true)]
    public class AutoPrefabItemDrawer : PropertyDrawer
    {
        protected SerializedProperty coordinateProp;
        protected SerializedProperty instanceIDProp;
        protected SerializedProperty prefabProp;
        protected SerializedProperty ignoreProp;

        protected void FindCommonPropertys(SerializedProperty property)
        {
            coordinateProp = property.FindPropertyRelative("coordinate");
            instanceIDProp = property.FindPropertyRelative("instanceID");
            ignoreProp = property.FindPropertyRelative("ignore");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindCommonPropertys(property);
            prefabProp = property.FindPropertyRelative("prefab");
            return (property.isExpanded ? 2 : 1) * EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FindCommonPropertys(property);
            prefabProp = property.FindPropertyRelative("prefab");

            if (instanceIDProp.intValue != 0 && EditorUtility.InstanceIDToObject(instanceIDProp.intValue) == null)
            {
                instanceIDProp.intValue = 0;
            }
            if (prefabProp.objectReferenceValue != null)
            {
                label = new GUIContent(prefabProp.objectReferenceValue.name);
            }

            property.isExpanded = instanceIDProp.intValue != 0;

            var rect = new Rect(position.x, position.y, position.width * 0.9f, EditorGUIUtility.singleLineHeight);
            var str = prefabProp.objectReferenceValue == null ? "" : prefabProp.objectReferenceValue.name;

            GUI.contentColor = ignoreProp.boolValue ? ActionGUIUtil.IgnoreColor : ActionGUIUtil.NormalColor;
            if (!string.IsNullOrEmpty(ActionGUIUtil.searchWord) && prefabProp.objectReferenceValue != null)
            {
                GUI.contentColor = prefabProp.objectReferenceValue.ToString().ToLower().Contains(ActionGUIUtil.searchWord.ToLower()) ?
                   ActionGUIUtil.MatchColor : GUI.contentColor;
            }

            if (GUI.Button(rect, str, EditorStyles.toolbarDropDown))
            {
                if (instanceIDProp.intValue == 0)
                {
                    ActionEditorUtility.LoadPrefab(prefabProp, instanceIDProp, coordinateProp);
                }
                else
                {
                    ActionEditorUtility.SavePrefab(instanceIDProp, coordinateProp);
                }
            }
            GUI.contentColor = Color.white;


            InformationShow(rect);

            rect = new Rect(position.max.x - position.width * 0.1f, position.y, 20, EditorGUIUtility.singleLineHeight);

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
                        Debug.Log(DragAndDrop.objectReferences.Length);
                        if (DragAndDrop.objectReferences.Length > 0)
                        {
                            var obj = DragAndDrop.objectReferences[0];
                            if (obj is GameObject)
                            {
                                ActionEditorUtility.InsertItem(prefabProp, obj);
                            }
                            DragAndDrop.AcceptDrag();
                        }
                        Event.current.Use();
                    }
                    break;
                case EventType.DragExited:
                    break;
            }

            if (prefabProp.objectReferenceValue != null)
            {
                if (GUI.Button(rect, "", EditorStyles.objectFieldMiniThumb))
                {
                    EditorGUIUtility.PingObject(prefabProp.objectReferenceValue);
                }
            }
            else
            {
                prefabProp.objectReferenceValue = EditorGUI.ObjectField(rect, null, typeof(GameObject), false);
            }

            rect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

            if (property.isExpanded)
            {
                DrawOptions(rect);
            }
        }

        protected void DrawOptions(Rect rect)
        {
            var optionCount = 4;
            var width = rect.width / optionCount;
            var choiseRect = new Rect(rect.x, rect.y, width, rect.height);
            ignoreProp.boolValue = EditorGUI.ToggleLeft(choiseRect, "[Ignore]", ignoreProp.boolValue);
            choiseRect.x += width;
        }

        protected void InformationShow(Rect rect)
        {
            if (prefabProp.objectReferenceValue == null)
            {
                EditorGUI.HelpBox(rect, "丢失", MessageType.Error);
            }
            else
            {
                var infoRect = rect;
                infoRect.x = infoRect.width - 80;
                infoRect.width = 100;
                GUI.contentColor = ActionGUIUtil.WarningColor;
                if (instanceIDProp.intValue != 0)
                {
                    EditorGUI.LabelField(infoRect, "开启中");
                }
                GUI.contentColor = Color.white;
            }
        }
    }
}