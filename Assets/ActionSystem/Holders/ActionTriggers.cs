using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ActionTriggers : MonoBehaviour
    {
        private List<ActionTrigger> objectList = new List<ActionTrigger>();
        private Dictionary<string, List<ActionTrigger>> objectDic = new Dictionary<string, List<ActionTrigger>>();
        public UnityAction<Dictionary<string,List<ActionTrigger>>> onAllElementInit;
        public bool AllRegisted { get; private set; }
     
        private void Start()
        {
            objectList.AddRange(GetComponentsInChildren<ActionTrigger>());

            foreach (var trigger in objectList)
            {
                var obj = trigger;
                if (objectDic.ContainsKey(obj.StepName))
                {
                    objectDic[obj.StepName].Add(obj);
                }
                else
                {
                    objectDic[obj.StepName] = new List<ActionTrigger>() { obj };
                }
            }
            onAllElementInit.Invoke(objectDic);
        }
    }
}