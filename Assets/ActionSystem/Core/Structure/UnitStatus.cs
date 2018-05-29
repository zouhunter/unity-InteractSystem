using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem.Structure
{
    public class UnitStatus
    {
        public Queue<List<ExecuteUnit>> waitUnits = new Queue<List<ExecuteUnit>>();
    }
}