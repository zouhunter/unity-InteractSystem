using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Structure
{
    public abstract class ExecuteState
    {
        public ActionStateMechine stateMechine { get; set; }
        protected Dictionary<ExecuteUnit, UnitStatus> statusDic { get { return stateMechine.statuDic; } }
        public static bool log = false;

        public virtual void Execute(ExecuteUnit unit)
        {
            if (!statusDic.ContainsKey(unit))
            {
                statusDic[unit] = new UnitStatus();
            }

            switch (statusDic[unit].statu)
            {
                case ExecuteStatu.UnStarted:
                    ExecuteOnUnStarted(unit);
                    break;
                case ExecuteStatu.Executing:
                    ExecuteOnExecuting(unit);
                    break;
                case ExecuteStatu.Completed:
                    ExecuteOnCompleted(unit);
                    break;
                default:
                    break;
            }
        }

        public virtual void Complete(ExecuteUnit unit)
        {
            if (!statusDic.ContainsKey(unit))
            {
                statusDic[unit] = new UnitStatus();
            }
        }
        public virtual void UnDo(ExecuteUnit unit)
        {
            if (!statusDic.ContainsKey(unit))
            {
                statusDic[unit] = new UnitStatus();
            }
        }

        protected virtual void ExecuteOnCompleted(ExecuteUnit unit)
        {
            if (log) Debug.Log("ExecuteCompleted:" + unit.node.name);
        }

        protected virtual void ExecuteOnExecuting(ExecuteUnit unit)
        {
            if (log) Debug.Log("ExecuteExecuting:" + unit.node.name);
        }

        protected virtual void ExecuteOnUnStarted(ExecuteUnit unit)
        {
            stateMechine.activedUnits.Push(unit);
            if (log) Debug.Log("ExecuteUnStarted：" + unit.node.name);
        }

        /// <summary>
        /// 结束一组执行
        /// </summary>
        /// <param name="unit"></param>
        protected void CompleteExecuteChildGroups(ExecuteUnit unit)
        {
            var childs = unit.childUnits.ToArray();
            foreach (var list in childs)
            {
                foreach (var item in list)
                {
                    if (statusDic.ContainsKey(item))
                    {
                        if (statusDic[item].statu != ExecuteStatu.Completed)
                        {
                            stateMechine.Complete(item);
                        }
                    }
                    else
                    {
                        Debug.Log("ignore complete :" + unit.node.name);
                    }
                   
                }
            }
        }
        /// <summary>
        /// 重置执行
        /// </summary>
        /// <param name="unit"></param>
        protected void UndoExecuteChildGroups(ExecuteUnit unit)
        {
            var childUnits = unit.childUnits.ToArray();
            Array.Reverse(childUnits);
            foreach (var list in childUnits)
            {
                foreach (var item in list)
                {
                    if(statusDic.ContainsKey(item))
                    {
                        if (statusDic[item].statu != ExecuteStatu.UnStarted) {
                            stateMechine.UnDo(item);
                        }
                        stateMechine.UnDo(item);
                    }
                   else
                    {
                       if(log) Debug.Log("ignore:" + item.node);
                    }
                }
            }
        }
        /// <summary>
        /// 开启执行
        /// </summary>
        /// <param name="unit"></param>
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
                stateMechine.Execute(unit);
            }
        }

        /// <summary>
        /// 判读执行单元当前开启的列表是否执行完成
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        protected bool HaveUnitNotComplete(ExecuteUnit unit)
        {
            if (statusDic[unit].workUnits.Count > 0)
            {
                var lastworking = statusDic[unit].workUnits.Peek();
                if (!IsGroupCompleted(lastworking))
                {
                    Debug.Log(unit.node + "等待还没有执行完的同级任务");
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

        /// <summary>
        /// 判断列表中的单元是否都已经执行完成
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        protected bool IsGroupCompleted(List<ExecuteUnit> units)
        {
            //判断是不否所有已经结束
            var noCompleted = units.FindAll(x => statusDic[x].statu != ExecuteStatu.Completed);

            if (noCompleted.Count > 0)
            {
                //等待还没有执行完的同级任务
                return false;
            }
            return true;
        }
    }
}