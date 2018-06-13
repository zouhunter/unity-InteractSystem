using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{ 
    [NodeGraph.CustomNode("Operate/Drag", 13, "InteratSystem")]
    public class DragNode : ClickAbleCollectNode<DragItem>
    {

        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Drag;
            }
        }
    }

}