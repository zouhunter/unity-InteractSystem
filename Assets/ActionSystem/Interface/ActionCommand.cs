using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public abstract class ActionCommand : MonoBehaviour, IActionCommand, IComparable<ActionCommand>
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
        private bool started;
        private bool completed;
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
                if (elementCtrl == null)
                {
                    elementCtrl = elementCtrlGet();
                }
                return elementCtrl;
            }
        }

        internal void UserError(string err)
        {
            userErr.Invoke(StepName, err);
        }

        /// <summary>
        /// 操作过程自动结束
        /// </summary>
        internal bool Complete()
        {
            if(!completed)
            {
                completed = true;
                stepComplete.Invoke(StepName);
                return true;
            }
            else
            {
                Debug.Log("already completed" + name);
                return false;
            }
        }

        public virtual bool StartExecute(bool forceAuto)
        {
            if(!started)
            {
                started = true;
                onBeforeActive.Invoke(StepName);
                if (coroutineCtrl == null)
                    coroutineCtrl = CreateCtrl();

                coroutineCtrl.OnStartExecute(forceAuto);
                if (coroutine == null)
                {
                    coroutine = StartCoroutine(coroutineCtrl.Update());
                }
                return true;
            }
            else
            {
                Debug.Log("already started" + name);
                return false;
            }
        }
        /// <summary>
        /// 强制结束
        /// </summary>
        public virtual bool EndExecute()
        {
            Debug.Log("EndExecute", gameObject);

            if (!completed)
            {
                completed = true;
                OnEndExecute();
                return true;
            }
            else
            {
                Debug.Log("already completed" + name);
                return false;
            }

        }

        public void OnEndExecute()
        {
            onBeforePlayEnd.Invoke(StepName);
            if (coroutineCtrl == null) return;
            coroutineCtrl.OnEndExecute();
            if (coroutine != null)
            {
                StopCoroutine(coroutineCtrl.Update());
            }
        }

        public virtual void UnDoExecute()
        {
            started = false;
            completed = false;
            onBeforeUnDo.Invoke(StepName);
            if (coroutineCtrl == null) return;
            coroutineCtrl.OnUnDoExecute();
            if (coroutine != null)
            {
                StopCoroutine(coroutineCtrl.Update());
            }
        }
    }
}

