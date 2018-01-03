using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    [ExecuteInEditMode]
    public class PortItemBehaiver : MonoBehaviour
    {
        #region Propertys
        public PortParentBehaiver Body { get; set; }
        public PortItemBehaiver ConnectedNode { get; set; }
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
        public List<ConnectAble> connectAble
        {
            get
            {
                return _connectAble;
            }
        }
        #endregion
        [SerializeField]
        private Renderer _render;
        public bool _renderActive;
        public int _nodeId;
        public List<ConnectAble> _connectAble;

        void Awake()
        {
            gameObject.layer = Layers.nodeLayer;
            if (_render == null) _render = GetComponentInChildren<Renderer>();
            _render.enabled = false;
        }

        public bool Attach(PortItemBehaiver item)
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
                ConnectAble connect = connectAble.Find(x => { return x.itemName == ConnectedNode.Body.Name && x.nodeId == ConnectedNode.NodeID; });
                if (connect != null){
                    Body.ResetBodyTransform(ConnectedNode.Body, connect.relativePos, connect.relativeDir);
                }
            }
        }


        public PortItemBehaiver Detach()
        {
            _render.enabled = false;
            PortItemBehaiver outItem = ConnectedNode;
            if (ConnectedNode != null)
            {
                ConnectedNode.ConnectedNode = null;
                ConnectedNode = null;
            }
            return outItem;
        }
    }

}