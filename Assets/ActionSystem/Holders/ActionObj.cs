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
    public class ActionObj : MonoBehaviour, ISortAble
    {
        public bool startActive;
        public bool endActive;
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
        [SerializeField]
        protected GameObject viewObj;
        [SerializeField]
        protected Color highLightColor = Color.green;
        public UnityAction<int> onEndExecute;
        public UnityEvent onBeforeStart;
        public UnityEvent onBeforeUnDo;
        public UnityEvent onBeforeComplete;
        private IHighLightItems highLighter;
        private ActionHook[] hooks;//外部结束钩子
        public ActionHook[] Hooks { get { return hooks; } }
        private HookCtroller hookCtrl;
        protected virtual void Start()
        {
            InitRender();
            gameObject.SetActive(startActive);
            hooks = GetComponentsInChildren<ActionHook>(false);
            if (hooks.Length > 0)
            {
                hookCtrl = new HookCtroller(this);
            }
        }
        protected virtual void Update()
        {
            if (Started && Complete) return;

            if(Started && !Complete)
            {
                if (Setting.highLightNotice) highLighter.HighLightTarget(viewObj, highLightColor);
            }
            else
            {
                if (Setting.highLightNotice) highLighter.UnHighLightTarget(viewObj);
            }
        }
        private void InitRender()
        {
            if (viewObj == null) viewObj = gameObject;
            highLighter = new ShaderHighLight();
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
            Debug.Log("onEndExecute" + name);

            if (!_complete)
            {
                if (Setting.highLightNotice) highLighter.UnHighLightTarget(viewObj);

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
            if (hooks.Length > 0)
            {
                hookCtrl.OnUnDoExecute();
            }
        }
    }
}