using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace InteractSystem.Prefer
{
    public class PlayerPrefsFloat : PreferValue<float>
    {
        public PlayerPrefsFloat(string key) : base(key) { }

        protected override float GetPreferValue()
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetFloat(key);
            }
            return 0;
        }

        protected override void SetPreferValue(float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
        protected override bool Equals(float a, float b)
        {
            return Mathf.Abs(a - b) < 0.01f;
        }
    }

}