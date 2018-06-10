using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace InteractSystem.Prefer
{
    public class PlayerPrefsInt : PreferValue<int>
    {
        public PlayerPrefsInt(string key) : base(key) { }

        protected override int GetPreferValue()
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key);
            }
            return 0;
        }

        protected override void SetPreferValue(int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
        protected override bool Equals(int a, int b)
        {
            return a == b;
        }
    }

}