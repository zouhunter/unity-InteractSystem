using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class LinkItem : MonoBehaviour,IPickUpAbleItem
    {
        private List<LinkPort> _childNodes = new List<LinkPort>();
        public List<LinkPort> ChildNodes
        {
            get
            {
                InitPorts();
                return _childNodes;
            }
        }

        public string Name {
            get
            {
                return name;
            }
        }

        public Transform Trans
        {
            get
            {
                return transform;
            }
        }

        public bool PickUpAble { get; set; }

        [SerializeField]
        private Collider _collider;
        public Collider Collider { get { if (_collider == null) _collider = GetComponent<Collider>();return _collider; } }

        private void Awake()
        {
            InitPorts();
            InitLayer();
        }
        private void InitPorts()
        {
            if(_childNodes == null || _childNodes.Count == 0)
            {
                var nodeItems = GetComponentsInChildren<LinkPort>(true);
                _childNodes.AddRange(nodeItems);
            }
        }

        private void InitLayer()
        {
            Collider.gameObject.layer = Layers.pickUpElementLayer;
        }
        public void ResetBodyTransform(LinkItem otherParent, Vector3 rPos, Vector3 rdDir)
        {
            transform.position = otherParent.Trans.TransformPoint(rPos);
            transform.forward = otherParent.Trans.TransformDirection(rdDir);
        }

        public void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        public void OnPickUp()
        {
        }

        public void OnPickStay()
        {
        }

        public void OnPickDown()
        {
        }
    }

}