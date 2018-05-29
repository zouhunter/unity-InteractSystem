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
        internal override void Execute(ExecuteUnit unit)
        {
            Debug.Log("StartState");
            //第一次执行
            if (!statusDic.ContainsKey(unit))
            {
                statusDic[unit] = new UnitStatus();
                for (int i = 1; i < unit.childUnits.Count; i++){
                    statusDic[unit].waitUnits.Enqueue(unit.childUnits[i]);
                }
                stateMechine.ExecuteGroup(unit.childUnits[0]);
            }
            //再次执行
            else if(statusDic[unit].waitUnits.Count > 0)
            {
                var units = statusDic[unit].waitUnits.Dequeue();
                stateMechine.ExecuteGroup(units);
            }
        }

    }
}