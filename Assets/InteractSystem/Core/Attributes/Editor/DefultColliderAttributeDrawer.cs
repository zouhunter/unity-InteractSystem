using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InteractSystem.Drawer
{
    [CustomPropertyDrawer(typeof(Attributes.DefultColliderAttribute))]
    public class DefultColliderAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
            {
                var go = (property.serializedObject.targetObject as MonoBehaviour).gameObject;
                property.objectReferenceValue = go.GetComponentInChildren<Collider>();
            }
        }
    }

}