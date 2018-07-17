using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InteractSystem.Drawer
{

    [CustomPropertyDrawer(typeof(Feature), true)]
    public class FeatureDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            property.isExpanded = true;
            return EditorGUI.GetPropertyHeight(property, true) - EditorGUIUtility.singleLineHeight - 2.5f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var oldlevel = EditorGUI.indentLevel;
            DrawChildInContent(property, position, property.depth + 1, -1);
            EditorGUI.indentLevel = oldlevel;
        }

        /// <summary>
        /// 绘制指定个数据的property
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="position"></param>
        /// <param name="level"></param>
        public virtual void DrawChildInContent(SerializedProperty serializedProperty, Rect position, int deepth, int level = 0)
        {
            bool enterChildren = true;
            while (serializedProperty.NextVisible(enterChildren))
            {
                if (serializedProperty.depth < deepth)
                {
                    break;
                }

                EditorGUI.indentLevel = serializedProperty.depth + level;
                position.height = EditorGUI.GetPropertyHeight(serializedProperty, null, true);
                EditorGUI.PropertyField(position, serializedProperty, true);
                position.y += position.height + 2f;
                enterChildren = false;
            }
        }
    }

}