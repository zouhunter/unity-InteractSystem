using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
    [NodeGraph.CustomNode("Operate/Connect", 10, "InteratSystem")]
    public class ConnectNode : Graph.OperaterNode,IRuntimeCtrl
    {
        public ControllerType CtrlType
        {
            get
            {
                return ControllerType.Connect;
            }
        }
        public CompleteAbleCollectNodeFeature completeableFeature = new CompleteAbleCollectNodeFeature(typeof(ConnectItem));

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            completeableFeature.target = this;
            features.Add(completeableFeature);
            return features;
        }
    }
}