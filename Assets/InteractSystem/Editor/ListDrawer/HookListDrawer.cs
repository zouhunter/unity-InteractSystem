using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

namespace InteractSystem.Drawer
{
    public class HookListDrawer : ReorderListDrawer
    {
        private string title;
        public HookListDrawer(string title)
        {
            this.title = title;
        }
        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var prop = property.GetArrayElementAtIndex(index);
            var content = prop.objectReferenceValue == null ? new GUIContent("Null"): new GUIContent(prop.objectReferenceValue.GetType().FullName);
            EditorGUI.ObjectField(rect,prop, content);
        }

        protected override void DrawHeaderCallBack(Rect rect)
        {
            EditorGUI.LabelField(rect, title);
        }

        protected override float ElementHeightCallback(int index)
        {
            return EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 2;
        }
    }
}
