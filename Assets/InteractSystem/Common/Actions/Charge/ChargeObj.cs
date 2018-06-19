using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InteractSystem.Common.Actions
{
    /// <summary>
    /// (暂时没有考虑不足和溢出的问题)
    /// </summary>
    public class ChargeObj : Graph.OperaterNode, IRuntimeCtrl
    {
        public ControllerType CtrlType
        {
            get
            {
                return ControllerType.Charge;
            }
        }
        public CompleteAbleCollectNodeFeature completeAbleFeature = new CompleteAbleCollectNodeFeature(typeof(ChargeItem));

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            completeAbleFeature.target = this;
            return features;
        }
    }

}