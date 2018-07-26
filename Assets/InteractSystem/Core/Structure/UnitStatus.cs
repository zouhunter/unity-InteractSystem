using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Structure
{

    public class UnitStatus
    {
        public ExecuteStatu statu;//执行状态
        public Stack<List<ExecuteUnit>> workUnits = new Stack<List<ExecuteUnit>>();//执行中
        public Queue<List<ExecuteUnit>> waitUnits = new Queue<List<ExecuteUnit>>();//执行等待

        internal void Clear()
        {
            waitUnits.Clear();
            workUnits.Clear();
        }
    }
}