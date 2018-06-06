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
    public abstract class BindingListDrawer : ScriptObjectListDrawer
    {
        private List<Type> _bindingTypes;
        protected List<Type> bindingTypes
        {
            get
            {
                if (_bindingTypes == null )
                {
                    _bindingTypes = LoadBindingTypes();
                }
                return _bindingTypes;
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

        protected abstract List<Type> LoadBindingTypes();

        protected void OnAddBindingItem()
        {
            var options = bindingTypes.ConvertAll(x => new GUIContent(x.FullName)).ToArray();
            Debug.Log(options.Length);
            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), options, -1, (data, ops, s) =>
            {
                if (s >= 0)
                {
                    var type = bindingTypes[s];
                    var asset = ScriptableObject.CreateInstance(type);
                    ProjectWindowUtil.CreateAsset(asset, "new_" + type.Name + ".asset");
                }
            }, null);
        }
    }
}