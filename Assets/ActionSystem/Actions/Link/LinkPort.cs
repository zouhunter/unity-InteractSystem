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
        public Renderer Render {
            get
            {
                return _render;
            }
        }
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
        #endregion
        [SerializeField]
        private Renderer _render;
        [SerializeField]
        private bool _renderActive;
        private int _nodeId;
        public List<LinkInfo> _connectAble;

        void Awake()
        {
            gameObject.layer = Layers.nodeLayer;
            if (_render == null)
                _render = GetComponentInChildren<Renderer>();
        }
        private void OnEnable()
        {
            _nodeId = transform.GetSiblingIndex();
            _render.enabled = _renderActive;
        }

        public bool Attach(LinkPort item)
        {
            item.ConnectedNode = this;
            ConnectedNode = item;
            _render.enabled = _renderActive;
            return true;
        }

        public void ResetTransform()
        {
            _render.enabled = _renderActive;
            if (ConnectedNode != null)
            {
                LinkInfo connect = connectAble.Find(x => { return x.itemName == ConnectedNode.Body.Name && x.nodeId == ConnectedNode.NodeID; });
                if (connect != null){
                    Body.ResetBodyTransform(ConnectedNode.Body, connect.relativePos, connect.relativeDir);
                }
            }
        }


        public LinkPort Detach()
        {
            _render.enabled = false;
            LinkPort outItem = ConnectedNode;
            if (ConnectedNode != null)
            {
                ConnectedNode.ConnectedNode = null;
                ConnectedNode = null;
            }
            return outItem;
        }
    }

}