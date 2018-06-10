using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace InteractSystem.Prefer
{
    public class EditorPrefsInt : PreferValue<int>
    {
        public EditorPrefsInt(string key) : base(key) { }

        protected override int GetPreferValue()
        {
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefs.GetInt(key);
            }
            return 0;
        }

        protected override void SetPreferValue(int value)
        {
            EditorPrefs.SetInt(key, value);
        }
        protected override bool Equals(int a, int b)
        {
            return a == b;
        }
    }

}