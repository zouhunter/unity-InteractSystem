using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    [NodeGraph.CustomNode("Operate/Connect", 13, "InteractSystem")]
    public class ConnectNode : Graph.OperaterNode
    {
        [System.Serializable]
        public class PointGroup
        {
            public int p1;
            public int p2;
            public Material material;
            public float width = 0.1f;
        }

        [SerializeField, Attributes.CustomField("材质")]
        protected Material lineMaterial;

        [SerializeField, Attributes.CustomField("线宽")]
        protected float lineWight = 0.1f;
       
        protected float autoTime { get { return Config.Instence.autoExecuteTime; } }

        public List<PointGroup> connectGroup;

        [SerializeField]
        protected CollectNodeFeature collectNodeFeature = new CollectNodeFeature(typeof(LinkItem));
        private Dictionary<int, LineRenderer> lineRenders = new Dictionary<int, LineRenderer>();
        private Dictionary<int, Vector3[]> positionDic = new Dictionary<int, Vector3[]>();

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();

            collectNodeFeature.SetTarget(this);
            collectNodeFeature.onAddToPool = OnAddedToPool;
            collectNodeFeature.onRemoveFromPool = OnRemovedFromPool;

            features.Add(collectNodeFeature);
            return features;
        }

        private void OnRemovedFromPool(ISupportElement arg0)
        {
        }

        private void OnAddedToPool(ISupportElement arg0)
        {
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