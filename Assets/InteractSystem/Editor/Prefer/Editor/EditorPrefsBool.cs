using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace InteractSystem.Prefer
{
    public class EditorPrefsBool : PreferValue<bool>
    {
        public EditorPrefsBool(string key) : base(key) { }

        protected override bool GetPreferValue()
        {
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefs.GetBool(key);
            }
            return false;
        }

        protected override void SetPreferValue(bool value)
        {
            EditorPrefs.SetBool(key, value);
        }
        protected override bool Equals(bool a, bool b)
        {
            return a == b;
        }
    }

}