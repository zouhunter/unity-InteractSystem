using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem.Actions
{

    [NodeGraph.CustomNode("Operate/Rotate", 19, "InteractSystem")]
    public class RotateNode : Graph.OperaterNode
    {
        [SerializeField]
        protected CompleteAbleCollectNodeFeature completeableFeature = new CompleteAbleCollectNodeFeature(typeof(RotateItem));

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            completeableFeature.SetTarget(this);
            features.Add(completeableFeature);
            return features; 
        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            RotateCtrl.Instence.RegistLock(this);
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            RotateCtrl.Instence.RemoveLock(this);
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            RotateCtrl.Instence.RemoveLock(this);
        }
    }
}