using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;

namespace InteractSystem.Graph
{
    [CustomNode("Logic", 0, "InteratSystem")]
    public class LogicNode : ActionNode
    {
        public LogicType logicType;

        public override void Initialize(NodeData data)
        {
            base.Initialize(data);
            if(data.InputPoints == null || data.InputPoints.Count == 0)
            {
                data.AddInputPoint("-", "actionconnect", 100);
            }
            if (data.OutputPoints == null || data.OutputPoints.Count == 0)
            {
                data.AddOutputPoint("0", "actionconnect", 100);
            }
        }
    }
}