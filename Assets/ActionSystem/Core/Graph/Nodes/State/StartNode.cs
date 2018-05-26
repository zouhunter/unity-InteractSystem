using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;
using System;

namespace WorldActionSystem.Graph
{
    [CustomNode("State/Start", 0, "ActionSystem")]
    public class StartNode : ActionNode
    {
        public override void Initialize(NodeData data)
        {
            base.Initialize(data);
            if (data.OutputPoints == null || data.OutputPoints.Count ==0)
            {
                data.AddOutputPoint("0", "actionconnect", 100);
            }
        }
    }
}