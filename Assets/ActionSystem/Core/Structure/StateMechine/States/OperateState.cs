using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem.Structure
{
    public class OperateState : ExecuteState
    {
        internal override void Execute(ExecuteUnit unit)
        {
            //var operateNode = unit.node as OperateNode;
            ////operateNode.onEndExecute = ExecuteAnGroup;
            ////打开下一级的步骤
            //for (int i = unit.childUnits.Count - 1; i >= 0; i--)
            //{
            //    //waitUnits.Push(unit.childUnits[i]);
            //}
            //operateNode.OnStartExecute(isForceAuto);
        }
    }
}