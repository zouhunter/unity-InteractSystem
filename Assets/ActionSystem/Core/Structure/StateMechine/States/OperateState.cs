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
        internal override void Execute(ExecuteUnit unit)
        {
            Debug.Log("OperateState");
            var operateNode = unit.node as OperateNode;
            if (!statusDic.ContainsKey(unit))
            {
                statusDic[unit] = new UnitStatus();

                if (unit.childUnits.Count == 0)
                {
                    TryActiveNextUnits(unit);
                }
                else
                {
                    for (int i = 0; i < unit.childUnits.Count; i++)
                    {
                        statusDic[unit].waitUnits.Enqueue(unit.childUnits[i]);
                    }
                }
            }
            else if (statusDic[unit].waitUnits.Count > 0)
            {
                Debug.Log("execute wait units!");
                var units = statusDic[unit].waitUnits.Dequeue();
                stateMechine.ExecuteGroup(units);
            }

            if (!operateNode.Started)
            {
                operateNode.onEndExecute = () => Execute(unit);
                operateNode.OnStartExecute(stateMechine.IsAuto);
            }
        }

        private void OnEndExecuteOperate()
        {
            
        }

        /// <summary>
        /// 试图开启下一阶段的节点
        /// </summary>
        private void TryActiveNextUnits(ExecuteUnit unit)
        {
            statusDic[unit].waitUnits.Enqueue(new List<ExecuteUnit>() { unit.parentUnits[0] });
            //stateMechine.Execute(unit.parentUnits[0]);
        }
    }
}