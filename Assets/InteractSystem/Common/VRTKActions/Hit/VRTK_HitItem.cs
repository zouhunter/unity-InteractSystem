using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using InteractSystem;
using Graph = InteractSystem.Graph;
namespace InteractSystem.VRTKActions
{
    public class VRTK_HitItem : ActionItem
    {
        [SerializeField]
        protected int clickableCount = 1;
        [SerializeField]
        protected float autoCompleteTime = 2;

        [SerializeField]
        private TouchAbleFeature touchFeature = new TouchAbleFeature();
        [SerializeField]
        private CompleteAbleItemFeature completeAbleFeature = new CompleteAbleItemFeature();
        protected CoroutineController coroutineCtrl { get { return CoroutineController.Instence; } }
        public const string layer = "i:clickitem";

        public override bool OperateAble
        {
            get { return clickableCount > targets.Count; }
        }

        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();

            touchFeature.Init(this, layer);
            touchFeature.RegistOnTouch(SetComplete);
            features.Add(touchFeature);

            completeAbleFeature.Init(this, AutoExecute);
            features.Add(completeAbleFeature);

            return features;
        }

        public void SetComplete()
        {
            completeAbleFeature.OnComplete(firstLock);
        }

        public void AutoExecute(Graph.OperaterNode node)
        {
            coroutineCtrl.DelyExecute(()=>completeAbleFeature.OnComplete(firstLock), autoCompleteTime);
        }
    }
}