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
    public class EnviromentItemListDrawer : ReorderListDrawer
    {
        protected List<GameObject> dragedGameObject = new List<GameObject>();

        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var prop = property.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop, true);

            var objRect = new Rect(rect.x + rect.width - ActionGUIUtil.bigButtonWidth, rect.y, ActionGUIUtil.bigButtonWidth, rect.height);

            if (Event.current.type == EventType.DragUpdated && objRect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".prefab", dragedGameObject);
            }
            else if (Event.current.type == EventType.DragPerform && objRect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragedGameObject)
                {
                    if (!property.HaveElement("prefab", item))
                    {
                        var eprop = property.AddItem();
                        var prefabProp = eprop.FindPropertyRelative("prefab");
                        prefabProp.objectReferenceValue = item;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("警告", "预制体重复,无法添加:" + item.name, "ok");
                    }
                }
            }
        }
        protected override void DrawHeaderCallBack(Rect rect)
        {
            base.DrawHeaderCallBack(rect);
            var btnRect = new Rect(rect.x + rect.width - ActionGUIUtil.bigButtonWidth, rect.y, ActionGUIUtil.bigButtonWidth, rect.height);
            if (GUI.Button(btnRect,"analysis"))
            {
                AnalysisFromNodes();
            }
        }
        protected override float ElementHeightCallback(int index)
        {
            var prop = property.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(prop, null, true) + ActionGUIUtil.padding * 2;
        }
        public override void DoLayoutList()
        {
            base.DoLayoutList();
            var rect = ActionGUIUtil. GetDragRect();
            if (Event.current.type == EventType.dragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".prefab", dragedGameObject);
            }
            else if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragedGameObject)
                {
                    if (!ActionEditorUtility.HaveElement(property, "prefab", item))
                    {
                        var prop = property.AddItem();
                        var prefabProp = prop.FindPropertyRelative("prefab");
                        var _name_prop = prop.FindPropertyRelative("_name");
                        _name_prop.stringValue = item.name;
                        prefabProp.objectReferenceValue = item;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("警告", "预制体重复,无法添加:" + item.name, "ok");
                    }
                }
            }
        }

        protected void AnalysisFromNodes()
        {
            Debug.LogWarning("节点中解析相应的元素");
        }
    }
}
