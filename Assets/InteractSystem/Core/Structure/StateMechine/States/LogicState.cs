using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Structure
{
    public class LogicState : ExecuteState
    {
        protected override void ExecuteOnUnStarted(ExecuteUnit unit)
        {
            base.ExecuteOnUnStarted(unit);
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
        protected override void ExecuteOnExecuting(ExecuteUnit unit)
        {
            base.ExecuteOnExecuting(unit);
            if (HaveUnitNotComplete(unit)){
                return;
            }

            if (!LunchStackGroup(unit))
            {
                statusDic[unit].statu = ExecuteStatu.Completed;
                Execute(unit);
            }
        }
        protected override void ExecuteOnCompleted(ExecuteUnit unit)
        {
            base.ExecuteOnCompleted(unit);
            Debug.Log("Logic Completed");
            stateMechine.ExecuteGroup(unit.parentUnits);
        }
        public override void Complete(ExecuteUnit unit)
        {
            base.Complete(unit);
            if (statusDic[unit].statu != ExecuteStatu.Completed)
            {
                CompleteExecuteChildGroups(unit);
                statusDic[unit].statu = ExecuteStatu.Completed;
            }
        }


        public override void UnDo(ExecuteUnit unit)
        {
            base.UnDo(unit);
            if (statusDic[unit].statu != ExecuteStatu.UnStarted)
            {
                UndoExecuteChildGroups(unit);
                statusDic[unit].statu = ExecuteStatu.UnStarted;
            }
        }
        private bool AllParentCompleted(ExecuteUnit unit)
        {
            var parentList = unit.parentUnits;
            Debug.Log(parentList.Count);
            foreach (var item in parentList)
            {
                if (item.node is Graph.OperateNode)
                {
                    var node = item.node as Graph.OperateNode;
                    Debug.Log(node + ": statu:" + node.Statu);
                    if (node.Statu != ExecuteStatu.Completed)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}