using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem.Actions
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
        public bool isMatching { get; internal set; }
        public bool Visiable
        {
            get { return m_render.gameObject.activeSelf && m_render.gameObject.activeInHierarchy && m_render.enabled; }
        }
        public event UnityAction onConnected;

        [SerializeField]
        private Renderer m_render;//可选择提示
        [SerializeField]
        private Color highLightColor = Color.green;



        private IHighLightItems highLighter;
        private Vector3 startPos;
        private Quaternion startRot;
        private Vector3 lastForward = Vector3.forward;
        private List<LinkPort> _groupNodes = new List<LinkPort>();
        private List<LinkItem> linkLock = new List<LinkItem>();
        private float posHoldTime = 3f;
        private float posHoldTimer;
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
            startRot = transform.localRotation;
        }

        protected override void Update()
        {
            base.Update();

            UpdateMatchTime();

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
                if (item.ConnectedNode != null)
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
        public override void OnPickStay()
        {
            base.OnPickStay();
            lastForward = Vector3.zero;
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
                if (child.ConnectedNode != null)
                {
                    count++;
                }
            }
            return count;
        }


        public override void SetPosition(Vector3 pos)
        {
            if (!isMatching)
            {
                transform.position = pos;
                linkLock.Clear();
                OnTranformChanged(linkLock);
            }
        }

        private void UpdateMatchTime()
        {
            if (isMatching)
            {
                if (posHoldTimer < posHoldTime)
                {
                    posHoldTimer += Time.deltaTime;
                }
                else
                {
                    isMatching = false;
                    posHoldTimer = 0;
                }
            }
        }

        public override void SetViewForward(Vector3 forward)
        {
            base.SetViewForward(forward);

            if(!isMatching)
            {
                if(lastForward == Vector3.zero){
                    lastForward = forward;
                }
                else if(lastForward != forward)
                {
                    transform.localRotation = Quaternion.FromToRotation(lastForward, forward) * transform.localRotation;
                    linkLock.Clear();
                    OnTranformChanged(linkLock);
                    lastForward = forward;
                }
            }
        }

        public void OnTranformChanged(List<LinkItem> context)
        {
            LinkUtil.UpdateBrotherPos(this, context);
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
            if (this.onConnected != null)
            {
                onConnected.Invoke();
            }
        }

        public override void StepActive()
        {
            PickUpAble = true;
            Active = true;
            Collider.enabled = true;
        }

        public override void StepComplete()
        {
            PickUpAble = false;
            Active = false;
            Collider.enabled = false;
        }

        public override void StepUnDo()
        {
            PickUpAble = false;
            Active = false;
            Collider.enabled = true;
            transform.position = startPos;
            transform.rotation = startRot;
        }

        public override void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}