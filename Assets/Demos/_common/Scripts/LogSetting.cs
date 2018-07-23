using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractSystem;
using InteractSystem.Graph;
using InteractSystem.Actions;
using InteractSystem.Binding;
using InteractSystem.Hooks;
using InteractSystem.Structure;
using NodeGraph;
using NodeGraph.DataModel;

public class LogSetting : MonoBehaviour {
    public bool nodeGraphObj_log;
    public bool operateNode_log;
    public bool actionItem_log;
    public bool actionHook_log;
    public bool feature_log;
    public bool actionItemBinding_log;
    public bool elementCtrl_Log;
	void Awake () {
        ActionItem.log = actionItem_log;
        NodeGraph.DataModel.NodeGraphObj.log = nodeGraphObj_log;
        ActionHook.log = actionHook_log;
        OperaterNode.log = operateNode_log;
        Feature.log = feature_log;
        ActionItemBinding.log = actionItemBinding_log;
        ElementController.log = elementCtrl_Log;
    }
	
}
