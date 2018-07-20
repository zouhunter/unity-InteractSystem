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
        public NodeListDrawer(string title = null) : base(title) { }
        public override void InitReorderList(IList list, Type type)
        {
            base.InitReorderList(list, type);
            reorderList.displayRemove = false;
            reorderList.displayAdd = false;
            reorderList.elementHeightCallback = CalcuteNodeHeightCallback;
        }

        protected float CalcuteNodeHeightCallback(int index)
        {
            var height = EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 2;
            return height;
        }

        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var item = list[index] as Graph.OperaterNode;
            var rect0 = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.ObjectField(rect0, item.Name, item, typeof(Graph.OperaterNode), false);
            if(isFocused && item)
            {
                EditorGUIUtility.PingObject(item);
            }
          
        }


        protected override float ElementHeightCallback(int index)
        {
            return reorderList.elementHeight;
        }
    }
}
