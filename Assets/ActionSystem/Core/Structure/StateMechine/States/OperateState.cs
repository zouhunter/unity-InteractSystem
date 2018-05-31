using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using OperateNode = WorldActionSystem.Graph.OperateNode;

namespace WorldActionSystem.Structure
{
    public class OperateState : ExecuteState
    {
        /// <summary>
        /// 首次执行
        /// </summary>
        /// <param name="unit"></param>
        protected override void ExecuteUnStarted(ExecuteUnit unit)
        {
            base.ExecuteUnStarted(unit);
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

                Execute(unit);
            };

            operateNode.OnStartExecute(stateMechine.IsAuto);
            stateMechine.OnStartAction(unit.node as Graph.OperateNode);
        }

        /// <summary>
        /// 再次执行
        /// </summary>
        /// <param name="unit"></param>
        protected override void ExecuteExecuting(ExecuteUnit unit)
        {
            base.ExecuteExecuting(unit);
            if(HaveUnitNotComplete(unit)){
                return;
            }

            if (!LunchStackGroup(unit))
            {
                statusDic[unit].statu = ExecuteStatu.Completed;
                Execute(unit);
            }
        }

        /// <summary>
        /// 结束执行
        /// </summary>
        /// <param name="unit"></param>
        protected override void ExecuteCompleted(ExecuteUnit unit)
        {
            base.ExecuteCompleted(unit);
            if (unit.parentUnits.Count == 0)
            {
                Debug.LogError(unit.node + "have no parent!");
            }
            stateMechine.Execute(unit.parentUnits[0]);
        }



    }
}