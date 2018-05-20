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
        public virtual GameObject Go { get { return gameObject; } }
        public virtual bool AlreadyPlaced { get { return obj != null; } }
        public virtual PlaceElement obj { get; protected set; }
        private static List<ActionObj> lockQueue = new List<ActionObj>();
        public abstract IPlaceState PlaceState { get; }
        protected override void Awake()
        {
            base.Awake();
            InitLayer();
        }
        protected override  void OnDestroy()
        {
            base.OnDestroy();
            if(lockQueue.Contains(this))
            {
                lockQueue.Remove(this);
            }
        }

        private void InitLayer()
        {
            GetComponentInChildren<Collider>().gameObject.layer = LayerMask.NameToLayer(Layers.placePosLayer);
        }
    
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            ActiveElements(this);
            if (auto || autoInstall){
                OnAutoInstall();
            }
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            CompleteElements(this, true);
        }
        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);
            CompleteElements(this, false);
        }
        protected abstract void OnAutoInstall();

        public virtual void Attach(PlaceElement obj)
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

        public virtual PlaceElement Detach()
        {
            PlaceElement old = obj;
            old.onInstallOkEvent -= OnInstallComplete;
            old.onUnInstallOkEvent -= OnUnInstallComplete;
            obj = default(PlaceElement);
            return old;
        }

        private void ActiveElements(ActionObj element) 
        {
            var actived = lockQueue.Find(x => x.Name == element.Name);
            if (actived == null)
            {
                var objs = ElementController.Instence.GetElements<PlaceElement>(element.Name);
                if (objs == null) return;
                for (int i = 0; i < objs.Count; i++)
                {
                    if (log) Debug.Log("ActiveElements:" + element.Name + (!objs[i].Active && !objs[i].HaveBinding));

                    if (!objs[i].Active && !objs[i].HaveBinding)
                    {
                        objs[i].StepActive();
                    }
                }
            }
            lockQueue.Add(element);
        }

        private void CompleteElements(ActionObj element, bool undo)
        {
            lockQueue.Remove(element);
            var active = lockQueue.Find(x => x.Name == element.Name);
            if (active == null)
            {
                var objs = ElementController.Instence.GetElements<PlaceElement>(element.Name);
                if (objs == null) return;
                for (int i = 0; i < objs.Count; i++)
                {
                    if (log) Debug.Log("CompleteElements:" + element.Name + objs[i].Active);

                    if (objs[i].Active)
                    {
                        if (undo)
                        {
                            objs[i].StepUnDo();
                        }
                        else
                        {
                            objs[i].StepComplete();
                        }
                    }
                }
            }


        }

    }
}