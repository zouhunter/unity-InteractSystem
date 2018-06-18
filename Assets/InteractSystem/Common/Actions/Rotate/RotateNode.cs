using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem.Common.Actions
{

    [NodeGraph.CustomNode("Operate/Rotate", 18, "InteratSystem")]
    public class RotateNode : Graph.OperaterNode, IRuntimeCtrl
    {
        public ControllerType CtrlType
        {
            get
            {
                return ControllerType.Rotate;
            }
        }
        [SerializeField]
        protected CompleteAbleCollectNodeFeature completeableFeature = new CompleteAbleCollectNodeFeature(typeof(RotateItem));

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            completeableFeature.target = this;
            completeableFeature.onComplete = ()=> OnEndExecute(false);
            return new List<OperateNodeFeature>() { completeableFeature };
        }
    }
}