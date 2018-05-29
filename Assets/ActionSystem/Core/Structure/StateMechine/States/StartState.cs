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
            Debug.Assert(unit.childUnits.Count > 0);
            //打开下一级的步骤
            for (int i = unit.childUnits.Count - 1; i >= 0; i--)
            {
                //if (waitUnits.ContainsKey(unit))
                //{
                //    waitUnits[unit].Push(unit.childUnits[i]);
                //}
            }
            //ExecuteAnGroup(unit);
        }
    }
}