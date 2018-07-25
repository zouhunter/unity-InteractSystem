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
    public class ClickNode : Graph.OperaterNode
    {
        public QueueCollectNodeFeature completeableFeature = new QueueCollectNodeFeature(typeof(ClickItem));

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
            ClickCtrl.Instence.RegistLock(this);
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            ClickCtrl.Instence.RemoveLock(this);
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            ClickCtrl.Instence.RemoveLock(this);
        }
    }
}