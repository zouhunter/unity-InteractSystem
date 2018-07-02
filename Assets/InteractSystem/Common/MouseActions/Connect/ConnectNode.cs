using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    [NodeGraph.CustomNode("Operate/Connect", 10, "InteractSystem")]
    public class ConnectNode : Graph.OperaterNode
    {
        public CompleteAbleCollectNodeFeature completeableFeature = new CompleteAbleCollectNodeFeature(typeof(ConnectItem));

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();

            completeableFeature.SetTarget(this);
            features.Add(completeableFeature);

            return features;
        }
    }
}