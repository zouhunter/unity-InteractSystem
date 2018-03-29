using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class LinkItem : PickUpAbleElement
    {
        private List<LinkPort> _childNodes = new List<LinkPort>();
        public Transform Trans
        {
            get
            {
                return transform;
            }
        }
        public ElementController elementCtrl { get { return ElementController.Instence; } }
        public List<LinkPort> GroupNodes
        {
            get
            {
                _groupNodes.Clear();
                linkLock.Clear();
                RetiveNodes(linkLock, this);
                return _groupNodes;
            }
        }
        public List<LinkPort> ChildNodes
        {
            get
            {
                InitPorts();
                return _childNodes;
            }
        }
        public bool Used { get { return elementCtrl.IsLocked(this); } }
        public bool Connected { get { return HaveConnected(this); } }
        public bool CanUse
        {
            get
            {
                return CalcuteConnected() < ChildNodes.Count;
            }
        }
        public event UnityAction onConnected;

        [SerializeField]
        private Renderer m_render;//可选择提示
        [SerializeField]
        private Color highLightColor = Color.green;


        private IHighLightItems highLighter;
        private Vector3 startPos;
        private Quaternion startRot;
        private List<LinkPort> _groupNodes = new List<LinkPort>();
        private List<LinkItem> linkLock = new List<LinkItem>();

        protected override void Awake()
        {
            base.Awake();
            InitPorts();
            InitLayer();
            InitHighLighter();
        }

        protected override void Start()
        {
            base.Start();
            startPos = transform.position;
            startRot = transform.rotation;
        }

        protected override void Update()
        {
            base.Update();
            if (m_render == null) return;

            if (Active)
            {
                highLighter.HighLightTarget(m_render, highLightColor);
            }
            else
            {
                highLighter.UnHighLightTarget(m_render);
            }
        }
        private void InitHighLighter()
        {
            if (m_render == null) m_render = GetComponentInChildren<Renderer>();
            highLighter = new ShaderHighLight();
        }

        private void InitPorts()
        {
            if (_childNodes == null || _childNodes.Count == 0)
            {
                var nodeItems = GetComponentsInChildren<LinkPort>(true);
                _childNodes.AddRange(nodeItems);
            }
        }

        public LinkPort[] GetLinkedPorts()
        {
            var connenctedPos = new List<LinkPort>();
            foreach (var item in ChildNodes)
            {
                if(item.ConnectedNode != null)
                {
                    connenctedPos.Add(item);
                }
            }
            return connenctedPos.ToArray();
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

        private int CalcuteConnected()
        {
            int count = 0;
            foreach (var child in ChildNodes)
            {
                if(child.ConnectedNode != null)
                {
                    count++;
                }
            }
            return count;
        }


        public override void SetPosition(Vector3 pos)
        {
            transform.position = pos;
            OnTranformChanged();
        }

        public void OnTranformChanged()
        {
            linkLock.Clear();
            LinkUtil.UpdateBrotherPos(this, linkLock);
        }

        /// <summary>
        /// 判断是否已经连接过了
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool HaveConnected(LinkItem item)
        {
            bool connected = false;
            foreach (var child in item.ChildNodes)
            {
                connected |= (child.ConnectedNode != null);
            }
            return connected;
        }

        public void OnConnected()
        {
            if(this.onConnected != null)
            {
                onConnected.Invoke();
            }
        }

        public override void StepActive()
        {
            PickUpAble = true;
            Active = true;
        }

        public override void StepComplete()
        {
            PickUpAble = false;
            Active = false;
        }
        public override void StepUnDo()
        {
            PickUpAble = false;
            Active = false;
            transform.position = startPos;
            transform.rotation = startRot;
        }
        public override void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
        /// <summary>
        /// 激活匹配点
        /// </summary>
        /// <param name="pickedUp"></param>
        //public void TryActiveLinkPort(LinkItem pickedUp)
        //{
        //    for (int i = 0; i < pickedUp.ChildNodes.Count; i++)
        //    {
        //        var node = pickedUp.ChildNodes[i];
        //        if (node.ConnectedNode == null && node.connectAble.Count > 0)
        //        {
        //            for (int j = 0; j < node.connectAble.Count; j++)
        //            {
        //                var info = node.connectAble[j];

        //                var otheritem = (from x in linkPool
        //                                 where (x != null && x != pickedUp && x.Name == info.itemName)
        //                                 select x).FirstOrDefault();

        //                if (otheritem != null)
        //                {
        //                    var otherNode = otheritem.ChildNodes[info.nodeId];
        //                    if (otherNode != null && otherNode.ConnectedNode == null)
        //                    {
        //                        angleCtrl.UnNotice(anglePos);
        //                        anglePos = otherNode.transform;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }

    }