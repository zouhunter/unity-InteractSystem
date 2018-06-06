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
    public class EnviromentInfoListDrawer : ReorderListDrawer
    {
        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var prop = property.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop, null, true);
        }

        protected override void DrawHeaderCallBack(Rect rect)
        {

        }

        protected override float ElementHeightCallback(int index)
        {
            var prop = property.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(prop, null, true) + ActionGUIUtil.padding * 2;
        }
    }
}