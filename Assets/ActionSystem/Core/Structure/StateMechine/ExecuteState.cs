using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem.Structure
{
    public abstract class ExecuteState
    {
        public ActionStateMechine stateMechine { get; set; }
        protected Dictionary<ExecuteUnit, UnitStatus> statusDic { get { return stateMechine.statuDic; } }
        public static bool log = true;

        internal virtual void Execute(ExecuteUnit unit)
        {
            if (!statusDic.ContainsKey(unit))
            {
                statusDic[unit] = new UnitStatus();
            }

            switch (statusDic[unit].statu)
            {
                case ExecuteStatu.UnStarted:
                    ExecuteUnStarted(unit);
                    break;
                case ExecuteStatu.Executing:
                    ExecuteExecuting(unit);
                    break;
                case ExecuteStatu.Completed:
                    ExecuteCompleted(unit);
                    break;
                default:
                    break;
            }
        }

        protected virtual void ExecuteCompleted(ExecuteUnit unit)
        {
            if (log) Debug.Log("ExecuteCompleted:" + unit.node.name);
        }

        protected virtual void ExecuteExecuting(ExecuteUnit unit)
        {
            if (log) Debug.Log("ExecuteExecuting:" + unit.node.name);
        }

        protected virtual void ExecuteUnStarted(ExecuteUnit unit)
        {
            if (log) Debug.Log("ExecuteUnStarted：" + unit.node.name);
        }
        protected void StartExecuteChildGroups(ExecuteUnit unit)
        {
            for (int i = 1; i < unit.childUnits.Count; i++)
            {
                statusDic[unit].waitUnits.Enqueue(unit.childUnits[i]);
            }

            if (unit.childUnits.Count > 0)
            {
                var firstGroup = unit.childUnits[0];
                stateMechine.ExecuteGroup(firstGroup);
                statusDic[unit].workUnits.Push(firstGroup);
            }
            else
            {
                //结束执行
                Debug.LogError("结束执行");
                statusDic[unit].statu = ExecuteStatu.Completed;
                Execute(unit);
            }
        }

        protected bool HaveUnitNotComplete(ExecuteUnit unit)
        {
            if (statusDic[unit].workUnits.Count > 0)
            {
                var lastworking = statusDic[unit].workUnits.Peek();
                if (!IsGroupCompleted(lastworking))
                {
                    Debug.Log("等待还没有执行完的同级任务");
                    return true;
                }
            }
            return false;
        }

        protected bool LunchStackGroup(ExecuteUnit unit)
        {
            if (statusDic[unit].waitUnits.Count > 0)
            {
                var units = statusDic[unit].waitUnits.Dequeue();
                statusDic[unit].workUnits.Push(units);
                stateMechine.ExecuteGroup(units);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool IsGroupCompleted(List<ExecuteUnit> units)
        {
            //判断是不否所有已经结束
            var noCompleted = units.FindAll(x => statusDic[x].statu != ExecuteStatu.Completed);

            if (noCompleted.Count > 0)
            {
                //等待还没有执行完的同级任务
                Debug.Log("等待还没有执行完的同级任务");
                return false;
            }
            return true;
        }
    }
}