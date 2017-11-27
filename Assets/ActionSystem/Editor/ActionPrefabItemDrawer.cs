using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace WorldActionSystem
{
    [CustomPropertyDrawer(typeof(ActionPrefabItem), true)]
    public class ActionPrefabItemDrawer : PropertyDrawer
    {
        protected SerializedProperty reparentProp;
        protected SerializedProperty containsCommandProp;
        protected SerializedProperty rematrixProp;
        protected SerializedProperty matrixProp;

        protected SerializedProperty instanceIDProp;
        protected SerializedProperty parentProp;
        protected SerializedProperty prefabProp;
        protected SerializedProperty ignoreProp;

        protected void FindCommonPropertys(SerializedProperty property)
        {
            rematrixProp = property.FindPropertyRelative("rematrix");
            matrixProp = property.FindPropertyRelative("matrix");
            reparentProp = property.FindPropertyRelative("reparent");
            instanceIDProp = property.FindPropertyRelative("instanceID");
            parentProp = property.FindPropertyRelative("parent");
            containsCommandProp = property.FindPropertyRelative("containsCommand");
            ignoreProp = property.FindPropertyRelative("ignore");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindCommonPropertys(property);
            prefabProp = property.FindPropertyRelative("prefab");
            return (property.isExpanded ? reparentProp.boolValue ? 4 : 2 : 1) * EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (prefabProp.objectReferenceValue != null){
                label = new GUIContent(prefabProp.objectReferenceValue.name);
            }
            var rect = new Rect(position.x, position.y, position.width * 0.9f, EditorGUIUtility.singleLineHeight);
            var str = prefabProp.objectReferenceValue == null ? "" : prefabProp.objectReferenceValue.name;
            GUI.contentColor = Color.cyan;
            if (GUI.Button(rect, str, EditorStyles.toolbarDropDown))
            {
                property.isExpanded = !property.isExpanded;
                if (property.isExpanded)
                {
                    ActionEditorUtility.LoadPrefab(prefabProp, instanceIDProp, reparentProp, parentProp, rematrixProp, matrixProp);
                }
                else
                {
                    ActionEditorUtility.SavePrefab(instanceIDProp, rematrixProp, matrixProp);
                }
            }
            GUI.contentColor = Color.white;

            InformationShow(rect);

             rect = new Rect(position.max.x - position.width * 0.1f, position.y,20, EditorGUIUtility.singleLineHeight);

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
                            if (obj is GameObject){
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
            var tempRect = rect;
            var optionCount = 4;
            var width = rect.width / optionCount;
            var choiseRect = new Rect(rect.x, rect.y, width, rect.height);
            rematrixProp.boolValue = EditorGUI.ToggleLeft(choiseRect, "Matrix", rematrixProp.boolValue);
            choiseRect.x += width;
            reparentProp.boolValue = EditorGUI.ToggleLeft(choiseRect, "Parent", reparentProp.boolValue);
            choiseRect.x += width;
            containsCommandProp.boolValue = EditorGUI.ToggleLeft(choiseRect, "Command", containsCommandProp.boolValue);
            choiseRect.x += width;
            ignoreProp.boolValue = EditorGUI.ToggleLeft(choiseRect, "[Ignore]", ignoreProp.boolValue);
            choiseRect.x += width;
            if (reparentProp.boolValue)
            {
                tempRect.y += 1.5f * EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(tempRect, parentProp);
            }
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
                infoRect.width = 25;
                if (reparentProp.boolValue)
                {
                    GUI.color = new Color(0.3f, 0.5f, 0.8f);
                    EditorGUI.SelectableLabel(infoRect, "[p]");
                    infoRect.x += infoRect.width;
                }

                if (rematrixProp.boolValue)
                {
                    GUI.color = new Color(0.8f, 0.8f, 0.4f);
                    EditorGUI.SelectableLabel(infoRect, "[m]");
                    infoRect.x += infoRect.width;
                }

                if (containsCommandProp.boolValue)
                {
                    GUI.color = new Color(0.5f, 0.8f, 0.3f);
                    EditorGUI.SelectableLabel(infoRect, "[c]");
                    infoRect.x += infoRect.width;
                }

                if(ignoreProp.boolValue)
                {
                    EditorGUI.HelpBox(rect, "", MessageType.Warning);
                }
                GUI.color = Color.white;
            }
        }
    }
}