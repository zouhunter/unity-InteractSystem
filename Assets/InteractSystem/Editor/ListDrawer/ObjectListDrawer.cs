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
    public class ObjectListDrawer<T> : ReorderListDrawer where T:UnityEngine.Object
    {
        public string titleFormat { get; set; }
        public ObjectListDrawer(string title) : base(title) { }

        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, "");
            var prop = property.GetArrayElementAtIndex(index);
            var label = string.IsNullOrEmpty(titleFormat) ? "第" + index + "个节点" : string.Format(titleFormat,index);
            EditorGUI.PropertyField(rect, prop, new GUIContent(label));

            if(isFocused && Event.current.type == EventType.MouseUp)
            {
                EditorGUIUtility.PingObject(prop.objectReferenceValue);
            }
        }

        protected override float ElementHeightCallback(int index)
        {
            return EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 2f;
        }
    }
}