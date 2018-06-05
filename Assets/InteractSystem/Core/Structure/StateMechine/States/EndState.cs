using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Structure
{
    public class EndState : ExecuteState
    {
        protected override void ExecuteOnUnStarted(ExecuteUnit unit)
        {
            base.ExecuteOnUnStarted(unit);
            statusDic[unit].statu = ExecuteStatu.Completed;
            stateMechine.Complete();
        }

        protected override void ExecuteOnCompleted(ExecuteUnit unit)
        {
            base.ExecuteOnCompleted(unit);
            Debug.LogWarning("已经结束");
        }
    }
}