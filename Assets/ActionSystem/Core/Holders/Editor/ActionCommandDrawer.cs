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
        private ActionCommand command { get { return target as ActionCommand; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            InitCommandGraph();
        }


        private void InitCommandGraph()
        {
            if (command.graphObj == null)
            {
                command.graphObj = ScriptableObject.CreateInstance<NodeGraph.DataModel.NodeGraphObj>();
                command.graphObj.ControllerType = typeof(Graph.AcionGraphCtrl).FullName;
                Debug.Log("Create New :" + command.graphObj);
                NodeGraph.ScriptableObjUtility.AddSubAsset(command.graphObj, command);
                EditorUtility.SetDirty(command);
            }
        }
    }
}