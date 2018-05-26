using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;

namespace WorldActionSystem.Graph
{
    [CustomNode("Logic", 0, "ActionSystem")]
    public class LogicNode : NodeGraph.DataModel.Node
    {
        public override void Initialize(NodeData data)
        {
            base.Initialize(data);
            if(data.InputPoints == null || data.InputPoints.Count == 0)
            {
                data.AddInputPoint("-", "actionconnect", 100);
            }
            if (data.OutputPoints == null || data.OutputPoints.Count == 0)
            {
                data.AddOutputPoint("+", "actionconnect", 1);
            }
        }
    }
}