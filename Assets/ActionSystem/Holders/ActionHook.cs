using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    public abstract class ActionHook : MonoBehaviour, IActionObj
    {
        protected bool _complete;
        public bool Complete { get { return _complete; } }
        protected bool _started;
        public bool Started { get { return _started; } }
        protected bool auto;
        [SerializeField,Range(0,10)]
        private int queueID;
        public int QueueID
        {
            get
            {
                return queueID;
            }
        }
        protected bool autoComplete = true;
        protected float autoTime = 2;
        Coroutine coroutine;
        

        public UnityAction<int> onEndExecute { get; set; }

        public virtual void OnStartExecute(bool auto = false)
        {
            //Debug.Log("onStart Execute Hook :" + name,gameObject);
            this.auto = auto;
            if (!_started)
            {
                _started = true;
                _complete = false;
                gameObject.SetActive(true);
                if (autoComplete && coroutine == null)
                    coroutine = StartCoroutine(AutoComplete());
            }
            else
            {
                Debug.Log("already started" + name, gameObject);
            }
        }
        protected virtual IEnumerator AutoComplete()
        {
            yield return new WaitForSeconds(autoTime);
            OnEndExecute();
        }
        public virtual void OnEndExecute()
        {
            if (!_complete)
            {
                _started = true;
                _complete = true;
                if (onEndExecute != null) {
                    onEndExecute.Invoke(queueID);
                }
                if (autoComplete && coroutine != null){
                    StopCoroutine(coroutine);
                }
            }
            else
            {
                Debug.Log("already completed" + name, gameObject);
            }

        }

        public virtual void OnUnDoExecute()
        {
            _started = false;
            _complete = false;
            if (autoComplete && coroutine != null){
                StopCoroutine(coroutine);
            }
        }
    }
}