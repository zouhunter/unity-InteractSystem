using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace InteractSystem.Prefer
{
    public class EditorPrefsString : PreferValue<string>
    {
        public EditorPrefsString(string key) : base(key) { }

        protected override string GetPreferValue()
        {
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefs.GetString(key);
            }
            return "";
        }

        protected override void SetPreferValue(string value)
        {
            EditorPrefs.SetString(key, value);
        }
        protected override bool Equals(string a, string b)
        {
            return a == b;
        }
    }

}