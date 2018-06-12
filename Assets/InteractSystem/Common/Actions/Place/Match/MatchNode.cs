using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Common.Actions
{
    
    [NodeGraph. CustomNode("Operate/Match", 10, "InteratSystem")]
    public class MatchNode : PlaceNode
    {
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Place;
            }
        }
    }
}