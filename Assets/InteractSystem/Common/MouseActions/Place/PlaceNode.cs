using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem.Actions
{
    [NodeGraph.CustomNode("Operate/Place", 10, "InteractSystem")]
    public class PlaceNode : Graph.OperaterNode
    {
        [SerializeField]
        protected CompleteAbleCollectNodeFeature completeAbleNodeFeature = new CompleteAbleCollectNodeFeature(typeof(PlaceItem));
        protected override List<OperateNodeFeature> RegistFeatures()
        {
            completeAbleNodeFeature.SetTarget(this);
            return new List<OperateNodeFeature>() { completeAbleNodeFeature };
        }
    }
}