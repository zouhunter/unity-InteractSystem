using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public abstract class ActionTrigger:MonoBehaviour,IComparable<ActionTrigger>, IActionEvents
    {
        [SerializeField]
        private string _stepName;
        public string StepName { get { return _stepName; } }
        [Range(0,100)]
        public int executeIndex;
        [Range(1,100)]
        public int repeat =1;
        public abstract IList<IActionCommand> CreateCommands();
        public Func<ElementController> ElementController { get; set; }
        public StepComplete onStepComplete { get; set; }
        public UserError onUserErr { get; set; }
        protected ActionObj[] actionObjs;

        protected virtual void Awake()
        {
            actionObjs = GetComponentsInChildren<ActionObj>(true);
            foreach (var item in actionObjs){
                item.StepName = _stepName;
            }
        }
        protected void OnComplete()
        {
            if (onStepComplete != null) onStepComplete.Invoke(StepName);
        }

        protected void OnUserError(string errorInfo)
        {
            if (onUserErr != null) onUserErr(StepName, errorInfo);
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