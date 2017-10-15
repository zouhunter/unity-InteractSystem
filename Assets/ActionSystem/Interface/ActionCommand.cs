using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ActionCommand : MonoBehaviour, IActionCommand, IComparable<ActionCommand>
    {
        [SerializeField]
        private string _stepName;
        [Range(0, 100)]
        public int executeIndex;
        [SerializeField]
        private Camera m_viewCamera;//触发相机
        private Camera m_mainCamera;
        public string StepName { get { return _stepName; } }
        public Camera viewCamera
        {
            get
            {
                if (m_viewCamera != null) return m_viewCamera;
                else
                {
                    return mainCamera;
                }
            }
        }
        private Camera mainCamera
        {
            get
            {
                if (m_mainCamera == null)
                {
                    m_mainCamera = Camera.main;
                }
                return m_mainCamera;
            }
        }
        private UserError userErr { get; set; }
        private StepComplete stepComplete { get; set; }
        private Func<ElementController> elementCtrlGet { get; set; }
        private ElementController elementCtrl;
        public ActionObj[] ActionObjs { get { return actionObjs; } }

        public ActionSystem actionSystem { get; set; }

        protected IActionCtroller coroutineCtrl;
        protected Coroutine coroutine;
        protected virtual IActionCtroller CreateCtrl()
        {
            return new ActionCtroller(this);
        }
        protected ActionObj[] actionObjs;
        [SerializeField]
        protected StepEvent onBeforeActive;
        [SerializeField]
        protected StepEvent onBeforeUnDo;
        [SerializeField]
        protected StepEvent onBeforePlayEnd;

        private bool started;
        private bool completed;
        private bool cameraStartVisiable;
        private bool mainCamraExecuteVisiable;

        protected virtual void Awake()
        {
            actionObjs = GetComponentsInChildren<ActionObj>(false);
            if (m_viewCamera != null) cameraStartVisiable = m_viewCamera.gameObject.activeSelf;
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
            if (!completed)
            {
                started = true;
                completed = true;
                OnEndExecute();
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
            if (!started)
            {
                if (m_viewCamera != null)
                {
                    m_viewCamera.gameObject.SetActive(true);
                    if (mainCamera != null && m_viewCamera != mainCamera)
                    {
                        mainCamraExecuteVisiable = mainCamera.gameObject.activeSelf;
                        mainCamera.gameObject.SetActive(false);
                    }
                }

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
                started = true;
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
            if (m_viewCamera != null)
            {
                m_viewCamera.gameObject.SetActive(cameraStartVisiable);
                mainCamera.gameObject.SetActive(mainCamraExecuteVisiable);
            }
            onBeforePlayEnd.Invoke(StepName);
            if (coroutineCtrl != null) coroutineCtrl.OnEndExecute();
            StopUpdateAction();
        }

        public virtual void UnDoExecute()
        {
            if (m_viewCamera != null)
            {
                m_viewCamera.gameObject.SetActive(cameraStartVisiable);
                mainCamera.gameObject.SetActive(mainCamraExecuteVisiable);
            }

            started = false;
            completed = false;
            onBeforeUnDo.Invoke(StepName);
            if (coroutineCtrl != null) coroutineCtrl.OnUnDoExecute();
            StopUpdateAction();
        }

        private void StopUpdateAction()
        {

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}

