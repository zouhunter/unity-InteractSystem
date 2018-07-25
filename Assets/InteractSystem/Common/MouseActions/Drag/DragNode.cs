using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{ 
    [NodeGraph.CustomNode("Operate/Drag", 15, "InteractSystem")]
    public class DragNode :Graph.OperaterNode
    {
        public QueueCollectNodeFeature completeableFeature = new QueueCollectNodeFeature(typeof(DragItem));
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
            DragCtrl.Instence.RegistLock(this);
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            DragCtrl.Instence.RemoveLock(this);
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            DragCtrl.Instence.RemoveLock(this);
        }
    }

}