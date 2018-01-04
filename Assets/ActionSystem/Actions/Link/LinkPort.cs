using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    [ExecuteInEditMode]
    public class LinkPort : MonoBehaviour
    {
        #region Propertys
        public LinkItem Body { get; set; }
        public LinkPort ConnectedNode { get; set; }
        public Vector3 Pos
        {
            get
            {
                return transform.position;
            }
        }
        public int NodeID { get { return _nodeId; } }
        public List<LinkInfo> connectAble
        {
            get
            {
                return _connectAble;
            }
        }

        public float Range { get { return _range; } }
        #endregion
        private int _nodeId;
        [SerializeField,Range(0.1f,100)]
        private float _range = 0.5f;
        public List<LinkInfo> _connectAble;

        void Awake()
        {
            InitLayer();
        }
        private void OnEnable()
        {
            _nodeId = transform.GetSiblingIndex();
        }
        private void InitLayer()
        {
            gameObject.GetComponentInChildren<Collider>().gameObject.layer = Layers.nodeLayer;

        }

        public bool Attach(LinkPort item)
        {
            item.ConnectedNode = this;
            ConnectedNode = item;
            item.ResetTransform();
            item.Body.transform.SetParent(Body.transform);
            return true;
        }

        public void ResetTransform()
        {
            if (ConnectedNode != null)
            {
                LinkInfo connect = connectAble.Find(x => { return x.itemName == ConnectedNode.Body.Name && x.nodeId == ConnectedNode.NodeID; });
                if (connect != null){
                    Body.ResetBodyTransform(ConnectedNode.Body, connect.relativePos, connect.relativeDir);
                }
            }
        }
        public LinkPort Detach(Transform parent)
        {
            LinkPort outItem = ConnectedNode;
            if (ConnectedNode != null)
            {
                ConnectedNode.ConnectedNode = null;
                ConnectedNode = null;
            }
            outItem.transform.SetParent(parent);
            return outItem;
        }
    }

}