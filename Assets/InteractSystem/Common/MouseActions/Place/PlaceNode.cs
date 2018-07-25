using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem.Actions
{
    [NodeGraph.CustomNode("Operate/Place", 17, "InteractSystem")]
    public class PlaceNode : Graph.OperaterNode
    {
        [SerializeField]
        protected QueueCollectNodeFeature completeAbleNodeFeature = new QueueCollectNodeFeature(typeof(PlaceItem));
        protected override List<OperateNodeFeature> RegistFeatures()
        {
            completeAbleNodeFeature.SetTarget(this);
            return new List<OperateNodeFeature>() { completeAbleNodeFeature };
        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            PlaceCtrl.Instence.RegistLock(this);
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            PlaceCtrl.Instence.RemoveLock(this);
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            PlaceCtrl.Instence.RemoveLock(this);
        }
    }
}