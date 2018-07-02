using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem.Actions
{

    [NodeGraph.CustomNode("Operate/Rotate", 18, "InteractSystem")]
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
    }
}