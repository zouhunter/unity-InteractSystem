using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;

namespace WorldActionSystem.Graph
{
    [CustomNode("Auto/Anim", 0, "ActionSystem")]
    public class AnimNode : OperateNode
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