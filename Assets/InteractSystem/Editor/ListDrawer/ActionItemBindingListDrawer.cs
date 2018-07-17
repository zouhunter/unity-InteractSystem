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
    public class ActionItemBindingListDrawer : BindingListDrawer
    {
        private List<Binding.ActionItemBinding> dragBindings = new List<Binding.ActionItemBinding>();

        public ActionItemBindingListDrawer(string title) {
            this.title = title;
        }

        protected override List<Type> LoadBindingTypes()
        {
            return Utility.GetSubInstenceTypes(typeof(Binding.ActionItemBinding));

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
            prop.objectReferenceValue = EditorGUI.ObjectField(objRect, prop.objectReferenceValue, typeof(Binding.ActionItemBinding), false);
        }
    }


}