using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;
namespace WorldActionSystem
{
    public abstract class ActionObj : MonoBehaviour, IActionObj
    {
        [SerializeField]
        protected string m_name;
        public bool startActive;
        public bool endActive;
        protected bool _complete;
        public string Name { get { return m_name; } }
        public bool Complete { get { return _complete; } }
        protected bool _started;
        public bool Started { get { return _started; } }
        protected bool auto;
        [SerializeField, Range(0, 10)]
        private int queueID;
        public int QueueID
        {
            get
            {
                return queueID;
            }
        }
        [SerializeField]
        private bool _queueInAuto = true;
        public bool QueueInAuto { get { return _queueInAuto; } }
        [SerializeField]
        private string _cameraID;
        public string CameraID { get { return _cameraID; } }
        public Transform anglePos;
        public UnityAction onEndExecute { get; set; }
        public Toggle.ToggleEvent onBeforeStart;
        public Toggle.ToggleEvent onBeforeComplete;
        public UnityEvent onBeforeUnDo;
        private ActionHook[] hooks;//外部结束钩子
        public ActionHook[] Hooks { get { return hooks; } }
        private HookCtroller hookCtrl;
        protected AngleCtroller angleCtrl { get { return system.AngleCtrl; } }
        private ActionGroup _system;
        public ActionGroup system { get { transform.SurchSystem(ref _system); return _system; } }
        protected ElementController elementCtrl { get { return ElementController.Instence; } }
       
        public abstract ControllerType CtrlType { get; }
        public static bool log = true;
        protected bool notice;
        protected virtual void Start()
        {
            if (string.IsNullOrEmpty(m_name)) m_name = name;
            hooks = GetComponentsInChildren<ActionHook>(false);
            if (hooks.Length > 0){
                hookCtrl = new HookCtroller(this);
            }
            gameObject.SetActive(startActive);
            if (anglePos == null){
                anglePos = transform;
            }
            WorpCameraID();
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
        protected virtual void Update()
        {
            if (Complete||!Started) return;

            if (!Config.angleNotice || this is AnimObj) return;

            if (notice)
            {
                if (angleCtrl) angleCtrl.Notice(anglePos);
            }
            else
            {
                if (angleCtrl) angleCtrl.UnNotice(anglePos);
            }

        }

        public virtual void OnStartExecute(bool auto = false)
        {
            if (log) Debug.Log("OnStartExecute:" + this);
            this.auto = auto;
            if (!_started)
            {
                _started = true;
                _complete = false;
                notice = true;
                onBeforeStart.Invoke(auto);
                gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("already started", gameObject);
            }
        }

        public virtual void OnEndExecute(bool force)
        {
            notice = false;

            if (force)
            {
                if (!Complete) CoreEndExecute(true);
            }
            else
            {
                if (hooks.Length > 0)
                {
                    if (hookCtrl.Complete)
                    {
                        if (!Complete) CoreEndExecute(false);
                    }
                    else if (!hookCtrl.Started)
                    {
                        hookCtrl.OnStartExecute(auto);
                    }
                    else
                    {
                        Debug.Log("wait:" + Name);
                    }
                }
                else
                {
                    if (!Complete) CoreEndExecute(false);
                }
            }
        }

        private void CoreEndExecute(bool force)
        {
            if (angleCtrl) angleCtrl.UnNotice(anglePos);

            if (log) Debug.Log("OnEndExecute:" + this + ":" + force, this);

            if (!_complete)
            {
                notice = false;
                _started = true;
                _complete = true;
                onBeforeComplete.Invoke(force);
                if (hooks.Length > 0){
                    hookCtrl.OnEndExecute();
                }
                if (onEndExecute != null)
                {
                    onEndExecute.Invoke();
                }
                gameObject.SetActive(endActive);
            }
            else
            {
                Debug.LogError("already completed", gameObject);
            }
        }

        public virtual void OnUnDoExecute()
        {
            if (angleCtrl)
                angleCtrl.UnNotice(anglePos);

            if (log) Debug.Log("OnUnDoExecute:" + this, this);

            if (_started) 
            {
                _started = false;
                _complete = false;
                notice = false;
                onBeforeUnDo.Invoke();
                gameObject.SetActive(startActive);

                if (hooks.Length > 0)
                {
                    hookCtrl.OnUnDoExecute();
                }
            }
            else
            {
                Debug.LogError(this + "allready undo");
            }

        }

        public int CompareTo(IActionObj other)
        {
            if(QueueID > other.QueueID)
            {
                return 1;
            }
            else if(QueueID < other.QueueID)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}