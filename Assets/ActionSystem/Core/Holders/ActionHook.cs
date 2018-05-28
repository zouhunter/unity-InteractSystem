using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    public class ActionHook : ActionSystemObject
    {
        public bool Complete { get { return _complete; } }
        public bool Started { get { return _started; } }
        [SerializeField, Range(0, 10)]
        private int queueID;
        public int QueueID
        {
            get
            {
                return queueID;
            }
        }
        public string CameraID { get { return null; } }
        protected virtual bool autoComplete { get { return false; } }
        protected float autoTime = 2;
        public UnityAction onEndExecute { get; set; }
        public Toggle.ToggleEvent onBeforeEndExecuted;

        public static bool log = false;
        protected bool _complete;
        protected bool _started;
        protected Graph.OperateNode operater;

        public void SetContext(Graph.OperateNode operater)
        {
            this.operater = operater;
        }
        public virtual void OnStartExecute(bool auto)
        {
            if(log) Debug.Log("onStart Execute Hook :" + this);
            if (!_started)
            {
                _started = true;
                _complete = false;
                coroutineCtrl.DelyExecute(AutoComplete, autoTime);
            }
            else
            {
                Debug.LogError("already started" + name);
            }
        }
        protected virtual void AutoComplete()
        {
            if(!Complete) OnEndExecute(false);
        }
        public virtual void OnEndExecute(bool force)
        {
            if (!Complete)
            {
                CoreEndExecute(force);
            }
        }
        public virtual void CoreEndExecute(bool force)
        {
            if (log) Debug.Log("onEnd Execute Hook :" + this + ":" + force);
            if (!_complete)
            {
                _started = true;
                _complete = true;
                onBeforeEndExecuted.Invoke(force);

                if (onEndExecute != null)
                {
                    onEndExecute.Invoke();
                }
                if (autoComplete)
                {
                    coroutineCtrl.Cansalce(AutoComplete);
                }
            }
            else
            {
                Debug.LogError("already completed" + this);
            }

        }
        public virtual void OnUnDoExecute()
        {
            _started = false;
            _complete = false;
            if (autoComplete)
            {
                coroutineCtrl.Cansalce(AutoComplete);
            }
        }
    }
}