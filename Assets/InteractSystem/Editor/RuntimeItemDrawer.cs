using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace InteractSystem.Drawer
{
    [CustomPropertyDrawer(typeof(RunTimePrefabItem), true)]
    public class RunTimePrefabItemDrawer : PropertyDrawer
    {
        protected SerializedProperty instanceIDProp;
        protected SerializedProperty prefabProp;
        protected void FindCommonPropertys(SerializedProperty property)
        {
            instanceIDProp = property.FindPropertyRelative("instanceID");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindCommonPropertys(property);
            prefabProp = property.FindPropertyRelative("prefab");
            return EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FindCommonPropertys(property);
            prefabProp = property.FindPropertyRelative("prefab");

            if (prefabProp.objectReferenceValue != null)
            {
                label = new GUIContent(prefabProp.objectReferenceValue.name);
            }
            var rect = new Rect(position.x, position.y, position.width * 0.9f, EditorGUIUtility.singleLineHeight);
            var str = prefabProp.objectReferenceValue == null ? "" : prefabProp.objectReferenceValue.name;

            GUI.contentColor = ActionGUIUtil.NormalColor;
            if (!string.IsNullOrEmpty(ActionGUIUtil.searchWord) && prefabProp.objectReferenceValue != null)
            {
                GUI.contentColor = prefabProp.objectReferenceValue.ToString().ToLower().Contains(ActionGUIUtil.searchWord.ToLower()) ?
                   ActionGUIUtil.MatchColor : GUI.contentColor;
            }

            if (GUI.Button(rect, str, EditorStyles.toolbarDropDown))
            {
                property.isExpanded = !property.isExpanded;
                ToggleLoadPrefab();
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
                        if (DragAndDrop.objectReferences.Length > 0)
                        {
                            var obj = DragAndDrop.objectReferences[0];
                            if (obj is GameObject)
                            {
                                ActionEditorUtility.InsertPrefab(prefabProp, obj);
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
        }

        private void ToggleLoadPrefab()
        {
            if(instanceIDProp.intValue == 0 || EditorUtility.InstanceIDToObject(instanceIDProp.intValue) == null)
            {
                ActionEditorUtility.LoadPrefab(prefabProp, instanceIDProp);
            }
            else
            {
                ActionEditorUtility.SavePrefab(instanceIDProp);
            }
        }


        protected void InformationShow(Rect rect)
        {
            if (prefabProp.objectReferenceValue == null)
            {
                EditorGUI.HelpBox(rect, "丢失", MessageType.Error);
            }

            if(instanceIDProp.intValue !=  0)
            {
                var instence = EditorUtility.InstanceIDToObject(instanceIDProp.intValue);
                if(instence)
                {
                    var infoRect = rect;
                    infoRect.x = infoRect.width - 80;
                    infoRect.width = 100;

                    GUI.color = ActionGUIUtil.WarningColor;
                    EditorGUI.LabelField(infoRect, "开启中");
                    GUI.color = Color.white;
                }
               
            }
        }
    }
}