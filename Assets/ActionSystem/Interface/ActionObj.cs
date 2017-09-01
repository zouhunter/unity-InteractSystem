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
    public abstract class ActionObj:MonoBehaviour,IActionCommand
    {
        [SerializeField]
        private string _stepName;
        public bool startActive;
        public bool endActive;
        [SerializeField]
        protected UnityEvent onBeforeActive;
        [SerializeField]
        protected UnityEvent onBeforeUnDo;
        [SerializeField]
        protected UnityEvent onBeforePlayEnd;
        protected bool _complete;
        public bool Complete { get { return _complete; } }
        protected bool _started;
        public bool Started { get { return _started; } }
        public string StepName { get { return _stepName; } }
        protected virtual void Start()
        {
            gameObject.SetActive(startActive);
        }
        public virtual void StartExecute(bool forceAuto = false)
        {
            onBeforeActive.Invoke();
            _started = true;
            gameObject.SetActive(true);
        }
        public virtual void EndExecute()
        {
            onBeforePlayEnd.Invoke();
            _complete = true;
            gameObject.SetActive(endActive);
        }
        public virtual void UnDoExecute()
        {
            onBeforeUnDo.Invoke();
            _started = false;
            _complete = false;
            gameObject.SetActive(startActive);
        }
    }
}