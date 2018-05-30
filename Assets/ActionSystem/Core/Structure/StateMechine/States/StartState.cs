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
        //第一次执行
        protected override void ExecuteUnStarted(ExecuteUnit unit)
        {
            base.ExecuteUnStarted(unit);
            statusDic[unit].statu = ExecuteStatu.Executing;
            StartExecuteChildGroups(unit);
        }

      
        //再次执行
        protected override void ExecuteExecuting(ExecuteUnit unit)
        {
            if (HaveUnitNotComplete(unit))
            {
                return;
            }

            if (!LunchStackGroup(unit))
            {
                Debug.LogError("结束执行");
            }
        }
      
    }
}