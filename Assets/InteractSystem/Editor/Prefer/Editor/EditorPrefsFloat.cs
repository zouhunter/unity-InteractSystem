using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace InteractSystem.Prefer
{
    public class EditorPrefsFloat : PreferValue<float>
    {
        public EditorPrefsFloat(string key) : base(key) { }

        protected override float GetPreferValue()
        {
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefs.GetFloat(key);
            }
            return 0;
        }

        protected override void SetPreferValue(float value)
        {
            EditorPrefs.SetFloat(key, value);
        }
        protected override bool Equals(float a, float b)
        {
            return Mathf.Abs(a - b) < 0.01f;
        }
    }

}