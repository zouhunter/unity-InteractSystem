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
        [SerializeField, Attributes.CustomField("可点击次数")]
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
            clickAbleFeature.onClick.AddListener(completeAbleFeature.OnComplete);
            features.Add(clickAbleFeature);

            completeAbleFeature.Init(this, AutoExecute);
            features.Add(completeAbleFeature);

            return features;
        }
        public override void StepActive()
        {
            base.StepActive();
            Notice(transform);
        }
        public override void StepUnDo()
        {
            base.StepUnDo();
            UnNotice(transform);
        }
        public override void StepComplete()
        {
            base.StepComplete();
            UnNotice(transform);
        }

        public void AutoExecute(Graph.OperaterNode node)
        {
            coroutineCtrl.DelyExecute(completeAbleFeature.OnComplete, autoCompleteTime);
        }
    }
}