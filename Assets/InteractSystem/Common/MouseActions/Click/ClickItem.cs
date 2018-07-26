using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    public class ClickItem : ActionItem
    {
        [SerializeField, Attributes.CustomField("节点支持")]
        protected int clickableCount = 1;

        [SerializeField]
        private ClickAbleFeature clickAbleFeature = new ClickAbleFeature();
        [SerializeField]
        private CompleteAbleItemFeature completeAbleFeature = new CompleteAbleItemFeature();
        protected CoroutineController coroutineCtrl { get {return CoroutineController.Instence; } }
        protected float autoCompleteTime { get { return Config.Instence.autoExecuteTime; } }
        public const string layer = "i:clickitem";

        public override bool OperateAble
        {
            get { return clickableCount > targets.Count; }
        }

        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
         
            clickAbleFeature.Init(this, layer);
            features.Add(clickAbleFeature);

            completeAbleFeature.Init(this, OnAutoExecute);
            features.Add(completeAbleFeature);

            return features;
        }
        protected override void OnSetActive(UnityEngine.Object target)
        {
            base.OnSetActive(target);
            clickAbleFeature.RegistOnClick(TriggerComplete);
            Notice(transform);
        }


        protected override void OnSetInActive(UnityEngine.Object target)
        {
            base.OnSetInActive(target);
            clickAbleFeature.RemoveOnClick(TriggerComplete);
            UnNotice(transform);
        }

        protected void TriggerComplete()
        {
            completeAbleFeature.OnComplete(firstLock);
        }

        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            clickAbleFeature.RemoveOnClick(TriggerComplete);
            UnNotice(transform);
        }
        public void OnAutoExecute(UnityEngine.Object node)
        {
            coroutineCtrl.DelyExecute(TriggerComplete, autoCompleteTime);
        }
    }
}