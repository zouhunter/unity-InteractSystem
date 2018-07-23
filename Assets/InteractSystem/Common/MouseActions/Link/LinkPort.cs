using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Actions
{
    public class LinkPort : MonoBehaviour
    {
        #region Propertys
        public LinkItem Body { get { if (_body == null) _body = GetComponentInParent<LinkItem>(); return _body; } }
        public LinkPort ConnectedNode { get; set; }
        public Vector3 Pos
        {
            get
            {
                return transform.position;
            }
        }
        public int NodeID { get { return _nodeId; }set  { if(value >=0) _nodeId = value; } }
        public List<LinkInfo> connectAble
        {
            get
            {
                return _connectAble;
            }
        }

        public float Range { get { return _range; } set { _range = value; } }
        #endregion

        private LinkItem _body;
        [SerializeField,HideInInspector]
        private int _nodeId;
        [SerializeField, Attributes.DefultCollider("碰撞体")]
        private Collider m_collider;
        [SerializeField, Range(0.1f, 2), HideInInspector]
        private float _range = 0.5f;
        [HideInInspector]
        public List<LinkInfo> _connectAble = new List<LinkInfo>();
        public const string layer = "i:linknode";

        private void Awake()
        {
            InitLayer();
        }

        public void InitLayer()
        {
            if (m_collider == null)
            {
                m_collider = GetComponentInChildren<Collider>();
            }
            if (m_collider && m_collider.gameObject.layer != LayerMask.NameToLayer(layer))
            {
                m_collider.gameObject.layer = LayerMask.NameToLayer(layer);
            }
        }

        public void ResetTransform()
        {
            if (ConnectedNode != null)
            {
                LinkInfo connect = connectAble.Find(x => { return x.itemName == ConnectedNode.Body.Name && x.nodeId == ConnectedNode.NodeID; });
                if (connect != null)
                {
                    LinkUtil.ResetTargetTranform(Body, ConnectedNode.Body, connect.relativePos, connect.relativeDir);
                    Body.OnTranformChanged(new List<LinkItem>() { ConnectedNode.Body });
                }
            }
        }
    }

}