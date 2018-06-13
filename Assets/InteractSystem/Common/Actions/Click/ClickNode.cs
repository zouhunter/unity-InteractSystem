using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
namespace InteractSystem.Common.Actions
{
    [CustomNode("Operate/Click", 12, "InteratSystem")]
    public class ClickNode : ClickAbleCollectNode<ClickItem>
    {
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Click;
            }
        }
    }
}