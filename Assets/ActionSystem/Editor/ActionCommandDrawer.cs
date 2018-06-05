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


        //[UnityEditor.Callbacks.OnOpenAsset]
        //static bool QuickOpen(int instanceID, int line)
        //{
        //    var command = EditorUtility.InstanceIDToObject(instanceID) as ActionCommand;
        //    if (command != null)
        //    {
        //        if (command.GraphObj == null)
        //        {
        //            CreateCommandGraph(command);
        //        }
        //        var window = EditorWindow.GetWindow<NodeGraph.NodeGraphWindow>();
        //        window.OpenGraph(command.GraphObj);
        //        return true;
        //    }
        //    return false;
        //}

        public static void CreateCommandGraph(ActionCommand command )
        {
            NodeGraph.ScriptableObjUtility.ClearSubAsset(command);
            var graphObj = ScriptableObject.CreateInstance<NodeGraph.DataModel.NodeGraphObj>();
            graphObj.ControllerType = typeof(Graph.AcionGraphCtrl).FullName;
            Debug.Log("Create New :" + graphObj);
            command.GetType().InvokeMember("_graphObj", System.Reflection.BindingFlags.SetField | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
               null, command, new object[] { graphObj });
            NodeGraph.ScriptableObjUtility.AddSubAsset(graphObj, command);
            EditorUtility.SetDirty(command);
        }
    }
}