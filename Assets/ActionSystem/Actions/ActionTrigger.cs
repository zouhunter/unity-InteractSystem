using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public abstract class ActionTrigger:MonoBehaviour,IComparable<ActionTrigger>
    {
        [SerializeField]
        private string _stepName;
        public string StepName { get { return _stepName; } }
        public int executeIndex;
        public bool Complete { get { return _complete; } }
        private bool _complete;
        protected ActionObj[] actionObjs;
        public StepComplete onStepComplete;
        public UserError onUserErr;
        public Func<ElementGroup> ElementGroup;
        public abstract IActionCommand CreateCommand();
        protected virtual void Awake()
        {
            actionObjs = GetComponentsInChildren<ActionObj>(true);
            foreach (var item in actionObjs){
                item.StepName = _stepName;
            }
        }
        public int CompareTo(ActionTrigger other)
        {
            if (other.executeIndex > executeIndex)
            {
                return -1;
            }
            else if (other.executeIndex == executeIndex)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }

}