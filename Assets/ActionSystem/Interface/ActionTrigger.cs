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
        [Range(0,100)]
        public int executeIndex;
        [Range(1,100)]
        public int repeat;
        public abstract IList<IActionCommand> CreateCommands();
        public Func<ElementGroup> ElementGroup { get; set; }
        private StepComplete onStepComplete;
        private UserError onUserErr;
        protected ActionObj[] actionObjs;

        protected virtual void Awake()
        {
            actionObjs = GetComponentsInChildren<ActionObj>(true);
            foreach (var item in actionObjs){
                item.StepName = _stepName;
            }
        }


        public void InitTrigger(StepComplete onStepComplete, UserError onUserErr/*, Func<ElementGroup> ElementGroup*/)
        {
            this.onStepComplete = onStepComplete;
            this.onUserErr = onUserErr;
            //this.ElementGroup = ElementGroup;
        }

        private int currentRepeat;

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