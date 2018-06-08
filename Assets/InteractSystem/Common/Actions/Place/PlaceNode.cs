using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem.Common.Actions
{
    public abstract class PlaceNode<T> : RuntimeNode<T> where T:PlaceItem
    {
        public bool autoInstall;//自动安装
        public bool ignorePass;//反忽略
        public Transform passBy;//路过
        public bool straightMove;//直线移动
        public bool ignoreMiddle;//忽略中间点
        public bool hideOnInstall;//安装完后隐藏
        public virtual bool AlreadyPlaced { get { return obj != null; } }
        public virtual PlaceElement obj { get; protected set; }
    
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
            //CompleteElements(this, true);
        }
        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);
            //Collider.enabled = false;
            //CompleteElements(this, false);
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
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="arg0"></param>
        protected override void OnAdd(T arg0)
        {
            base.OnAdd(arg0);
            //if (arg0 is PlaceElement)
            //{
            //    if (arg0.Name == Name)
            //    {
            //        var placeItem = arg0 as PlaceElement;
            //        if (!elementPool.Contains(placeItem))
            //        {
            //            elementPool.ScureAdd(placeItem);
            //            if (Started && !Completed)
            //            {
            //                placeItem.StepActive();
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="arg0"></param>
        protected override void OnRemove(T arg0)
        {
            //if (arg0.IsRuntimeCreated && arg0 is PlaceElement)
            //{
            //    if(arg0.Name == Name)
            //    {
            //        var placeItem = arg0 as PlaceElement;
            //        if (elementPool.Contains(placeItem))
            //        {
            //            elementPool.ScureRemove(placeItem);
            //        }
            //    }
            //}
        }

        public virtual PlaceElement Detach()
        {
            PlaceElement old = obj;
            old.onInstallOkEvent -= OnInstallComplete;
            old.onUnInstallOkEvent -= OnUnInstallComplete;
            obj = default(PlaceElement);
            return old;
        }

        private void ActiveElements(Graph.OperaterNode element) 
        {
            //var actived = startedList.Find(x => x.Name == element.Name);
            //if (actived == null)
            //{
            //    for (int i = 0; i < elementPool.Count; i++)
            //    {
            //        if (log)
            //            Debug.Log("ActiveElements:" + element.Name + (!elementPool[i].Active && !elementPool[i].HaveBinding));

            //        if (!elementPool[i].Active && !elementPool[i].HaveBinding)
            //        {
            //            elementPool[i].StepActive();
            //        }
            //    }
            //}
            //lockQueue.Add(element);
        }

        //private void CompleteElements(ActionObj element, bool undo)
        //{
        //    lockQueue.Remove(element);
        //    var active = lockQueue.Find(x => x.Name == element.Name);
        //    if (active == null)
        //    {
        //        for (int i = 0; i < elementPool.Count; i++)
        //        {
        //            if (log) Debug.Log("CompleteElements:" + element.Name + elementPool[i].Active);

        //            if (elementPool[i].Active)
        //            {
        //                if (undo)
        //                {
        //                    elementPool[i].StepUnDo();
        //                }
        //                else
        //                {
        //                    elementPool[i].StepComplete();
        //                }
        //            }
        //        }
        //    }


        //}

    }
}