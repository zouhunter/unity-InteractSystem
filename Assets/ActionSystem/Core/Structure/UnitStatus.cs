using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem.Structure
{
    
    public class UnitStatus
    {
        public ExecuteStatu statu;
        public Stack<List<ExecuteUnit>> workUnits = new Stack<List<ExecuteUnit>>();
        public Queue<List<ExecuteUnit>> waitUnits = new Queue<List<ExecuteUnit>>();
    }
}