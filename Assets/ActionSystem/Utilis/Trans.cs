using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public static class TransUtil
    {
        public static S[] FindComponentsInChild<S>(Transform root) where S : MonoBehaviour
        {
            var list = new List<S>();
            FindChild(root, list);
            return list.ToArray();
        }
        private static void FindChild<S>(Transform parent, List<S> finded)
        {
            var s = parent.GetComponent<S>();
            if (s != null) finded.Add(s);

            if (parent.childCount == 0)
            {
                return;
            }
            else
            {
                foreach (Transform item in parent)
                {
                    FindChild<S>(item, finded);
                }
            }
        }
    }

}
