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
        protected override void ExecuteUnStarted(ExecuteUnit unit)
        {
            base.ExecuteUnStarted(unit);
            var logicNode = unit.node as Graph.LogicNode;

            switch (logicNode.logicType)
            {
                case Graph.LogicType.And:
                    if (AllParentCompleted(unit)){
                        statusDic[unit].statu = ExecuteStatu.Executing;
                        StartExecuteChildGroups(unit);
                    }
                    break;
                case Graph.LogicType.Or:
                    statusDic[unit].statu = ExecuteStatu.Executing;
                    StartExecuteChildGroups(unit);
                    break;
                case Graph.LogicType.ExclusiveOr:
                    break;
                default:
                    break;
            }
        }
        protected override void ExecuteExecuting(ExecuteUnit unit)
        {
            base.ExecuteExecuting(unit);
            if (HaveUnitNotComplete(unit)){
                return;
            }

            if (!LunchStackGroup(unit))
            {
                statusDic[unit].statu = ExecuteStatu.Completed;
                Execute(unit);
            }
        }
        protected override void ExecuteCompleted(ExecuteUnit unit)
        {
            base.ExecuteCompleted(unit);
            Debug.Log("Logic Completed");
            stateMechine.ExecuteGroup(unit.parentUnits);
        }

        private bool AllParentCompleted(ExecuteUnit unit)
        {
            var parentList = unit.parentUnits;
            Debug.Log(parentList.Count);
            foreach (var item in parentList)
            {
                if(item.node is Graph.OperateNode)
                {
                    var node = item.node as Graph.OperateNode;
                    Debug.Log(node+ ": statu:" + node.Statu);
                    if(node.Statu != ExecuteStatu.Completed)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}