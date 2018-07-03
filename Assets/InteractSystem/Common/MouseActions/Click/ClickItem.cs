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
        [SerializeField]
        protected int clickableCount = 1;
        [SerializeField]
        protected float autoCompleteTime = 2;

        [SerializeField]
        private ClickAbleFeature clickAbleFeature = new ClickAbleFeature();
        [SerializeField]
        private CompleteAbleItemFeature completeAbleFeature = new CompleteAbleItemFeature();
        protected CoroutineController coroutineCtrl { get {return CoroutineController.Instence; } }
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

        public void AutoExecute(Graph.OperaterNode node)
        {
            coroutineCtrl.DelyExecute(completeAbleFeature.OnComplete, autoCompleteTime);
        }
    }
}