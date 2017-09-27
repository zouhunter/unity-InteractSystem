using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class ActionCommand : MonoBehaviour,IActionCommand, IComparable<ActionCommand>
    {
        [SerializeField]
        private string _stepName;
        [Range(0, 100)]
        public int executeIndex;
        public string StepName { get { return _stepName; } }
        private UserError userErr { get; set; }
        private StepComplete stepComplete { get; set; }
        private Func<ElementController> elementCtrlGet { get; set; }
        private ElementController elementCtrl;
        public ActionObj[] ActionObjs { get { return actionObjs; } }
        public ActionSystem actionSystem { get; set; }

        protected ActionObj[] actionObjs;
        [SerializeField]
        protected StepEvent onBeforeActive;
        [SerializeField]
        protected StepEvent onBeforeUnDo;
        [SerializeField]
        protected StepEvent onBeforePlayEnd;
        protected virtual void Awake()
        {
            actionObjs = GetComponentsInChildren<ActionObj>(true);
        }

        public void RegistAsOperate(UserError userErr, StepComplete stepComplete, Func<ElementController> elementCtrlGet)
        {
            this.userErr = userErr;
            this.stepComplete = stepComplete;
            this.elementCtrlGet = elementCtrlGet;
                
        }
        public int CompareTo(ActionCommand other)
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

        internal ElementController ElementCtrl
        {
            get
            {
                if(elementCtrl == null)
                {
                    elementCtrl = elementCtrlGet();
                }
                return elementCtrl;
            }
        }

        internal void UserError(string err)
        {
            userErr.Invoke(StepName,err);
        }

        internal void Complete()
        {
            stepComplete.Invoke(StepName);
        }

        public virtual void StartExecute(bool forceAuto)
        {
            onBeforeActive.Invoke(StepName);
            foreach (var item in ActionObjs)
            {
                item.OnStartExecute();
            }
        }

        public virtual void EndExecute()
        {
            onBeforePlayEnd.Invoke(StepName);
            foreach (var item in ActionObjs)
            {
                item.OnEndExecute();
            }
        }

        public virtual void UnDoExecute()
        {
            onBeforeUnDo.Invoke(StepName);
            foreach (var item in ActionObjs)
            {
                item.OnUnDoExecute();
            }
        }
    }

}