using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem.Binding
{
    public class GroupHide
    {
        private readonly static List<string> HideKeys = new List<string>();
        public static void Record(string key)
        {
            if (string.IsNullOrEmpty(key)) return;

            if (!HideKeys.Contains(key))
            {
                HideKeys.Add(key);
            }
        }
        public static bool Contains(string key)
        {
            return HideKeys.Contains(key);
        }
        public static void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) return;

            if (HideKeys.Contains(key))
            {
                HideKeys.Remove(key);
            }
        }
    }

  
}
