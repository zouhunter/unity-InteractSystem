using UnityEngine;
using UnityEditor;

namespace InteractSystem.Attributes
{
    [CustomPropertyDrawer(typeof(DefultNameAttribute))]
    public class DefultNameAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            DefultNameAttribute att = (DefultNameAttribute)attribute;
            return  EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DefultNameAttribute att = (DefultNameAttribute)attribute;

            if(property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property);

                if (string.IsNullOrEmpty(property.stringValue))
                {
                    GUI.contentColor = Color.gray;
                    EditorGUI.LabelField(position, new GUIContent("    "),new GUIContent( property.serializedObject.targetObject.name));
                    GUI.contentColor = Color.white;
                }
            }

        }
    }

}