using System;
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
    public class CommandBindingListDrawer : BindingListDrawer
    {
        private List<Binding.CommandBinding> dragBindings = new List<Binding.CommandBinding>();

        public CommandBindingListDrawer(string title) {
            this.title = title;
        }

        protected override void DrawHeaderCallBack(Rect rect)
        {
            base.DrawHeaderCallBack(rect);
            EditorGUI.LabelField(rect, title);
        }

        protected override List<Type> LoadBindingTypes()
        {
            return Utility.GetSubInstenceTypes(typeof(Binding.CommandBinding));

        }
        protected override void DrawDragField(Rect rect)
        {
            if (Event.current.type == EventType.DragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".asset", dragBindings);
            }
            else if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragBindings)
                {
                    property.InsertArrayElementAtIndex(property.arraySize);
                    var prop = property.GetArrayElementAtIndex(property.arraySize - 1);
                    prop.objectReferenceValue = item;
                }
            }
        }
        protected override void DrawDragField(Rect objRect, SerializedProperty prop)
        {
            if (Event.current.type == EventType.DragUpdated && objRect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".asset", dragBindings);
            }

            else if (Event.current.type == EventType.DragPerform && objRect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragBindings)
                {
                    prop.objectReferenceValue = item;
                    break;
                }
            }
        }

        protected override void DrawObjectField(Rect objRect, SerializedProperty prop)
        {
            prop.objectReferenceValue = EditorGUI.ObjectField(objRect, prop.objectReferenceValue, typeof(Binding.CommandBinding), false);
        }
    }


}