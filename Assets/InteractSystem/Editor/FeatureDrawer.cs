using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InteractSystem.Drawer
{

    [CustomPropertyDrawer(typeof(Feature),true)]
    public class FeatureDrawer :  PropertyDrawer{

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            property.isExpanded = true;
            return EditorGUI.GetPropertyHeight(property,true) - EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ActionGUIUtil.DrawChildInContent(property, position,null,null,-1);
        }
    }

}