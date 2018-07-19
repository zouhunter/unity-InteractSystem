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
        protected override List<Type> supportTypes
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

        protected abstract List<Type> LoadBindingTypes();
        
    }
}