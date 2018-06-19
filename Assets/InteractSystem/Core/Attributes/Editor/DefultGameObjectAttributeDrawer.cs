using UnityEngine;
using UnityEditor;

namespace InteractSystem.Attributes
{
    [CustomPropertyDrawer(typeof(DefultGameObjectAttribute))]
    public class DefultGameObjectAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            DefultGameObjectAttribute att = (DefultGameObjectAttribute)attribute;
            return EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DefultGameObjectAttribute att = (DefultGameObjectAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(position, property);

                if (property.objectReferenceValue == null && property.serializedObject.targetObject is MonoBehaviour)
                {
                    property.objectReferenceValue = (property.serializedObject.targetObject as MonoBehaviour).gameObject;
                }
            }

        }
    }

}