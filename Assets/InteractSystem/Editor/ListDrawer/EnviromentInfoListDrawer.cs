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
    public class EnviromentInfoListDrawer : ReorderListDrawer
    {
        private List<GameObject> dragedObjects = new List<GameObject>();
        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var prop = property.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop, null, true);
            if (isFocused)
            {
                prop.isExpanded = true;
                reorderList.ReleaseKeyboardFocus();
            }
        }

        protected override void DrawHeaderCallBack(Rect rect)
        {

        }

        protected override float ElementHeightCallback(int index)
        {
            var prop = property.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(prop, null, true) + ActionGUIUtil.padding * 2;
        }
        public override void DoLayoutList()
        {
            base.DoLayoutList();
            var rect = ActionGUIUtil.GetDragRect();
            if (Event.current.type == EventType.DragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".prefab", dragedObjects);
            }
            else if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragedObjects)
                {
                    var path = AssetDatabase.GetAssetPath(item);
                    if (string.IsNullOrEmpty(path)) return;
                    var guid = AssetDatabase.AssetPathToGUID(path);
                    if (!ActionEditorUtility.HaveElement(property, "guid", guid))
                    {
                        var prop = property.AddItem();
                        var guidProp = prop.FindPropertyRelative("guid");
                        var enviromentNameProp = prop.FindPropertyRelative("enviromentName");
                        var coordinateProp = prop.FindPropertyRelative("coordinate");

                        guidProp.stringValue = guid;
                        enviromentNameProp.stringValue = item.name;
                        ActionEditorUtility.SaveCoordinatesInfo(coordinateProp, (item as GameObject).transform);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("警告", "预制体重复,无法添加:" + item.name, "ok");
                    }
                }
            }
        }
    }
}