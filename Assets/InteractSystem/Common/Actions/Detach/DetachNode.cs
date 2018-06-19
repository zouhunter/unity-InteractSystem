using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem.Common.Actions
{
    [NodeGraph.CustomNode("Operate/Detach", 16, "InteratSystem")]
    public class DetachNode :Graph.OperaterNode,IRuntimeCtrl
    {
        public ControllerType CtrlType
        {
            get
            {
                return ControllerType.Detach;
            }
        }
        public CompleteAbleCollectNodeFeature completeableFeature = new CompleteAbleCollectNodeFeature(typeof(ClickItem));

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            completeableFeature.target = this;
            features.Add(completeableFeature);
            return features;
        }
    }
}
