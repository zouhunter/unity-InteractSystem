using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InteractSystem.Drawer
{
    [CustomPropertyDrawer(typeof(Enviroment.EnviromentInfo))]
    public class EnviromentInfoDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(property, label, true);
            return height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);e
            var btnRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var elementNameProp = property.FindPropertyRelative("enviromentName");
            if (GUI.Button(btnRect, elementNameProp.stringValue, EditorStyles.toolbarDropDown))
            {
                property.isExpanded = !property.isExpanded;
            }

            if (property.isExpanded)
            {
                position.y += EditorGUIUtility.singleLineHeight + 2f;

                DrawChildInContent(property, position);
            }
        }

        private void DrawChildInContent(SerializedProperty serializedProperty, Rect position)
        {
            bool enterChildren = true;
            var endProperty = serializedProperty.FindPropertyRelative("ignore");
            while (serializedProperty.NextVisible(enterChildren))
            {
                EditorGUI.indentLevel = serializedProperty.depth - 1;
                position.height = EditorGUI.GetPropertyHeight(serializedProperty, null, true);
                EditorGUI.PropertyField(position, serializedProperty, true);
                position.y += position.height + 2f;
                enterChildren = false;

                if(SerializedProperty.EqualContents(serializedProperty, endProperty))
                {
                    break;
                }
            }
        }
    }

}