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

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            ConnectCtrl.Instence.RegistLock(this);
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            ConnectCtrl.Instence.RemoveLock(this);
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            ConnectCtrl.Instence.RemoveLock(this);
        }
    }
}