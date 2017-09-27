using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public abstract class ActionCommand : MonoBehaviour,IActionCommand, IComparable<ActionCommand>
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
        protected IActionCtroller coroutineCtrl;
        protected Coroutine coroutine;
        protected abstract IActionCtroller CreateCtrl();
        protected ActionObj[] actionObjs;
        [SerializeField]
        protected StepEvent onBeforeActive;
        [SerializeField]
        protected StepEvent onBeforeUnDo;
        [SerializeField]
        protected StepEvent onBeforePlayEnd;
        public int Count { get; private set; }
        protected virtual void Awake()
        {
            actionObjs = GetComponentsInChildren<ActionObj>(true);
        }
        public void RegistComplete(StepComplete stepComplete)
        {
            this.stepComplete = stepComplete;
        }
        public void RegistAsOperate(UserError userErr, Func<ElementController> elementCtrlGet)
        {
            this.userErr = userErr;
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
            Debug.Log(StepName);
            stepComplete.Invoke(StepName);
        }

        public virtual void StartExecute(bool forceAuto)
        {
            onBeforeActive.Invoke(StepName);
            if (coroutineCtrl == null)
                coroutineCtrl = CreateCtrl();

            coroutineCtrl.StartExecute(forceAuto);
            if (coroutine == null)
            {
                coroutine = StartCoroutine(coroutineCtrl.Update());
            }
        }
        public virtual void EndExecute()
        {
            onBeforePlayEnd.Invoke(StepName);
            if (coroutineCtrl == null) return;
            coroutineCtrl.EndExecute();
            if (coroutine != null)
            {
                StopCoroutine(coroutineCtrl.Update());
            }
        }
        public virtual void UnDoExecute()
        {
            onBeforeUnDo.Invoke(StepName);
            if (coroutineCtrl == null) return;
            coroutineCtrl.UnDoExecute();
            if (coroutine != null)
            {
                StopCoroutine(coroutineCtrl.Update());
            }
        }
    }

}