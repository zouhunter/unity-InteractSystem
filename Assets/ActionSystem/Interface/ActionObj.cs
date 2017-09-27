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
    public abstract class ActionObj:MonoBehaviour
    {
        public bool startActive;
        public bool endActive;
        protected bool _complete;
        public bool Complete { get { return _complete; } }
        protected bool _started;
        public bool Started { get { return _started; } }
        protected virtual void Start()
        {
            gameObject.SetActive(startActive);
        }
        public virtual void OnStartExecute()
        {
            _started = true;
            _complete = false;
            gameObject.SetActive(true);
        }
        public virtual void OnEndExecute()
        {
            _started = true;
            _complete = true;
            gameObject.SetActive(endActive);
        }
        public virtual void OnUnDoExecute()
        {
            _started = false;
            _complete = false;
            gameObject.SetActive(startActive);
        }
    }
}