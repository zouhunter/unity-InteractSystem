using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem.Common.Actions
{
    [NodeGraph.CustomNode("Operate/Detach", 16, "InteratSystem")]
    public class DetachNode : ClickAbleCollectNode<DetachItem>
    {
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Detach;
            }
        }
    }
}
