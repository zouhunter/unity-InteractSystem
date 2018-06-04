using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem.Structure
{
    public class EndState : ExecuteState
    {
        protected override void ExecuteOnUnStarted(ExecuteUnit unit)
        {
            base.ExecuteOnUnStarted(unit);
            statusDic[unit].statu = ExecuteStatu.Completed;
            Debug.LogError("EndNode: 结束执行");
        }

        protected override void ExecuteOnCompleted(ExecuteUnit unit)
        {
            base.ExecuteOnCompleted(unit);
            Debug.LogError("已经结束");
        }
    }
}