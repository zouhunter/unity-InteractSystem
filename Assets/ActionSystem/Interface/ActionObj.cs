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
    public abstract class ActionObj:MonoBehaviour, ISortAble
    {
        public bool startActive;
        public bool endActive;
        protected bool _complete;
        public bool Complete { get { return _complete; } }
        protected bool _started;
        public bool Started { get { return _started; } }
        [SerializeField]
        private int queueID;
        public int QueueID
        {
            get
            {
                return queueID;
            }
        }
        public UnityAction<int> onEndExecute;
        public UnityEvent onBeforeComplete;
        public UnityEvent onBeforeStart;
        public UnityEvent onBeforeUnDo;

        protected virtual void Start()
        {
            gameObject.SetActive(startActive);
        }

        public virtual void OnStartExecute()
        {
            if(!_started)
            {
                onBeforeStart.Invoke();
                _started = true;
                _complete = false;
                gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("already started" ,gameObject);
            }
           
        }
        public virtual void OnEndExecute()
        {
            if(!_complete)
            {
                onBeforeComplete.Invoke();
                _started = true;
                _complete = true;
                gameObject.SetActive(endActive);
                if (onEndExecute != null)
                {
                    onEndExecute.Invoke(queueID);
                }
            }
            else
            {
                Debug.Log("already completed", gameObject);
            }
          
        }
        public virtual void OnUnDoExecute()
        {
            _started = false;
            _complete = false;
            gameObject.SetActive(startActive);
        }
    }
}