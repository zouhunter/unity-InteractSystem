using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace InteractSystem.Prefer
{
    public class PlayerPrefsString : PreferValue<string>
    {
        public PlayerPrefsString(string key) : base(key) { }

        protected override string GetPreferValue()
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetString(key);
            }
            return "";
        }

        protected override void SetPreferValue(string value)
        {
            PlayerPrefs.SetString(key, value);
        }
        protected override bool Equals(string a, string b)
        {
            return a == b;
        }
    }

}