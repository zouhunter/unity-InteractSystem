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
        protected SerializedProperty resetProp;
        protected SerializedProperty containsCommandProp;
        protected SerializedProperty containsPickAbleProp;
        
        protected SerializedProperty instanceIDProp;
        protected SerializedProperty targetProp;
        protected SerializedProperty prefabProp;
        private string drawrText;
        private Color color;
        protected void FindCommonPropertys(SerializedProperty property)
        {
            resetProp = property.FindPropertyRelative("reset");
            instanceIDProp = property.FindPropertyRelative("instanceID");
            targetProp = property.FindPropertyRelative("target");
            containsCommandProp = property.FindPropertyRelative("containsCommand");
            containsPickAbleProp = property.FindPropertyRelative("containsPickAble");
        }
        protected void TryHideItem()
        {
            var gitem = EditorUtility.InstanceIDToObject(instanceIDProp.intValue);
            if (gitem != null)
            {
                var prefab = PrefabUtility.GetPrefabParent(gitem);
                if (prefab != null)
                {
                    var root = PrefabUtility.FindPrefabRoot((GameObject)prefab);
                    if (root != null)
                    {
                        PrefabUtility.ReplacePrefab(gitem as GameObject, root, ReplacePrefabOptions.ConnectToPrefab);
                    }
                }
                GameObject.DestroyImmediate(gitem);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindCommonPropertys(property);
            prefabProp = property.FindPropertyRelative("prefab");
            return (property.isExpanded ? resetProp.boolValue ? 4 : 2 : 1) * EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (prefabProp.objectReferenceValue != null)
            {
                label = new GUIContent(prefabProp.objectReferenceValue.name);
            }
            var rect = new Rect(position.x, position.y, position.width * 0.9f, EditorGUIUtility.singleLineHeight);
            var str = prefabProp.objectReferenceValue == null ? "" : prefabProp.objectReferenceValue.name;
            if (GUI.Button(rect, str, EditorStyles.toolbarDropDown))
            {
                property.isExpanded = !property.isExpanded;
                if (property.isExpanded)
                {
                    TryCreateItem();
                }
                else
                {
                    TryHideItem();
                }
            }

            if (prefabProp.objectReferenceValue == null)
            {
                EditorGUI.HelpBox(rect, "丢失", MessageType.Error);
            }
            else
            {
                drawrText = "";
                color = Color.clear;
                if (resetProp.boolValue)
                {
                    drawrText += "[reset] ";
                    color += new Color(0.1f, 0.2f, 0.3f);
                }
                if (containsCommandProp.boolValue)
                {
                    drawrText += "[command] ";
                    color += new Color(0.2f, 0.3f, 0.1f);
                }
                if (containsPickAbleProp.boolValue)
                {
                    drawrText += "[pickup] ";
                    color += new Color(0.3f, 0.2f, 0.1f);
                }
                var infoRect = rect;
                infoRect.x += 100f;
                infoRect.width -= 100f;
                GUI.color = color;
                EditorGUI.SelectableLabel(infoRect, drawrText);
                GUI.color = Color.white;
            }

            rect = new Rect(position.max.x - position.width * 0.1f, position.y, position.width * 0.1f, EditorGUIUtility.singleLineHeight);

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
                                var prefab = PrefabUtility.GetPrefabParent(obj);
                                if(prefab != null)
                                {
                                    prefabProp.objectReferenceValue = prefab;
                                }
                                else
                                {
                                    var path = AssetDatabase.GetAssetPath(obj);
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        prefabProp.objectReferenceValue = obj;
                                    }
                                }
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
                var choiseRect = new Rect(rect.x, rect.y, position.width * 0.3f, rect.height);
                resetProp.boolValue = EditorGUI.ToggleLeft(choiseRect,"Reset", resetProp.boolValue);
                choiseRect.x += position.width * 0.32f;
                containsCommandProp.boolValue = EditorGUI.ToggleLeft(choiseRect,"Command", containsCommandProp.boolValue);
                choiseRect.x += position.width * 0.32f;
                containsPickAbleProp.boolValue = EditorGUI.ToggleLeft(choiseRect,"PickUp", containsPickAbleProp.boolValue);
                if (resetProp.boolValue)
                {
                    rect.y += 1.5f * EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, targetProp);
                }
            }
        }
        protected void TryCreateItem()
        {
            if (prefabProp.objectReferenceValue == null){
                return;
            }
            GameObject gopfb = prefabProp.objectReferenceValue as GameObject;
            if (gopfb != null)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(gopfb) as GameObject;
                var actionSystem = GameObject.FindObjectOfType<ActionSystem>();
                go.transform.SetParent(actionSystem.transform, true);
                if (resetProp.boolValue)
                {
                    var trans = targetProp.objectReferenceValue;
                    if(trans != null)
                    {
                        var target = trans as Transform;
                        go.transform.position = target.position;
                        go.transform.rotation = target.rotation;
                        go.transform.localScale = target.localScale;
                    }
                  else
                    {
                        Debug.LogError("坐标对象为空");
                    }
                }
                instanceIDProp.intValue = go.GetInstanceID();
            }
        }
    }
}