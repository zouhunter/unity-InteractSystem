using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
    [NodeGraph.CustomNode("Operate/Connect", 10, "InteratSystem")]
    public class ConnectNode : CompleteAbleCollectNode<ConnectItem>,IRuntimeCtrl
    {
        public ControllerType CtrlType
        {
            get
            {
                return ControllerType.Connect;
            }
        }
    }
}