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
        public string _name;
        public bool startActive;
        public bool endActive;
        protected bool _complete;
        public string Name { get { return _name; } }
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
        protected AngleCtroller angleCtrl { get { return AngleCtroller.Instance; } }
        public static bool log = true;
        protected bool notice;
        protected virtual void Start()
        {
            if (string.IsNullOrEmpty(_name)) _name = name;
            hooks = GetComponentsInChildren<ActionHook>(false);
            if (hooks.Length > 0)
            {
                hookCtrl = new HookCtroller(this);
            }
            gameObject.SetActive(startActive);
            if (anglePos == null)
            {
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

            if (!Setting.angleNotice || this is AnimObj) return;

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
                onBeforeStart.Invoke(auto);
                _started = true;
                _complete = false;
                notice = true;
                gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("already started", gameObject);
            }
        }

        public virtual void OnEndExecute(bool force)
        {
            if (angleCtrl) angleCtrl.UnNotice(anglePos);
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
            if (log) Debug.Log("OnEndExecute:" + this + ":" + force, this);

            if (!_complete)
            {
                onBeforeComplete.Invoke(force);
                notice = false;
                _started = true;
                _complete = true;
                gameObject.SetActive(endActive);
                if (onEndExecute != null)
                {
                    onEndExecute.Invoke();
                }
                if (hooks.Length > 0)
                {
                    hookCtrl.OnEndExecute();
                }
            }
            else
            {
                Debug.LogError("already completed", gameObject);
            }
        }

        public virtual void OnUnDoExecute()
        {
            if(_started) 
            {
                _started = false;
                _complete = false;
                notice = false;
                onBeforeUnDo.Invoke();

                gameObject.SetActive(startActive);
                if (angleCtrl) angleCtrl.UnNotice(anglePos);

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

    }
}