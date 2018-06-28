using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InteractSystem.Drawer
{
    [CustomPropertyDrawer(typeof(Enviroment.EnviromentInfo))]
    public class EnviromentInfoDrawer : PropertyDrawer
    {
        protected SerializedProperty elementNameProp;
        protected SerializedProperty guidProp;
        protected SerializedProperty instenceIDProp;
        protected SerializedProperty enviromentNameProp;
        protected SerializedProperty coordinateProp;
        public List<GameObject> dragedGameObjs = new List<GameObject>();
        protected Transform instenceTranform
        {
            get
            {
                if (instenceIDProp.intValue != 0)
                {
                    var obj = EditorUtility.InstanceIDToObject(instenceIDProp.intValue);
                    if(obj != null)
                    {
                        return (obj as GameObject).transform;
                    }
                }
                return null;
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(property, label, true);
            return height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            elementNameProp = property.FindPropertyRelative("enviromentName");
            guidProp = property.FindPropertyRelative("guid");
            instenceIDProp = property.FindPropertyRelative("instenceID");
            enviromentNameProp = property.FindPropertyRelative("enviromentName");
            coordinateProp = property.FindPropertyRelative("coordinate");


            var btnRect = new Rect(position.x, position.y, position.width - ActionGUIUtil.middleButtonWidth, EditorGUIUtility.singleLineHeight);
            if (instenceIDProp.intValue != 0 && EditorUtility.InstanceIDToObject(instenceIDProp.intValue) == null)
            {
                instenceIDProp.intValue = 0;
                property.isExpanded = false;
            }
            if (!string.IsNullOrEmpty(guidProp.stringValue) && string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guidProp.stringValue)))
            {
                guidProp.stringValue = null;
            }

            if (GUI.Button(btnRect, elementNameProp.stringValue, EditorStyles.toolbarDropDown))
            {
                property.isExpanded = !property.isExpanded;
                if (property.isExpanded)
                {
                    ActionEditorUtility.LoadPrefab(guidProp.stringValue,instenceIDProp, coordinateProp);
                }
                else
                {
                    ActionEditorUtility.SavePrefab(instenceIDProp, coordinateProp);
                }
            }

            var objRect = new Rect(position.x + position.width - ActionGUIUtil.middleButtonWidth, position.y, ActionGUIUtil.middleButtonWidth, EditorGUIUtility.singleLineHeight);
            if (!string.IsNullOrEmpty(guidProp.stringValue))
            {
                if (GUI.Button(objRect, "", EditorStyles.objectFieldMiniThumb))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guidProp.stringValue));
                    EditorGUIUtility.PingObject(prefab);
                }

                if (Event.current.type == EventType.DragUpdated && objRect.Contains(Event.current.mousePosition))
                {
                    ActionGUIUtil.UpdateDragedObjects<GameObject>(".prefab", dragedGameObjs);
                }

                else if (Event.current.type == EventType.DragPerform && objRect.Contains(Event.current.mousePosition))
                {
                    foreach (var item in dragedGameObjs)
                    {
                        RecordPrefab(item);
                        break;
                    }
                }
            }
            else
            {
                var obj = EditorGUI.ObjectField(objRect, null, typeof(GameObject), false);
                if (obj != null)
                {
                    RecordPrefab(obj);
                }
            }

            if(instenceIDProp.intValue != 0)
            {
                DrawInfo(position);
            }

            if (property.isExpanded)
            {
                position.y += EditorGUIUtility.singleLineHeight + 2f;
                EditorGUI.BeginChangeCheck();
                ActionGUIUtil.DrawChildInContent(property, position,null, "ignore",-1);
                if(EditorGUI.EndChangeCheck())
                {
                    if (instenceTranform)
                    {
                        ActionEditorUtility.LoadCoordinatePropInfo(coordinateProp, instenceTranform);
                    }
                }
                else
                {
                    if(instenceTranform)
                    {
                        ActionEditorUtility.SaveCoordinatesInfo(coordinateProp, instenceTranform);
                    }
                }
            }
        }

        private void DrawInfo(Rect position)
        {
            var rect = new Rect(position.x + position.width - 100, position.y, 100, EditorGUIUtility.singleLineHeight);
            GUI.contentColor = ActionGUIUtil.NormalColor;
            EditorGUI.LabelField(rect, "开启中");
            GUI.contentColor = Color.white;
        }

        private void RecordPrefab(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            guidProp.stringValue = guid;
            if (string.IsNullOrEmpty(enviromentNameProp.stringValue))
            {
                enviromentNameProp.stringValue = obj.name;
            }
            if (coordinateProp.FindPropertyRelative("localScale").vector3Value == Vector3.zero)
            {
                ActionEditorUtility.SaveCoordinatesInfo(coordinateProp, (obj as GameObject).transform);
            }
        }

       
    }

}