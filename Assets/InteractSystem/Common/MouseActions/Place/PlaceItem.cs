using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractSystem.Actions
{
    public abstract class PlaceItem : ActionItem
    {
        [Attributes.CustomField("自动安装")] public bool autoInstall;//自动安装
        [Attributes.CustomField("防止忽略")] public bool ignorePass;//反忽略
        [Attributes.CustomField("中间节点")] public Transform passBy;//路过
        [Attributes.CustomField("跳过节点")] public bool ignoreMiddle;//忽略中间点
        [Attributes.CustomField("直线移动")] public bool straightMove;//直线移动
        [Attributes.CustomField("结束隐藏")] public bool hideOnInstall;//安装完后隐藏

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

            completeFeature.Init(this, OnAutoExecute);
            features.Add(completeFeature);

            clickAbleFeature.Init(this, placePosLayer);
            features.Add(clickAbleFeature);

            return features;
        }
        public abstract void OnAutoExecute(UnityEngine.Object node);

        public abstract bool CanPlace(PlaceElement element, out string why);

        public abstract void PlaceObject(PlaceElement pickup);

        protected override void OnSetActive(UnityEngine.Object target)
        {
            base.OnSetActive(target);
            Notice(transform);
        }

        protected override void OnSetInActive(UnityEngine.Object target)
        {
            base.OnSetInActive(target);
            UnNotice(transform);
        }

        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            UnNotice(transform);
            if (AlreadyPlaced)
            {
                var detachedObj = Detach();
                detachedObj.QuickUnInstall();
                detachedObj.UnDoChanges(this);
            }
        }

        /// <summary>
        /// 找出一个没有安装的元素
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public PlaceElement GetUnInstalledObj(string elementName,bool attach = true,bool active = true)
        {
            var elements = elementCtrl.GetElements<PlaceElement>(elementName, true);
            if (elements != null)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    var element = elements[i];
                    if(element.OperateAble && !element.IsPlaying)
                    {
                        if(active && !element .Actived) element.SetActive(this);
                        if (attach) Attach(element);
                        return element;
                    }
                }
            }
            throw new Exception("配制错误,缺少" + elementName);
        }

        protected abstract void OnInstallComplete();
        protected abstract void OnUnInstallComplete();

        public virtual void Attach(PlaceElement obj)
        {
            if (contentFeature.Element != null){
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