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
    public class OperaterBindingListDrawer : BindingListDrawer
    {
        protected List<Binding.OperaterBinding> dragBindings = new List<Binding.OperaterBinding>();

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

        protected override void DrawDragField(Rect objRect)
        {
            if (Event.current.type == EventType.DragUpdated && objRect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".asset", dragBindings);
            }

            else if (Event.current.type == EventType.DragPerform && objRect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragBindings)
                {
                    var prop = property.AddItem();
                    prop.objectReferenceValue = item;
                    break;
                }
            }
        }

        protected override void DrawObjectField(Rect objRect, SerializedProperty prop)
        {
            prop.objectReferenceValue = EditorGUI.ObjectField(objRect, prop.objectReferenceValue, typeof(Binding.OperaterBinding), false);
        }

        protected override List<Type> LoadBindingTypes()
        {
           return Utility.GetSubInstenceTypes(typeof(Binding.OperaterBinding));
        }
    }
}