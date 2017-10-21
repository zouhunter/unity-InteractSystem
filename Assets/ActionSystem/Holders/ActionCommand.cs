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
        [SerializeField,Range(0, 10)]
        private int _queueID;
        public int QueueID { get { return _queueID; } }
        [SerializeField]
        private string _cameraID;
        public string CameraID { get { return _cameraID; } }
        public string StepName { get { return _stepName; } }
        private UserError userErr { get; set; }
        private StepComplete stepComplete { get; set; }
        private CameraController cameraCtrl { get; set; }
        private ElementController elementCtrl { get; set; }
        public IActionObj[] ActionObjs { get { return actionObjs; } }
        public ActionSystem actionSystem { get; set; }

        protected ActionCtroller coroutineCtrl;

        protected IActionObj[] actionObjs;
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
        protected virtual void Awake()
        {
            RegistActionObjs();
            WorpCameraID();
        }
        private void RegistActionObjs()
        {
            ///只操作第一层活动的对象
            var first = new List<IActionObj>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var item = transform.GetChild(i);
                if (item.gameObject.activeSelf)
                {
                    var actions = item.GetComponents<IActionObj>();
                    if (actions != null)
                    {
                        first.AddRange(actions);
                    }
                }
            }

            actionObjs = first.ToArray();
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

        public void RegistComplete(StepComplete stepComplete)
        {
            this.stepComplete = stepComplete;
        }

        public void RegistAsOperate(UserError userErr)
        {
            this.userErr = userErr;
            this.elementCtrl = elementCtrl;
            this.cameraCtrl = cameraCtrl;
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
                return elementCtrl;
            }
        }
        internal CameraController CameraCtrl
        {
            get { return cameraCtrl; }
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
                started = true;
                onBeforeActive.Invoke(StepName);

                CameraController.SetViewCamera(() => {
                    coroutineCtrl.OnStartExecute(forceAuto);
                }, _cameraID);

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
            onBeforePlayEnd.Invoke(StepName);
            if (coroutineCtrl != null) coroutineCtrl.OnEndExecute();
        }

        public virtual void UnDoExecute()
        {
            started = false;
            completed = false;
            onBeforeUnDo.Invoke(StepName);
            if (coroutineCtrl != null) coroutineCtrl.OnUnDoExecute();
        }

     
    }
}

