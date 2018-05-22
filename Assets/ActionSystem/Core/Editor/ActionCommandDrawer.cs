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
    [CustomEditor(typeof(ActionCommand)), CanEditMultipleObjects]
    public class ActionCommandDrawer : Editor
    {
        private void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //if (Selection.gameObjects.Length == 1)
            //{
            //    serializedObject.Update();
            //    ModifyType();
            //    serializedObject.ApplyModifiedProperties();
            //}
        }
    

        private void ModifyType()
        {
            var cmd = target as ActionCommand;
            var actionObjs = cmd.GetComponentsInChildren<ActionObj>(true);
            if (actionObjs != null)
            {
                foreach (var item in actionObjs) {
                }
            }
        }
    }
}