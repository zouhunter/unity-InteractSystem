﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using InteractSystem.Binding;

namespace InteractSystem.Drawer
{
    public abstract class ScriptObjectListDrawer : ReorderListDrawer
    {
        private SerializedObject serializedObj;
        private List<string> ignoreProps = new List<string> { "m_Script" };

        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var prop = property.GetArrayElementAtIndex(index);
            var content = prop.objectReferenceValue == null ? new GUIContent("Null") : new GUIContent(prop.objectReferenceValue.GetType().Name);

            var btnRect = new Rect(rect.x, rect.y, rect.width - ActionGUIUtil.middleButtonWidth, EditorGUIUtility.singleLineHeight);
            var objRect = new Rect(rect.x + rect.width - ActionGUIUtil.middleButtonWidth, rect.y, ActionGUIUtil.middleButtonWidth, EditorGUIUtility.singleLineHeight);

            if (GUI.Button(btnRect, content, EditorStyles.toolbarDropDown))
            {
                prop.isExpanded = !prop.isExpanded;
            }

            if (prop.objectReferenceValue != null)
            {
                if (GUI.Button(objRect, "", EditorStyles.objectFieldMiniThumb))
                {
                    EditorGUIUtility.PingObject(prop.objectReferenceValue);
                }
                DrawDragField(objRect, prop);
            }
            else
            {
                DrawObjectField(objRect, prop);
            }

            if (isFocused)
            {
                prop.isExpanded = true;
                reorderList.ReleaseKeyboardFocus();
            }

            if (prop.isExpanded && prop.objectReferenceValue != null)
            {
                DrawObjectDetail(prop.objectReferenceValue, rect);
            }
        }

        protected abstract void DrawObjectField(Rect objRect, SerializedProperty prop);
        protected abstract void DrawDragField(Rect objRect, SerializedProperty prop);
        protected abstract void DrawDragField(Rect rect);
        protected override float ElementHeightCallback(int index)
        {
            var prop = property.GetArrayElementAtIndex(index);
            var height = EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 2;
            if (prop.objectReferenceValue != null && prop.isExpanded)
            {
                var se = ActionGUIUtil.CreateCachedSerializedObject(prop.objectReferenceValue);
                height += ActionGUIUtil.GetSerializedObjectHeight(se, ignoreProps) + ActionGUIUtil.padding * 2;
            }
            return height;
        }
        public override void DoLayoutList()
        {
            base.DoLayoutList();
            var rect = ActionGUIUtil.GetDragRect();
            DrawDragField(rect);
        }
        protected void DrawObjectDetail(UnityEngine.Object obj, Rect rect)
        {
            if (obj != null)
            {
                serializedObj = ActionGUIUtil.CreateCachedSerializedObject(obj);
                rect.y += EditorGUIUtility.singleLineHeight + 5;
                ActionGUIUtil.DrawChildInContent(serializedObj.GetIterator(), rect, ignoreProps, null, 1);
            }
        }

    }
}