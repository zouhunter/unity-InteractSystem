using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem
{
    public class ActionHook : InteractObject
    {
        [SerializeField, Attributes.Range(0, 10)]
        private int queueID;
        public int QueueID
        {
            get
            {
                return queueID;
            }
        }
        public string CameraID { get { return null; } }
        public UnityAction onEndExecute { get; set; }
        public ExecuteStatu Statu { get { return status; } }

        [HideInInspector]
        public Toggle.ToggleEvent onBeforeEndExecuted;
        public static bool log = false;
        protected ExecuteStatu status;
        protected InteractObject operater;

        public void SetContext(InteractObject operater)
        {
            this.operater = operater;
        }

        protected virtual void OnEnable()
        {
            status = ExecuteStatu.UnStarted;
        }

        public virtual void OnStartExecute(bool auto)
        {
            if (log) Debug.Log("onStart Execute Hook :" + this);
            if (status == ExecuteStatu.UnStarted)
            {
                status = ExecuteStatu.Executing;
                CoreStartExecute();
            }
        }
        protected virtual void CoreStartExecute() { }

        public virtual void OnEndExecute(bool force)
        {
            if (status != ExecuteStatu.Completed)
            {
                status = ExecuteStatu.Completed;
                OnBeforeEndExecute();
                CoreEndExecute(force);
            }
            else
            {
                Debug.LogError("already completed" + this);
            }
        }

        protected virtual void OnBeforeEndExecute()
        {

        }

        public virtual void CoreEndExecute(bool force)
        {
            if (log) Debug.Log("onEnd Execute Hook :" + this + ":" + force);

            onBeforeEndExecuted.Invoke(force);

            if (!force && onEndExecute != null)
            {
                onEndExecute.Invoke();
            }
        }
        public virtual void OnUnDoExecute()
        {
            status = ExecuteStatu.UnStarted;
        }
    }
}