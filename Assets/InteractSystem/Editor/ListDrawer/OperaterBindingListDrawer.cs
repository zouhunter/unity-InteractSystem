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
    public class OperaterBindingListDrawer : ReorderListDrawer
    {
        private List<Type> _operateBindingTypes;
        protected List<Type> operateBindingTypes
        {
            get
            {
                if (_operateBindingTypes == null || _operateBindingTypes.Count == 0)
                {
                    _operateBindingTypes = typeof(ActionGroup).Assembly.GetTypes().
                        Where(x => x.IsSubclassOf(typeof(Binding.OperaterBinding))).ToList();
                }
                return _operateBindingTypes;
            }

        }
        private List<Binding.OperaterBinding> dragBindings = new List<Binding.OperaterBinding>();
        private float elementHeight = EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 2;
        private Editor drawer;

        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var prop = property.GetArrayElementAtIndex(index);
            var content = prop.objectReferenceValue == null ? new GUIContent("Null") : new GUIContent(prop.objectReferenceValue.GetType().Name);
            EditorGUI.PropertyField(rect, prop, content);
            if(isActive)
            {
                if(prop.objectReferenceValue != null)
                {
                    DrawOperateBindingDetail(prop.objectReferenceValue as Binding.OperaterBinding);
                }
            }
        }

        protected override void DrawHeaderCallBack(Rect rect)
        {
            var btnRect = new Rect(rect.x + rect.width - ActionGUIUtil.bigButtonWidth, rect.y, ActionGUIUtil.bigButtonWidth, rect.height);
            if (GUI.Button(btnRect, "new", EditorStyles.miniButtonRight))
            {
                OnAddBindingItem();
            }
        }

        protected override float ElementHeightCallback(int index)
        {
            return elementHeight;
        }
        public override void DoLayoutList()
        {
            base.DoLayoutList();
            var rect = ActionGUIUtil.GetDragRect();

            if (Event.current.type == EventType.dragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".asset", dragBindings);
            }
            else if (Event.current.type == EventType.dragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragBindings)
                {
                    property.InsertArrayElementAtIndex(property.arraySize);
                    var prop = property.GetArrayElementAtIndex(property.arraySize - 1);
                    prop.objectReferenceValue = item;
                }
            }
        }
        protected void DrawOperateBindingDetail(Binding.OperaterBinding operaterBinding)
        {
            Editor.CreateCachedEditor(operaterBinding, typeof(Editor), ref drawer);
            drawer.OnInspectorGUI();
        }
        private void OnAddBindingItem()
        {
            var options = operateBindingTypes.ConvertAll(x => new GUIContent(x.FullName)).ToArray();
            Debug.Log(options.Length);
            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), options, -1, (data, ops, s) =>
            {
                if (s >= 0)
                {
                    var type = operateBindingTypes[s];
                    var asset = ScriptableObject.CreateInstance(type);
                    ProjectWindowUtil.CreateAsset(asset, "new_" + type.Name + ".asset");
                }
            }, null);
        }
    }
}