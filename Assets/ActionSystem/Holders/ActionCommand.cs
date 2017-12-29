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
        [SerializeField, Range(0, 10)]
        private int _queueID;
        public int QueueID { get { return _queueID; } }
        [SerializeField]
        private string _cameraID = CameraController.defultID;
        public string CameraID { get { return _cameraID; } }
        public string StepName { get { return _stepName; } }
        public bool Startd { get { return started; } }
        public bool Completed { get { return completed; } }
        private UserError userErr { get; set; }
        private StepComplete stepComplete { get; set; }//步骤自动结束方法
        public IActionObj[] ActionObjs { get { return actionObjs; } }
        public ActionSystem actionSystem { get; set; }
        protected ActionCtroller coroutineCtrl;
        public ActionCtroller ActionCtrl { get { return coroutineCtrl; } }
        protected IActionObj[] actionObjs;
        [EnumMask, HideInInspector]
        public ControllerType commandType;//用于editor

        #region 可选参数配制

        [HideInInspector]
        public float lineWight = 0.1f;
        [HideInInspector]
        public Material lineMaterial;
        #endregion

        [HideInInspector]
        public InputField.OnChangeEvent onBeforeActive;
        [HideInInspector]
        public InputField.OnChangeEvent onBeforeUnDo;
        [HideInInspector]
        public InputField.OnChangeEvent onBeforePlayEnd;

        private bool started;
        private bool completed;

        private CommandController _commandCtrl;
        protected CommandController commandCtrl { get { transform.RetriveCommandCtrl(ref _commandCtrl); return _commandCtrl; } }
        protected virtual void Awake()
        {
            RegistActionObjs();
            WorpCameraID();
        }
        protected virtual void Start()
        {
            if (coroutineCtrl == null)
                coroutineCtrl = new ActionCtroller(this);
            RegistToActionSystem();
        }
        private void RegistToActionSystem()
        {
            commandCtrl.RegistCommand(this);
        }

        private void WorpCameraID()
        {
            if (string.IsNullOrEmpty(_cameraID))
            {
                var node = GetComponentInChildren<CameraNode>();
                if (node != null)
                {
                    _cameraID = node.name;
                }
            }
        }
        private void RegistActionObjs()
        {
            actionObjs = GetComponentsInChildren<IActionObj>(false);
        }

        public void RegistAsOperate(UserError userErr)
        {
            this.userErr = userErr;
        }
        public void RegistComplete(StepComplete stepComplete)
        {
            this.stepComplete = stepComplete;
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
                if (stepComplete != null) stepComplete.Invoke(StepName);
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
                started = true;
                onBeforeActive.Invoke(StepName);
                if(coroutineCtrl!=null)
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
            //Debug.Log("EndExecute", gameObject);

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
            onBeforePlayEnd.Invoke(StepName);
            if (coroutineCtrl != null) coroutineCtrl.OnEndExecute();
        }

        public virtual void UnDoExecute()
        {
            started = false;
            completed = false;
            onBeforeUnDo.Invoke(StepName);
            if (coroutineCtrl != null)
                coroutineCtrl.OnUnDoExecute();
        }


    }
}

