using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using InteractSystem;
namespace InteractSystem.VRTKActions
{
    [CustomNode("VRTK/Hit", 12, "InteractSystem")]
    public class VRTK_HitNode : InteractSystem.Graph.OperaterNode
    {
        public CompleteAbleCollectNodeFeature completeableFeature = new CompleteAbleCollectNodeFeature(typeof(VRTK_HitItem));

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            completeableFeature.SetTarget(this);
            features.Add(completeableFeature);
            return features;
        }
    }
}