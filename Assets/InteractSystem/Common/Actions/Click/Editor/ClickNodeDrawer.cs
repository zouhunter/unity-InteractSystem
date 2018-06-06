using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using InteractSystem.Common.Actions;
using UnityEditorInternal;
using System;

namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(ClickNode))]
    public class ClickNodeDrawer : OperateNodeDrawer
    {
        SerializedProperty clickList_prop;
        ReorderableList reorderList;
        protected override void InitPropertys()
        {
            base.InitPropertys();
            clickList_prop = serializedObject.FindProperty("clickList");
        }
        protected override void InitDrawers()
        {
            base.InitDrawers();
            reorderList = new ReorderableList(serializedObject, clickList_prop);
            reorderList.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "点击列表（顺序）"); };
            reorderList.drawElementCallback = DrawElement;
            reorderList.elementHeight = EditorGUIUtility.singleLineHeight + 2 * ActionGUIUtil.padding;
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var prop = clickList_prop.GetArrayElementAtIndex(index);
            prop.stringValue = EditorGUI.TextField(rect, prop.stringValue);
        }

        protected override void DrawDefult()
        {
            reorderList.DoLayoutList();
        }
    }

}