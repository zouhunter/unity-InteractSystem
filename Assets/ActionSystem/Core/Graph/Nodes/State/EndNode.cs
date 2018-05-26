using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;

namespace WorldActionSystem.Graph
{
    [CustomNode("State/End", 1, "ActionSystem")]
    public class EndNode: Node
    {
        public override void Initialize(NodeData data)
        {
            base.Initialize(data);
            if (data.InputPoints == null || data.InputPoints.Count == 0){
                data.AddInputPoint("->", "actionconnect", 100);
            }
        }
    }
}