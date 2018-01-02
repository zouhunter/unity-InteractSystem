using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public abstract class PlaceObj : ActionObj
    {
        public bool autoInstall;//自动安装
        public bool ignorePass;//反忽略
        public Transform passBy;//路过
        public bool straightMove;//直线移动
        public bool ignoreMiddle;//忽略中间点
        public bool hideOnInstall;//安装完后隐藏
        public int layer { get { return Layers.placePosLayer; } }

        public virtual GameObject Go { get { return gameObject; } }
        public virtual bool AlreadyPlaced { get { return obj != null; } }
        public virtual PickUpAbleElement obj { get; protected set; }

        protected virtual void Awake()
        {
            gameObject.layer = layer;
            onBeforeStart.AddListener(OnBeforeStart);
            onBeforeComplete.AddListener(OnBeforeComplete);
            onBeforeUnDo.AddListener(OnBeforeUnDo);
        }
        protected virtual void OnBeforeStart(bool auto)
        {
            elementCtrl.ActiveElements(this);
        }
        protected virtual void OnBeforeComplete(bool force)
        {
            elementCtrl.CompleteElements(this, false);
        }
        protected virtual void OnBeforeUnDo()
        {
            elementCtrl.CompleteElements(this, true);
        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            if (auto || autoInstall)
            {
                OnAutoInstall();
            }
        }

        protected abstract void OnAutoInstall();

        public virtual void Attach(PickUpAbleElement obj)
        {
            if (this.obj != null)
            {
                Debug.LogError(this + "allready attached");
            }

            this.obj = obj;
            obj.onInstallOkEvent += OnInstallComplete;
            obj.onUnInstallOkEvent += OnUnInstallComplete;
        }

        protected virtual void OnInstallComplete() { }

        protected virtual void OnUnInstallComplete() { }

        public virtual PickUpAbleElement Detach()
        {
            PickUpAbleElement old = obj;
            old.onInstallOkEvent -= OnInstallComplete;
            old.onUnInstallOkEvent -= OnUnInstallComplete;
            obj = default(PickUpAbleElement);
            return old;
        }
    }
}