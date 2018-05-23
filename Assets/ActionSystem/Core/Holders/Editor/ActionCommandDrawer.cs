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

        [UnityEditor.Callbacks.OnOpenAsset]
        static bool QuickOpen(int instanceID, int line)
        {
            var command = EditorUtility.InstanceIDToObject(instanceID) as ActionCommand;
            if (command != null)
            {
                if(command.graphObj==null){
                   CreateCommandGraph(command);
                }
                var window = EditorWindow.GetWindow<NodeGraph.NodeGraphWindow>();
                window.OpenGraph(command.graphObj);
                return true;
            }
            return false;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        public static void CreateCommandGraph(ActionCommand command )
        {
            NodeGraph.ScriptableObjUtility.ClearSubAsset(command);
            command.graphObj = ScriptableObject.CreateInstance<NodeGraph.DataModel.NodeGraphObj>();
            command.graphObj.ControllerType = typeof(Graph.AcionGraphCtrl).FullName;
            Debug.Log("Create New :" + command.graphObj);
            NodeGraph.ScriptableObjUtility.AddSubAsset(command.graphObj, command);
            EditorUtility.SetDirty(command);
        }
    }
}