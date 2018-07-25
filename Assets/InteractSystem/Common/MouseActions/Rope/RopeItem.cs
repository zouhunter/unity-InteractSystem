using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace InteractSystem.Actions
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

      

        [SerializeField,Attributes.CustomField("绳子坐标")]
        private Transform bestRopePos;
        [SerializeField, Attributes.CustomField("默认绳子")]
        private RopeElement defultRope;
        [SerializeField,Attributes.CustomField("接收距离")]
        private float triggerDistence;
        [SerializeField]
        private ContentActionItemFeature contentFeature = new ContentActionItemFeature(typeof(RopeElement));
        private CompleteAbleItemFeature completeFeature = new CompleteAbleItemFeature();
        [SerializeField]
        private List<Collider> ropeNodeTo = new List<Collider>();

        private List<Collider> connected = new List<Collider>();
        private Vector3[] ropeNodeStartPos;
        private RopeElement ropeElement { get { return contentFeature.Element as RopeElement == null ? defultRope: contentFeature.Element as RopeElement; } }
        private ElementController elementCtrl { get { return ElementController.Instence; } }
        public const string layer = "i:ropepos";

        #region UnityAPI 
        protected override void Awake()
        {
            base.Awake();
            RegistNodes();
            if (defultRope != null)
                defultRope.bindingTarget = this;
        }

        protected override void Start()
        {
            base.Start();
            elementCtrl.onRegistElememt += OnRegistElement;
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
            if (ropeSelected == null) return;
            //if (ropeElement != null) return;
            if (!ropeSelected.OperateAble) return;

            var distence = Vector3.Distance(ropeSelected.transform.position, transform.position);
            if (distence < triggerDistence)
            {
                ropeSelected.bindingTarget = this;
                contentFeature.Element = ropeSelected;

                ropeSelected.transform.position = bestRopePos.transform.position;
                ropeSelected.transform.rotation = bestRopePos.transform.rotation;
                ropeSelected.OnPlace();
            }
        }
        #endregion

        #region Override
        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            //可结束
            completeFeature.Init(this, (graph) =>
            {
                StartCoroutine(AutoConnectRopeNodes(completeFeature.OnComplete));
            });
            features.Add(completeFeature);

            //可点击
            //clickAbleFeature.Init(this,PickUpAbleItem.layer);
            //features.Add(clickAbleFeature);

            //子元素
            contentFeature.Init(this);
            features.Add(contentFeature);

            return features;
        }

        /// <summary>
        /// 试图绑定绳子
        /// </summary>
        /// <param name="arg0"></param>
        protected void OnRegistElement(ISupportElement arg0)
        {
            if (ropeElement == null && arg0 is RopeElement)
            {
                var element = arg0 as RopeElement;
                if (Active && element.OperateAble)
                {
                    element.SetActive(this);
                    element.RegistOnPlace(TryPlaceRope);
                }
            }
        }

        public override void SetActive(UnityEngine.Object target)
        {
            base.SetActive(target);
            TryFindAnRopeItems();
        }

        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            PickDownAllCollider();
            connected.Clear();
        }
        public override void SetInActive(UnityEngine.Object target)
        {
            base.SetInActive(target);
            if(ropeElement == null)
            {
                QuickPlaceRope();
            }
            QuickInstallRopeNodes(ropeElement.RopeNodeFrom);
        }

        private void QuickPlaceRope()
        {
        }
        #endregion

        #region Private
        /// <summary>
        /// 找到或创建绳索
        /// </summary>
        private void TryFindAnRopeItems()
        {
            var ropes = ElementController.Instence.GetElements<RopeElement>(contentFeature.ElementName,true);
            if (ropes != null)
            {
                var ropeElements = ropes.FindAll(x => x.bindingTarget == this || x.bindingTarget == null);
                foreach (var rope in ropeElements)
                {
                    if (rope != null)
                    {
                        if(!rope.Active)
                            rope.SetActive(this);

                        rope.RegistOnPlace(TryPlaceRope);
                    }
                }
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
                ropeNodeTo[i].gameObject.layer = LayerMask.NameToLayer(layer);
            }
        }
        /// <summary>
        /// 自动进行连接
        /// </summary>
        /// <returns></returns>
        public IEnumerator AutoConnectRopeNodes(UnityAction onComplete)
        {
            Collider currentTarget;
            Collider current = SelectOneRopeNode(out currentTarget);

            while (current != null)
            {
                var startPos = current.transform.position;

                for (float i = 0; i < 1f; i += Time.deltaTime)
                {
                    current.transform.position = Vector3.Lerp(startPos, currentTarget.transform.position, i);
                    yield return null;
                }

                connected.Add(currentTarget);
                connected.Add(current);

                current = SelectOneRopeNode(out currentTarget);
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