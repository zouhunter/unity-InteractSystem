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
           ActionGUIUtil.DrawScriptablesMenu(bindingTypes);
        }
    }
}