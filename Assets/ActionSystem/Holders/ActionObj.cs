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
    public class ActionObj : MonoBehaviour, IActionObj
    {
        public bool startActive;
        public bool endActive;
        protected bool _complete;
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

        public UnityAction<int> onEndExecute { get; set; }
        public UnityEvent onBeforeStart;
        public UnityEvent onBeforeUnDo;
        public UnityEvent onBeforeComplete;
        private ActionHook[] hooks;//外部结束钩子
        public ActionHook[] Hooks { get { return hooks; } }
        private HookCtroller hookCtrl;
        private AngleCtroller angleCtrl { get { return AngleCtroller.Instance; } }

        protected virtual void Start()
        {
            hooks = GetComponentsInChildren<ActionHook>(false);
            if (hooks.Length > 0)
            {
                hookCtrl = new HookCtroller(this);
                //Debug.Log(name + "registHooks :" + hooks.Length);
            }
            gameObject.SetActive(startActive);

        }
        protected virtual void Update()
        {
            if (Started && Complete) return;

            if (!Setting.highLightNotice) return;

            if (Started && !Complete)
            {
                if (angleCtrl) angleCtrl.Notice(transform);
            }
            else
            {
                if (angleCtrl) angleCtrl.UnNotice(transform);
            }
        }

        public virtual void OnStartExecute(bool auto = false)
        {
            this.auto = auto;
            if (!_started)
            {
                onBeforeStart.Invoke();
                _started = true;
                _complete = false;
                gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("already started", gameObject);
            }
        }

        public virtual void TryEndExecute()
        {
            if (hooks.Length > 0)
            {
                if (hookCtrl.Complete)
                {
                    OnEndExecute();
                }
                else if (!hookCtrl.Started)
                {
                    hookCtrl.OnStartExecute(auto);
                }
                else
                {
                    Debug.Log("wait:" + name);
                }
            }
            else
            {
                OnEndExecute();
            }
        }

        public virtual void OnEndExecute()
        {
            //Debug.Log("onEndExecute" + name);

            if (!_complete)
            {
                onBeforeComplete.Invoke();
                _started = true;
                _complete = true;
                gameObject.SetActive(endActive);
                if (onEndExecute != null)
                {
                    onEndExecute.Invoke(queueID);
                }
                if (hooks.Length > 0)
                {
                    hookCtrl.OnEndExecute();
                }

                if (angleCtrl) angleCtrl.UnNotice(transform);
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

            if (angleCtrl) angleCtrl.UnNotice(transform);

            if (hooks.Length > 0)
            {
                hookCtrl.OnUnDoExecute();
            }

        }

    }
}