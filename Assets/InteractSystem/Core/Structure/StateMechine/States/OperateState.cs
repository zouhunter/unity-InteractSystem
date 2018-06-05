using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using OperateNode = InteractSystem.Graph.OperateNode;

namespace InteractSystem.Structure
{
    public class OperateState : ExecuteState
    {
        /// <summary>
        /// 首次执行
        /// </summary>
        /// <param name="unit"></param>
        protected override void ExecuteOnUnStarted(ExecuteUnit unit)
        {
            base.ExecuteOnUnStarted(unit);
            statusDic[unit].statu = ExecuteStatu.Executing;
            var operateNode = unit.node as OperateNode;

            //判断是不是叶节点
            var leaf = unit.childUnits.Count == 0;

            if (!leaf)
            {
                for (int i = 0; i < unit.childUnits.Count; i++)
                {
                    statusDic[unit].waitUnits.Enqueue(unit.childUnits[i]);
                }
            }

            operateNode.onEndExecute = () =>
            {
                stateMechine.OnStopAction(unit.node as Graph.OperateNode);

                if (leaf || statusDic[unit].waitUnits.Count == 0)
                {
                    statusDic[unit].statu = ExecuteStatu.Completed;
                }

                stateMechine.Execute(unit);
            };

            operateNode.OnStartExecute(stateMechine.IsAuto);
            stateMechine.OnStartAction(unit.node as Graph.OperateNode);
        }

        /// <summary>
        /// 再次执行
        /// </summary>
        /// <param name="unit"></param>
        protected override void ExecuteOnExecuting(ExecuteUnit unit)
        {
            base.ExecuteOnExecuting(unit);
            if (HaveUnitNotComplete(unit))
            {
                return;
            }

            if (!LunchStackGroup(unit))
            {
                statusDic[unit].statu = ExecuteStatu.Completed;
                stateMechine.Execute(unit);
            }
        }

        /// <summary>
        /// 结束执行
        /// </summary>
        /// <param name="unit"></param>
        protected override void ExecuteOnCompleted(ExecuteUnit unit)
        {
            base.ExecuteOnCompleted(unit);
            if (unit.parentUnits.Count == 0)
            {
                Debug.LogError(unit.node + "have no parent!");
            }
            stateMechine.Execute(unit.parentUnits[0]);
        }


        public override void UnDo(ExecuteUnit unit)
        {
            base.UnDo(unit);
            UndoExecuteChildGroups(unit);
            if (statusDic[unit].statu != ExecuteStatu.UnStarted)
            {
                statusDic[unit].statu = ExecuteStatu.UnStarted;
                var operateNode = unit.node as OperateNode;
                operateNode.OnUnDoExecute();
                stateMechine.OnStopAction(unit.node as Graph.OperateNode);
            }
        }
        public override void Complete(ExecuteUnit unit)
        {
            base.Complete(unit);
            CompleteExecuteChildGroups(unit);
            statusDic[unit].statu = ExecuteStatu.Completed;
            var operateNode = unit.node as OperateNode;
            operateNode.onEndExecute = null;
            operateNode.OnEndExecute(true);
            stateMechine.OnStopAction(unit.node as Graph.OperateNode);
        }
    }
}