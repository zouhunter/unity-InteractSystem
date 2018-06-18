using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace InteractSystem.Common.Actions
{
    public class RopeItem : ActionItem
    {
        public bool Connected { get { return connected.Count == 2 * ropeNodeTo.Count; } }

        public override bool OperateAble
        {
            get
            {
                return targets.Count == 0;
            }
        }

        [SerializeField]
        private List<Collider> ropeNodeTo = new List<Collider>();
        [SerializeField]
        private Transform bestRopePos;
        [SerializeField]
        private float triggerDistence;
        [SerializeField]
        private ClickAbleFeature clickAbleFeature;
        [SerializeField]
        private ContentActionItemFeature contentFeature;

        private List<Collider> connected = new List<Collider>();
        private Transform angleTemp;
        private Coroutine antoCoroutine;
        private Vector3[] ropeNodeStartPos;
        private RopeElement ropeElement;
        private CompleteAbleItemFeature completeFeature;
        private ElementController elementCtrl { get { return ElementController.Instence; } }
        #region UnityAPI 
        protected override void Awake()
        {
            base.Awake();
            RegistNodes();
        }

        protected override void Start()
        {
            base.Start();
            //angleTemp = anglePos;
            elementCtrl.onRegistElememt += OnRegistElement;
            //elementCtrl.onRemoveElememt += OnRemoveElement;
        }
        #endregion


        #region API

        public void PickupCollider(Collider collider)
        {
            if (!ropeElement) return;

            if (connected.Contains(collider))
            {
                var otherNode = ropeNodeTo.Find(x => x.name == collider.name && connected.Contains(x));
                connected.Remove(collider);
                connected.Remove(otherNode);
            }

            var toCollider = ropeNodeTo.Find(x => x.name == collider.name && !connected.Contains(x));
            if (toCollider != null)
            {
                //angleCtrl.UnNotice(anglePos);
                //anglePos = toCollider.transform;
            }
        }

        public bool CanInstallCollider(Collider collider)
        {
            bool havePos = false;
            for (int i = 0; i < ropeElement.RopeNodeFrom.Count; i++)
            {
                if (ropeElement.RopeNodeFrom[i].name == collider.name && !connected.Contains(ropeElement.RopeNodeFrom[i]))
                {
                    havePos = true;
                }
            }
            return havePos;
        }

        internal void QuickInstallRopeItem(Collider clid)
        {
            if (!ropeElement) return;

            var nodeTo = ropeNodeTo.Find(x => x.name == clid.name && !connected.Contains(x));
            if (nodeTo)
            {
                connected.Add(nodeTo);
                connected.Add(clid);
                clid.transform.position = nodeTo.transform.position;
            }
            NoticeOnePickupAbleNode();
        }

        public void PickDownCollider(Collider collider)
        {
            Debug.Assert(collider != null);
            var id = ropeElement.RopeNodeFrom.IndexOf(collider);
            collider.transform.position = ropeNodeStartPos[id];
            NoticeOnePickupAbleNode();
        }

        public void PickDownAllCollider()
        {
            for (int i = 0; i < ropeElement.RopeNodeFrom.Count; i++)
            {
                ropeElement.RopeNodeFrom[i].transform.position = ropeNodeStartPos[i];
            }
        }

        internal void QuickInstallRopeNodes(List<Collider> ropeNodeFrom)
        {
            if (ropeElement == null) return;

            for (int i = 0; i < ropeNodeFrom.Count; i++)
            {
                var from = ropeNodeFrom[i];
                var obj = ropeElement.RopeNodeFrom.Find(x => x.name == from.name && !connected.Contains(x));

                if (obj != null)
                {
                    Debug.Log("form:" + from.transform.position);
                    Debug.Log("to:" + obj.transform.position);

                    from.transform.position = obj.transform.position;
                    connected.Add(obj);
                    connected.Add(from);
                }
            }

        }

        /// <summary>
        /// 尝试放置绳子
        /// </summary>
        /// <param name="ropeSelected"></param>
        public void TryPlaceRope(RopeElement ropeSelected)
        {
            if (ropeSelected != ropeElement) return;
            var distence = Vector3.Distance(ropeSelected.transform.position, transform.position);
            if (distence < triggerDistence)
            {
                ropeSelected.transform.position = bestRopePos.transform.position;
                ropeSelected.transform.rotation = bestRopePos.transform.rotation;
                ropeSelected.OnPlace();
            }
        }
        #endregion

        #region Override
        protected override List<ActionItemFeature> RegistFeatures()
        {
            //可结束
            completeFeature = new CompleteAbleItemFeature();
            completeFeature.target = this;
            completeFeature.onAutoExecute = (graph) =>{
                StartCoroutine(AutoConnectRopeNodes(completeFeature.OnComplete));
            };
            //可点击
            clickAbleFeature.LayerName = Layers.pickUpElementLayer;
            clickAbleFeature.target = this;

            //子元素
            contentFeature.target = this;
            contentFeature.type = typeof(RopeElement);
            return new List<ActionItemFeature>() { completeFeature, clickAbleFeature, contentFeature };
        }
        /// <summary>
        /// 试图绑定绳子
        /// </summary>
        /// <param name="arg0"></param>
        protected void OnRegistElement(ISupportElement arg0)
        {
            if (ropeElement == null && arg0 is RopeElement)
            {
                ropeElement = arg0 as RopeElement;
                if (Active && ropeElement.OperateAble)
                {
                    ropeElement.StepActive();
                    ropeElement.RegistOnPlace(TryPlaceRope);
                }
            }
        }

        /// <summary>
        /// 试图解除绑定
        /// </summary>
        /// <param name="arg0"></param>
        //protected override void OnRemoveElement(ISupportElement arg0)
        //{
        //    if(ropeItem)
        //    {
        //        ropeItem.BindingTarget = null;
        //        ropeItem = null;
        //    }
        //}

        //public override void OnStartExecute(bool auto = false)
        //{
        //    base.OnStartExecute(auto);
        //    TryFindAnRopeItem();

        //    if (auto)
        //    {
        //        antoCoroutine = StartCoroutine(AutoConnectRopeNodes());
        //    }
        //    else
        //    {
        //        NoticeOnePickupAbleNode();
        //    }
        //}

        //public override void OnUnDoExecute()
        //{
        //    base.OnUnDoExecute();

        //    if (antoCoroutine != null)
        //        StopCoroutine(antoCoroutine);

        //    PickDownAllCollider();

        //    connected.Clear();
        //    anglePos = angleTemp;

        //}

        //protected override void OnBeforeEnd(bool force)
        //{
        //    base.OnBeforeEnd(force);

        //    if (antoCoroutine != null)
        //        StopCoroutine(antoCoroutine);

        //    QuickInstallRopeNodes(ropeItem.RopeNodeFrom);


        //}
        #endregion

        #region Private
        /// <summary>
        /// 找到或创建绳索
        /// </summary>
        private void TryFindAnRopeItem()
        {
            var ropes = ElementController.Instence.GetElements<RopeElement>(Name,true);
            if (ropes != null)
            {
                ropeElement = ropes.Find(x => x.bindingTarget == this || x.bindingTarget == null);
            }

            if (ropeElement != null)
            {
                ropeElement.bindingTarget = this;
                ropeElement.StepActive();
            }
        }
        /// <summary>
        /// 注册所有安装点的layer
        /// 并记录其初始坐标
        /// </summary>
        private void RegistNodes()
        {
            ropeNodeStartPos = new Vector3[ropeNodeTo.Count];

            for (int i = 0; i < ropeNodeStartPos.Length; i++)
            {
                ropeNodeStartPos[i] = ropeNodeTo[i].transform.position;
                ropeNodeTo[i].gameObject.layer = LayerMask.NameToLayer(Layers.ropePosLayer);
            }
        }
        /// <summary>
        /// 自动进行连接
        /// </summary>
        /// <returns></returns>
        public IEnumerator AutoConnectRopeNodes(UnityAction onComplete)
        {
            Collider current;
            Collider currentTarget;
            while ((current = SelectOneRopeNode(out currentTarget)) != null)
            {
                var startPos = current.transform.position;

                for (float i = 0; i < 1f; i += Time.deltaTime)
                {
                    current.transform.position = Vector3.Lerp(startPos, currentTarget.transform.position, i);
                    yield return null;
                }

                connected.Add(currentTarget);
                connected.Add(current);
            }

            onComplete();
        }
        
        /// <summary>
        /// 通知一个可以被拿起的绳子节点
        /// </summary>
        private void NoticeOnePickupAbleNode()
        {
            if (Connected)
            {
                completeFeature.OnComplete();
                //OnComplete();
            }
            else
            {
                if (ropeElement == null) return;

                for (int i = 0; i < ropeElement.RopeNodeFrom.Count; i++)
                {
                    if (connected.Contains(ropeElement.RopeNodeFrom[i])) continue;

                    var ropeTo = ropeElement.RopeNodeFrom.Find(x => x.name == ropeElement.RopeNodeFrom[i].name && !connected.Contains(x));
                    if (ropeTo != null)
                    {
                        //angleCtrl.UnNotice(anglePos);
                        //anglePos = ropeItem.RopeNodeFrom[i].transform;
                        //if (log) Debug.Log("Notice:" + anglePos);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 选择一个节点
        /// 并返回目标节点
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private Collider SelectOneRopeNode(out Collider target)
        {
            if (!Connected && ropeElement)
            {
                var ropeNodeFrom = ropeElement.RopeNodeFrom;

                for (int i = 0; i < ropeNodeFrom.Count; i++)
                {
                    var clid = ropeNodeFrom[i];
                    if (!connected.Contains(clid))
                    {
                        target = ropeNodeTo.Find(x => x.name == clid.name && !connected.Contains(x));
                        //angleCtrl.UnNotice(anglePos);
                        //anglePos = target.transform;
                        //Debug.Log("Notice:" + anglePos);
                        return clid;
                    }
                }
            }
            target = null;
            return null;
        }

        #endregion

    }
}