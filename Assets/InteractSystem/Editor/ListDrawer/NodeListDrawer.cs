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
        protected Dictionary<int, float> rectDic = new Dictionary<int, float>();
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
            var item = list[index] as Graph.OperaterNode;
            if(item != null && rectDic.ContainsKey(index))
            {
                return rectDic[index];
            }
            var height = EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 2;
            return height;
        }

        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var item = list[index] as Graph.OperaterNode;
            var rect0 = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.ObjectField(rect0, item.Name, item, typeof(Graph.OperaterNode), false);
            var rect1 = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, rect.height - EditorGUIUtility.singleLineHeight);
            if (isActive && item as Graph.OperaterNode != null)
            {
                drawer = GetEditor(item as Graph.OperaterNode) as OperateNodeDrawer;
                if(drawer!=null)
                {
                    drawer.serializedObject.Update();
                    var height = drawer.OnDrawDefult(rect1.x, rect1.y, rect1.width, 0);
                    rectDic[index] = height;
                    drawer.OnDrawBindings();
                    drawer.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                if(rectDic.ContainsKey(index)) {
                    rectDic.Remove(index);
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

        protected override float ElementHeightCallback(int index)
        {
            return reorderList.elementHeight;
        }
    }
}
