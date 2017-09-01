using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public abstract class ActionTrigger:MonoBehaviour
    {
        [SerializeField]
        private string _stepName;
        public string StepName { get { return _stepName; } }
        public Func<List<InstallItem>> InstallItems;
        public Func<ActionResponce> Responce;
        public abstract IActionCommand CreateCommand();
    }

}