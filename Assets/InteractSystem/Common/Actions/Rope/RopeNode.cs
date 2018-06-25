using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
    [NodeGraph.CustomNode("Operate/Rope", 10, "InteratSystem")]
    public class RopeNode : Graph.OperaterNode, IRuntimeCtrl
    {
        public ControllerType CtrlType
        {
            get
            {
                return ControllerType.Rope;
            }
        }

        public CompleteAbleCollectNodeFeature completeFeature = new CompleteAbleCollectNodeFeature(typeof(RopeItem));

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            completeFeature.SetTarget(this);
            features.Add(completeFeature);
            return features;
        }

    }
}