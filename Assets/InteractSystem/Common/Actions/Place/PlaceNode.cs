using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem.Common.Actions
{
    [NodeGraph.CustomNode("Operate/Place", 10, "InteratSystem")]
    public class PlaceNode : Graph.OperaterNode,IRuntimeCtrl
    {
        [SerializeField]
        protected CompleteAbleCollectNodeFeature completeAbleNodeFeature = new CompleteAbleCollectNodeFeature(typeof(PlaceItem));
        public ControllerType CtrlType
        {
            get
            {
                return ControllerType.Place;
            }
        }
        protected override List<OperateNodeFeature> RegistFeatures()
        {
            completeAbleNodeFeature.SetTarget(this);
            return new List<OperateNodeFeature>() { completeAbleNodeFeature };
        }
    }
}