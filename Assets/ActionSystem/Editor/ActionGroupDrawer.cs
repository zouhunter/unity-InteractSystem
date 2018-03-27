using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace WorldActionSystem
{

    [CustomEditor(typeof(ActionGroup)), CanEditMultipleObjects]
    public class ActionGroupDrawer : ActionGroupDrawerBase
    {
        protected override void RemoveDouble()
        {
            var actionSystem = target as ActionGroup;
            var newList = new List<AutoPrefabItem>();
            var needRemove = new List<AutoPrefabItem>();
            foreach (var item in actionSystem.autoLoadElement)
            {
                if (newList.Find(x => x.ID == item.ID) == null)
                {
                    newList.Add(item);
                }
                else
                {
                    needRemove.Add(item);
                }
            }
            foreach (var item in needRemove)
            {
                actionSystem.autoLoadElement.Remove(item);
            }
            EditorUtility.SetDirty(actionSystem);
        }
    }
}