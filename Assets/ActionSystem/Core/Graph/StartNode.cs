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
        public override ControllerType CtrlType
        {
            get
            {
                return 0;
            }
        }
    }
}