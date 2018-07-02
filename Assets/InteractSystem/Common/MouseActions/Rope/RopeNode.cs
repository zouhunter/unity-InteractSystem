using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    [NodeGraph.CustomNode("Operate/Rope", 10, "InteractSystem")]
    public class RopeNode : Graph.OperaterNode
    {
        public CompleteAbleCollectNodeFeature completeFeature = new CompleteAbleCollectNodeFeature(typeof(RopeItem));

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            completeFeature.SetTarget(this);
            features.Add(completeFeature);
            return features;
        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            RopeCtrl.Instence.RegistLock(this);
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            RopeCtrl.Instence.RemoveLock(this);
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            RopeCtrl.Instence.RemoveLock(this);
        }
    }
}