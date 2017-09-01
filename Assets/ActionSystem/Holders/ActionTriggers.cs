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
        public UnityAction<List<ActionTrigger>> onAllElementInit;
        private List<ActionTrigger> objectList = new List<ActionTrigger>();
        public bool AllRegisted { get; private set; }
     
        private void Start()
        {
            objectList.AddRange(GetComponentsInChildren<ActionTrigger>());
            onAllElementInit.Invoke(objectList);
        }
    }
}