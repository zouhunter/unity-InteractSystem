using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;

namespace WorldActionSystem.Graph
{
    [CustomNode("Action/Anim", 0, "ActionSystem")]
    public class AnimNode : ActionNode
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