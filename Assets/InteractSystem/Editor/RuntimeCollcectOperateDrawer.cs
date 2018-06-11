using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using System;

namespace InteractSystem.Drawer
{
    public abstract class RuntimeCollcectOperateDrawer : OperateNodeDrawer
    {
        protected override void OnDrawDefult()
        {
            DrawItemList();
            base.OnDrawDefult();
        }
        private SerializedProperty itemList_prop;
        private ReorderableList reorderList;

        protected override void InitPropertys()
        {
            base.InitPropertys();
            itemList_prop = serializedObject.FindProperty("itemList");
        }
        protected override void InitDrawers()
        {
            base.InitDrawers();
            reorderList = new ReorderableList(serializedObject, itemList_prop);
            reorderList.drawHeaderCallback = DrawHead;
            reorderList.drawElementCallback = DrawElement;
            reorderList.elementHeight = EditorGUIUtility.singleLineHeight + 2 * ActionGUIUtil.padding;
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var prop = itemList_prop.GetArrayElementAtIndex(index);
            prop.stringValue = EditorGUI.TextField(rect, prop.stringValue);
        }

        protected abstract void DrawHead(Rect rect);

        protected  void DrawItemList()
        {
            reorderList.DoLayoutList();
        }
    }

    [CustomEditor(typeof(ClickAbleActionNode), true)]
    public class RuntimeOrderCollectNodeDrawer : RuntimeCollcectOperateDrawer
    {
        protected override void DrawHead(Rect rect)
        {
            EditorGUI.LabelField(rect, "点击列表（顺序）");
        }
    }

    //[CustomEditor(typeof(RuntimeDisorderCollectNode<>), true)]
    //public class RuntimeDisorderCollectNodeDrawer : RuntimeCollcectOperateDrawer
    //{
    //    protected override void DrawHead(Rect rect)
    //    {
    //        EditorGUI.LabelField(rect, "点击列表（无序）");
    //    }
    //}

}