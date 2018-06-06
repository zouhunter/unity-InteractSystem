using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace InteractSystem.Drawer
{
    [CustomPropertyDrawer(typeof(Enviroment.EnviromentItem))]
    public class EnviromentItemDrawer : PropertyDrawer
    {
        SerializedProperty _name_prop;
        SerializedProperty prefab_prop;
        SerializedProperty instanceID_prop;
        private const float buttonWidth = 40f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _name_prop = property.FindPropertyRelative("_name");
            prefab_prop = property.FindPropertyRelative("prefab");
            instanceID_prop = property.FindPropertyRelative("instanceID");

            var nameRect = new Rect(position.x, position.y, position.width * 0.3f, position.height);

            GUI.contentColor = ActionGUIUtil.NormalColor;
            if (!string.IsNullOrEmpty(ActionGUIUtil.searchWord))
                GUI.contentColor = _name_prop.stringValue.ToLower().Contains(ActionGUIUtil.searchWord.ToLower()) ?
                   ActionGUIUtil.MatchColor : GUI.contentColor;

            _name_prop.stringValue = EditorGUI.TextField(nameRect, _name_prop.stringValue);
            GUI.contentColor = Color.white;

            if (string.IsNullOrEmpty(_name_prop.stringValue) && prefab_prop.objectReferenceValue != null)
            {
                _name_prop.stringValue = prefab_prop.objectReferenceValue.name;
            }
            var showName = "Null";
            if (prefab_prop.objectReferenceValue != null)
            {
                showName = prefab_prop.objectReferenceValue.name;
            }
            var btnRect = new Rect(position.x + position.width * 0.35f, position.y, position.width * 0.6f - buttonWidth, position.height);



            if (GUI.Button(btnRect, showName, EditorStyles.toolbarDropDown) && prefab_prop.objectReferenceValue)
            {
                if (instanceID_prop.intValue == 0)
                {
                    ActionEditorUtility.LoadPrefab(prefab_prop, instanceID_prop);
                }
                else
                {
                    var instence = EditorUtility.InstanceIDToObject(instanceID_prop.intValue);
                    if (instence)
                    {
                        ActionEditorUtility.SavePrefab(instanceID_prop,true);
                    }
                }
            }

            if (instanceID_prop.intValue != 0 && EditorUtility.InstanceIDToObject(instanceID_prop.intValue) != null)
            {
                var labelRect = new Rect(btnRect.x + btnRect.width - 100, btnRect.y, 100, btnRect.height);
                GUI.contentColor = ActionGUIUtil.WarningColor;
                EditorGUI.LabelField(labelRect, "开启中");
                GUI.contentColor = Color.white;
            }
            else
            {
                instanceID_prop.intValue = 0;
            }

            var objRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);
            if (prefab_prop.objectReferenceValue == null)
            {
                prefab_prop.objectReferenceValue = EditorGUI.ObjectField(objRect, prefab_prop.objectReferenceValue, typeof(GameObject), false);
            }
            else
            {
                if (GUI.Button(objRect, "", EditorStyles.objectFieldMiniThumb))
                {
                    EditorGUIUtility.PingObject(prefab_prop.objectReferenceInstanceIDValue);
                }

            }
        }


    }
}
