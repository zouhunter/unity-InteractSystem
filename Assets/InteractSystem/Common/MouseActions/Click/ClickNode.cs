using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
namespace InteractSystem.Actions
{
    [CustomNode("Operate/Click", 12, "InteractSystem")]
    public class ClickNode : Graph.OperaterNode,IRuntimeCtrl
    {
        public ControllerType CtrlType
        {
            get
            {
                return ControllerType.Click;
            }
        }

        public CompleteAbleCollectNodeFeature completeableFeature = new CompleteAbleCollectNodeFeature(typeof(ClickItem));

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            completeableFeature.SetTarget(this);
            features.Add(completeableFeature);
            return features;
        }
    }
}