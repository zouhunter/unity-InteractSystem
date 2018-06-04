using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem.Structure
{
    public class StartState : ExecuteState
    {
        /// <summary>
        /// 结束
        /// </summary>
        /// <param name="unit"></param>
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
        //第一次执行
        protected override void ExecuteOnUnStarted(ExecuteUnit unit)
        {
            base.ExecuteOnUnStarted(unit);
            statusDic[unit].statu = ExecuteStatu.Executing;
            StartExecuteChildGroups(unit);
        }

        //再次执行
        protected override void ExecuteOnExecuting(ExecuteUnit unit)
        {
            if (HaveUnitNotComplete(unit))
            {
                return;
            }

            if (!LunchStackGroup(unit))
            {
                statusDic[unit].statu = ExecuteStatu.Completed;
                stateMechine.Complete();
            }
        }
      
    }
}