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
    public class NodeListDrawer : ReorderListDrawer
    {
        protected Editor drawer;
        public override void InitReorderList(IList list, Type type)
        {
            base.InitReorderList(list, type);
            reorderList.displayRemove = false;
            reorderList.displayAdd = false;
            reorderList.elementHeight = EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 2;
        }
        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var item = list[index] as Graph.OperaterNode;
            EditorGUI.ObjectField(rect, item.Name, item, typeof(Graph.OperaterNode), false);
            if (isActive)
            {
                Editor.CreateCachedEditor(item, typeof(OperateNodeDrawer), ref drawer);
                drawer.OnInspectorGUI();
            }
        }
        protected override void DrawHeaderCallBack(Rect rect)
        {
            EditorGUI.LabelField(rect, "");
        }
        protected override float ElementHeightCallback(int index)
        {
            return reorderList.elementHeight;
        }
    }
}
