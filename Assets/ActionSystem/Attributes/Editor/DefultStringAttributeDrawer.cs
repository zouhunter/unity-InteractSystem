using UnityEngine;
using UnityEditor;

namespace WorldActionSystem.Attributes
{
    [CustomPropertyDrawer(typeof(DefultStringAttribute))]
    public class DefultStringAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DefultStringAttribute att = (DefultStringAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property);

                if (string.IsNullOrEmpty(property.stringValue))
                {
                    GUI.contentColor = Color.gray;
                    EditorGUI.LabelField(position, new GUIContent("    "), new GUIContent(att.text));
                    GUI.contentColor = Color.white;
                }
            }

        }
    }

}