using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
  
    public class ClickItem : ActionItem
    {
        [SerializeField]
        protected int clickableCount = 1;
        [SerializeField]
        protected float autoCompleteTime = 2;

        public ClickAbleFeature clickAbleFeature = new ClickAbleFeature();
        public CompleteAbleItemFeature completeAbleFeature = new CompleteAbleItemFeature();


        protected CoroutineController coroutineCtrl { get {return CoroutineController.Instence; } }
        public override bool OperateAble
        {
            get { return clickableCount > targets.Count; }
        }

        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            clickAbleFeature.target = this;
            clickAbleFeature.LayerName = Layers.clickItemLayer;
            features.Add(clickAbleFeature);
            completeAbleFeature.target = this;
            completeAbleFeature.onAutoExecute = AutoExecute;
            return features;
        }
        public void AutoExecute(Graph.OperaterNode node)
        {
            coroutineCtrl.DelyExecute(completeAbleFeature. OnComplete, autoCompleteTime);
        }
    }
}