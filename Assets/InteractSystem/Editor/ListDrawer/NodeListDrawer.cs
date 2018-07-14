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
        protected OperateNodeDrawer drawer;
        protected Dictionary<Graph.OperaterNode, Editor> drawerDic = new Dictionary<Graph.OperaterNode, Editor>();
        protected Dictionary<Graph.OperaterNode, float> rectDic = new Dictionary<Graph.OperaterNode, float>();
        public override void InitReorderList(IList list, Type type)
        {
            base.InitReorderList(list, type);
            reorderList.displayRemove = false;
            reorderList.displayAdd = false;
            reorderList.elementHeightCallback = CalcuteNodeHeightCallback;
        }

        protected float CalcuteNodeHeightCallback(int index)
        {
            var item = list[index] as Graph.OperaterNode;
            if(item != null && rectDic.ContainsKey(item))
            {
                return rectDic[item];
            }
            var height = EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 2;
            return height;
        }

        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var item = list[index] as Graph.OperaterNode;
            EditorGUI.ObjectField(rect, item.Name, item, typeof(Graph.OperaterNode), false);
            if (isActive && item as Graph.OperaterNode != null)
            {
                drawer = GetEditor(item as Graph.OperaterNode) as OperateNodeDrawer;
                if(drawer!=null)
                {
                   var rect0 = drawer.OnDrawDefult(rect.x,rect.y + EditorGUIUtility.singleLineHeight,rect.width, 0);
                    rectDic[item] = rect0;
                }
            }
        }
        private Editor GetEditor(Graph.OperaterNode node) 
        {
            if (!drawerDic.ContainsKey(node)||drawerDic[node] == null)
            {
                 drawerDic[node] = Editor.CreateEditor(node);// Editor.CreateEditor(node);
            }
            return drawerDic[node] ;
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
