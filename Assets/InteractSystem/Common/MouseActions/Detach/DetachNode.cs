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
    }
}
