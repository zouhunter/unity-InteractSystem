using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace InteractSystem.Drawer
{
    public class ActionNoticeListDrawer : ScriptObjectListDrawer
    {
        protected List<Notice.ActionNotice> dragnotices = new List<Notice.ActionNotice>();
        public ActionNoticeListDrawer(string title) : base(title) { }
        private List<Type> _supportTypes;
        protected override List<Type> supportTypes
        {
            get
            {
                if(_supportTypes == null)
                {
                    _supportTypes = Utility.GetSubInstenceTypes(typeof(Notice.ActionNotice));
                }
                return _supportTypes;
            }
        }

        protected override void DrawDragField(Rect rect, SerializedProperty property)
        {
            if (Event.current.type == EventType.DragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".asset", dragnotices);
            }
            else if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragnotices)
                {
                    property.InsertArrayElementAtIndex(property.arraySize);
                    var prop = property.GetArrayElementAtIndex(property.arraySize - 1);
                    prop.objectReferenceValue = item;
                }
            }
        }

        protected override void DrawDragField(Rect rect)
        {
            if (Event.current.type == EventType.DragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".asset", dragnotices);
            }
            else if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragnotices)
                {
                    property.InsertArrayElementAtIndex(property.arraySize);
                    var prop = property.GetArrayElementAtIndex(property.arraySize - 1);
                    prop.objectReferenceValue = item;
                }
            }
        }

        protected override void DrawObjectField(Rect objRect, SerializedProperty prop)
        {
            prop.objectReferenceValue = EditorGUI.ObjectField(objRect, prop.objectReferenceValue, typeof(Notice.ActionNotice), false);
        }
    }
}
