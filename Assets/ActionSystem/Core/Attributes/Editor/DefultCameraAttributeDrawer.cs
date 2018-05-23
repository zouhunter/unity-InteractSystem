using UnityEngine;
using UnityEditor;

namespace WorldActionSystem.Attributes
{
    [CustomPropertyDrawer(typeof(DefultCameraAttribute))]
    public class DefultCameraAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DefultCameraAttribute att = (DefultCameraAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property);

                if (string.IsNullOrEmpty(property.stringValue))
                {
                    string cameraName = "defult";
                    //var target = (property.serializedObject.targetObject as MonoBehaviour).gameObject;
                    //if(target)
                    //{
                    //    var camreaNode = target.GetComponentInChildren<CameraNode>();
                    //    if (camreaNode){
                    //        cameraName = camreaNode.name;
                    //    }
                    //}
                    GUI.contentColor = Color.gray;
                    EditorGUI.LabelField(position, new GUIContent("    "), new GUIContent(cameraName));
                    GUI.contentColor = Color.white;
                }
            }

        }
    }

}