using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem.Structure
{
    public class LogicState : ExecuteState
    {
        internal override void Execute(ExecuteUnit unit)
        {
            //Debug.Log("执行到逻辑节点");
            //var logicNode = unit.node as LogicNode;
            //switch (logicNode.logicType)
            //{
            //    case LogicType.And:
            //        break;
            //    case LogicType.Or:
            //        break;
            //    case LogicType.ExclusiveOr:
            //        break;
            //    default:
            //        break;
            //}
        }
    }
}