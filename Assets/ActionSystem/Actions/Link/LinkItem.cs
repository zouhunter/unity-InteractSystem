using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class LinkItem : PickUpAbleItem, ISupportElement
    {
        [SerializeField]
        private string _name;
        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    _name = name;
                }
                return _name;
            }
        }

        public GameObject Body
        {
            get
            {
                return gameObject;
            }
        }
        public bool Used { get; set; }
        private List<LinkPort> _childNodes = new List<LinkPort>();
        public List<LinkPort> ChildNodes
        {
            get
            {
                InitPorts();
                return _childNodes;
            }
        }
        public Transform Trans
        {
            get
            {
                return transform;
            }
        }
        public bool IsRuntimeCreated { get; set; }
        public bool Active { get; protected set; }
        public LinkObj BindingTarget { get; set; }
        public bool Connected { get; set; }
        private ElementController elementCtrl { get { return ElementController.Instence; } }
        private Vector3 startPos;
        private Quaternion startRot;
        private List<LinkPort> _groupNodes = new List<LinkPort>();
        private List<LinkItem> _context = new List<LinkItem>();
        public List<LinkPort> GroupNodes
        {
            get
            {
                _groupNodes.Clear();
                _context.Clear();
                RetiveNodes(_context, this);
                return _groupNodes;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            InitPorts();
            InitLayer();
            elementCtrl.RegistElement(this);
        }

        protected virtual void Start()
        {
            startPos = transform.position;
            startRot = transform.rotation;
        }
        protected virtual void OnDestroy()
        {
            elementCtrl.RemoveElement(this);
        }

        private void InitPorts()
        {
            if (_childNodes == null || _childNodes.Count == 0)
            {
                var nodeItems = GetComponentsInChildren<LinkPort>(true);
                _childNodes.AddRange(nodeItems);
            }
        }

        private void RetiveNodes(List<LinkItem> context, LinkItem linkItem)
        {
            context.Add(linkItem);
            _groupNodes.AddRange(linkItem.ChildNodes);

            foreach (var item in linkItem.ChildNodes)
            {
                if (item.ConnectedNode != null && !context.Contains(item.ConnectedNode.Body))
                {
                    RetiveNodes(context, item.ConnectedNode.Body);
                }
            }
        }

        private void InitLayer()
        {
            Collider.gameObject.layer = LayerMask.NameToLayer(Layers.pickUpElementLayer);
        }


        public override void SetPosition(Vector3 pos)
        {
            transform.position = pos;
            OnTranformChanged();
        }

        public void OnTranformChanged()
        {
            _context.Clear();
            LinkUtil.UpdateBrotherPos(this, _context);
        }

        public void StepActive()
        {
            PickUpAble = true;
            Active = true;
        }

        public void StepComplete()
        {
            PickUpAble = false;
            Active = false;
        }

        public void StepUnDo()
        {
            PickUpAble = false;
            Active = false;
            transform.position = startPos;
            transform.rotation = startRot;
        }
    }

}