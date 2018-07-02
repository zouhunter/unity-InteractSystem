using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem.Actions
{
    [NodeGraph.CustomNode("Operate/Detach", 16, "InteractSystem")]
    public class DetachNode :Graph.OperaterNode
    {
        public CompleteAbleCollectNodeFeature completeableFeature = new CompleteAbleCollectNodeFeature(typeof(DetachItem));

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
            DetachCtrl.Instence.RegistLock(this);
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            DetachCtrl.Instence.RemoveLock(this);
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            DetachCtrl.Instence.RemoveLock(this);
        }
    }
}
