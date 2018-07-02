using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractSystem.Actions
{
    public abstract class PlaceItem : ActionItem
    {
        public bool autoInstall;//自动安装
        public bool ignorePass;//反忽略
        public Transform passBy;//路过
        public bool straightMove;//直线移动
        public bool ignoreMiddle;//忽略中间点
        public bool hideOnInstall;//安装完后隐藏

        [SerializeField]
        protected ContentActionItemFeature contentFeature = new ContentActionItemFeature(typeof(PlaceElement));
        [SerializeField]
        protected CompleteAbleItemFeature completeFeature = new CompleteAbleItemFeature();
        [SerializeField]
        protected ClickAbleFeature clickAbleFeature = new ClickAbleFeature();//可点击 

        protected ElementController elementCtrl { get { return ElementController.Instence; } }
        public virtual bool AlreadyPlaced { get { return contentFeature.Element != null; } }

        public override bool OperateAble
        {
            get
            {
                return targets == null || targets.Count == 0;
            }
        }
        public const string placePosLayer = "i:placepos";

        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();

            contentFeature.Init(this);
            features.Add(contentFeature);

            completeFeature.Init(this,AutoExecute);
            features.Add(completeFeature);

            clickAbleFeature.Init(this, placePosLayer);
            features.Add(clickAbleFeature);

            return features;
        }
        public abstract void AutoExecute(Graph.OperaterNode node);

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
                PlaceElement obj = GetUnInstalledObj(contentFeature.ElementName);
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

        protected virtual void OnInstallComplete() {
            contentFeature.Element.StepComplete();
        }

        protected virtual void OnUnInstallComplete() {
            contentFeature.Element.StepUnDo();
        }

        public virtual void Attach(PlaceElement obj)
        {
            if (contentFeature.Element != null)
            {
                Debug.LogError(this + "allready attached");
            }

            contentFeature.Element = obj;
            obj.onInstallOkEvent += OnInstallComplete;
            obj.onUnInstallOkEvent += OnUnInstallComplete;
        }

        public virtual PlaceElement Detach()
        {
            PlaceElement old = contentFeature.Element as PlaceElement;
            old.onInstallOkEvent -= OnInstallComplete;
            old.onUnInstallOkEvent -= OnUnInstallComplete;
            contentFeature.Element = default(PlaceElement);
            return old;
        }
    }
}