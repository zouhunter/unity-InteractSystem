using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractSystem.Common.Actions
{
    public abstract class PlaceItem : CompleteAbleContentActionItem<PlaceElement>
    {
        public bool autoInstall;//自动安装
        public bool ignorePass;//反忽略
        public Transform passBy;//路过
        public bool straightMove;//直线移动
        public bool ignoreMiddle;//忽略中间点
        public bool hideOnInstall;//安装完后隐藏

        public virtual bool AlreadyPlaced { get { return element != null; } }
        protected override string LayerName
        {
            get
            {
                return Layers.placePosLayer;
            }
        }
        public override bool OperateAble
        {
            get
            {
                return targets == null || targets.Count == 0;
            }
        }
        public abstract bool CanPlace(PlaceElement element, out string why);
        public abstract void PlaceObject(PlaceElement pickup);
        public override void StepUnDo()
        {
            base.StepUnDo();
            if (AlreadyPlaced)
            {
                var detachedObj = Detach();
                detachedObj.QuickUnInstall();
                detachedObj.StepUnDo();
            }
        }
        public override void StepComplete()
        {
            base.StepComplete();
            if (!AlreadyPlaced)
            {
                PlaceElement obj = GetUnInstalledObj(elementName);
                Attach(obj);
                obj.QuickInstall(this, true);
                obj.StepComplete();
            }
        }

        /// <summary>
        /// 找出一个没有安装的元素
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public PlaceElement GetUnInstalledObj(string elementName)
        {
            var elements = elementCtrl.GetElements<PlaceElement>(elementName, true);
            if (elements != null)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    if (!elements[i].HaveBinding)
                    {
                        return elements[i];
                    }
                }
            }
            throw new Exception("配制错误,缺少" + elementName);
        }

        protected virtual void OnInstallComplete() { }

        protected virtual void OnUnInstallComplete() { }

        public virtual void Attach(PlaceElement obj)
        {
            if (this.element != null)
            {
                Debug.LogError(this + "allready attached");
            }

            this.element = obj;
            obj.onInstallOkEvent += OnInstallComplete;
            obj.onUnInstallOkEvent += OnUnInstallComplete;
        }

        public virtual PlaceElement Detach()
        {
            PlaceElement old = element;
            old.onInstallOkEvent -= OnInstallComplete;
            old.onUnInstallOkEvent -= OnUnInstallComplete;
            element = default(PlaceElement);
            return old;
        }
    }
}