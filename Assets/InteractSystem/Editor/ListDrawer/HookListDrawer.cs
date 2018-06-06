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
    public class HookListDrawer : ScriptObjectListDrawer
    {
        private string title;
        private List<ActionHook> dragHooks = new List<ActionHook>();
        public HookListDrawer(string title)
        {
            this.title = title;
        }

      
        protected override void DrawObjectField(Rect objRect, SerializedProperty prop)
        {
            prop.objectReferenceValue = EditorGUI.ObjectField(objRect, prop.objectReferenceValue, typeof(ActionHook), false);
        }

        protected override void DrawDragField(Rect objRect, SerializedProperty prop)
        {
            if (Event.current.type == EventType.dragUpdated && objRect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects<ActionHook>(".asset", dragHooks);
            }

            else if (Event.current.type == EventType.DragPerform && objRect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragHooks)
                {
                    prop.objectReferenceValue = item;
                    break;
                }
            }
        }
        protected override void DrawHeaderCallBack(Rect rect)
        {
            base.DrawHeaderCallBack(rect);
            var labelRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, title);
        }
        protected override void DrawDragField(Rect rect)
        {

            if (Event.current.type == EventType.dragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects<ActionHook>(".asset", dragHooks);
            }

            else if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragHooks)
                {
                    var prop = property.AddItem();
                    prop.objectReferenceValue = item;
                    break;
                }
            }
        }
    }
}
