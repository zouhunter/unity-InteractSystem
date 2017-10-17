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
        [SerializeField,Range(0, 100)]
        private int _queueID;
        public int QueueID { get { return _queueID; } }
     
        private Camera m_mainCamera;
        public string StepName { get { return _stepName; } }
        [SerializeField]
        private Camera m_viewCamera;//触发相机
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

        protected ActionCtroller coroutineCtrl;

        protected ActionObj[] actionObjs;
        [EnumMask, HideInInspector]
        public ControllerType commandType;

        #region 可选参数配制
     
        [HideInInspector]
        public float     lineWight = 0.1f;
        [HideInInspector]
        public Material lineMaterial;
        [HideInInspector]
        public float hitDistence = 10;
        [HideInInspector]
        public float pointDistence = 0.1f;
        #endregion

        [HideInInspector]
        public InputField.OnChangeEvent onBeforeActive;
        [HideInInspector]
        public InputField.OnChangeEvent onBeforeUnDo;
        [HideInInspector]
        public InputField.OnChangeEvent onBeforePlayEnd;
        public UnityEvent<string> Test;
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
            if (other.QueueID > QueueID)
            {
                return -1;
            }
            else if (other.QueueID == QueueID)
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
            if (coroutineCtrl == null)
                coroutineCtrl = new ActionCtroller(this);

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
                
                coroutineCtrl.OnStartExecute(forceAuto);
              
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
        }

     
    }
}

