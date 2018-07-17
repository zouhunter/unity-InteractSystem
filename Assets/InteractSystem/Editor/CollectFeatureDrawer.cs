using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace InteractSystem.Drawer
{
    [CustomPropertyDrawer(typeof(CollectNodeFeature),true)]
    public class CollectFeatureDrawer :PropertyDrawer
    {
        private SerializedProperty itemListProp;
        private ReorderableList listDrawer;
        private float defultHeight;
        private void Init(SerializedProperty property)
        {
            if(itemListProp == null)
                itemListProp = property.FindPropertyRelative("itemList");
            if (listDrawer == null)
            {
                listDrawer = new ReorderableList(property.serializedObject, itemListProp);
                listDrawer.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "执行列表"); };
                listDrawer.elementHeight = ActionGUIUtil.padding * 2 + EditorGUIUtility.singleLineHeight;
                listDrawer.drawElementCallback = DrawElement;
            }
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var prop = itemListProp.GetArrayElementAtIndex(index);
            prop.stringValue = EditorGUI.TextField(rect,prop.stringValue);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Init(property);
            defultHeight = EditorGUI.GetPropertyHeight(property, label, true) - EditorGUIUtility.singleLineHeight;
            return defultHeight + listDrawer.GetHeight();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect1 = new Rect(position.x, position.y, position.width, defultHeight);
            var rect2 = new Rect(position.x, position.y + defultHeight, position.width, position.height - defultHeight);
            listDrawer.DoList(rect2);

            var oldLevel = EditorGUI.indentLevel;
            DrawChildInContent(property, rect1, property.depth + 1,-1);
            EditorGUI.indentLevel = oldLevel;
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