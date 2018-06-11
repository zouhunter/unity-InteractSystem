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
        public bool completeMoveBack = true;//结束时退回
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Place;
            }
        }
        protected override void AutoCompleteItems()
        {
            throw new NotImplementedException();
        }

    }
}