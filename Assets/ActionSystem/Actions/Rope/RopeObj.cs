using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    [AddComponentMenu(MenuName.RopeObj)]
    public class RopeObj : ActionObj
    {
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Rope;
            }
        }
        [SerializeField]
        private List<Collider> ropeNodeTo = new List<Collider>();
        [SerializeField]
        private Transform bestRopePos;
        [SerializeField]
        private float triggerDistence;

        private List<Collider> connected = new List<Collider>();
        public bool Connected { get { return connected.Count == 2 * ropeNodeTo.Count; } }

        private Transform angleTemp;
        private Coroutine antoCoroutine;
        private Vector3[] ropeNodeStartPos;
        private RopeItem ropeItem;

        protected void Awake()
        {
            RegistNodes();
        }

        protected override void Start()
        {
            base.Start();
            angleTemp = anglePos;
        }

        private void RegistNodes()
        {
            ropeNodeStartPos = new Vector3[ropeNodeTo.Count];

            for (int i = 0; i < ropeNodeStartPos.Length; i++)
            {
                ropeNodeStartPos[i] = ropeNodeTo[i].transform.position;
                ropeNodeTo[i].gameObject.layer = LayerMask.NameToLayer(Layers.ropePosLayer);
            }
        }

        protected override void OnRegistElement(ISupportElement arg0)
        {
            base.OnRegistElement(arg0);
            if(ropeItem == null && arg0 is RopeItem)
            {
                ropeItem = arg0 as RopeItem;
                if (Started && ropeItem.BindingTarget == null)
                {
                    ropeItem.StepActive();
                    ropeItem.BindingTarget = this;
                }
            }
        }

        internal void TryPlaceRope(RopeItem ropeSelected)
        {
            if (ropeSelected != ropeItem) return;
            var distence = Vector3.Distance(ropeSelected.transform.position, transform.position);
            if(distence < triggerDistence)
            {
                ropeSelected.transform.position = bestRopePos.transform.position;
                ropeSelected.transform.rotation = bestRopePos.transform.rotation;
                ropeSelected.OnPlace();
            }
        }

        protected override void OnRemoveElement(ISupportElement arg0)
        {
            base.OnRemoveElement(arg0);
            ropeItem.BindingTarget = null;
            ropeItem = null;
        }

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            TryFindAnRopeItem();

            if (auto)
            {
                antoCoroutine = StartCoroutine(AutoConnectRopeNodes());
            }
            else
            {
                NoticeOnePickupAbleNode();
            }
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();

            if (antoCoroutine != null)
                StopCoroutine(antoCoroutine);

            PickDownAllCollider();

            connected.Clear();
            anglePos = angleTemp;
            
        }

        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);

            if (antoCoroutine != null)
                StopCoroutine(antoCoroutine);

            QuickInstallRopeNodes(ropeItem.RopeNodeFrom);

            
        }

        private void TryFindAnRopeItem()
        {
            var ropes = elementCtrl.GetElements<RopeItem>(Name);
            if(ropes != null)
            {
                ropeItem = ropes.Find(x => x.BindingTarget == this || x.BindingTarget == null);
            }
            else
            {
                ropeItem = elementCtrl.TryCreateElement<RopeItem>(Name);
            }

            if(ropeItem != null)
            {
                ropeItem.BindingTarget = this;
                ropeItem.StepActive();
            }
        }

        private IEnumerator AutoConnectRopeNodes()
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

            OnEndExecute(false);
        }

        private void NoticeOnePickupAbleNode()
        {
            if (Connected)
            {
                OnEndExecute(false);
            }
            else
            {
                if (ropeItem == null) return;

                for (int i = 0; i <ropeItem.RopeNodeFrom.Count; i++)
                {
                    if (connected.Contains(ropeItem.RopeNodeFrom[i])) continue;

                    var ropeTo = ropeItem.RopeNodeFrom.Find(x => x.name == ropeItem.RopeNodeFrom[i].name && !connected.Contains(x));
                    if (ropeTo != null)
                    {
                        angleCtrl.UnNotice(anglePos);
                        anglePos = ropeItem.RopeNodeFrom[i].transform;
                        if (log) Debug.Log("Notice:" + anglePos);
                        break;
                    }
                }
            }
        }

        private Collider SelectOneRopeNode(out Collider target)
        {
            if (!Connected && ropeItem)
            {
                var ropeNodeFrom = ropeItem.RopeNodeFrom;

                for (int i = 0; i < ropeNodeFrom.Count; i++)
                {
                    var clid = ropeNodeFrom[i];
                    if (!connected.Contains(clid))
                    {
                        target = ropeNodeTo.Find(x => x.name == clid.name && !connected.Contains(x));
                        angleCtrl.UnNotice(anglePos);
                        anglePos = target.transform;
                        //Debug.Log("Notice:" + anglePos);
                        return clid;
                    }
                }
            }
            target = null;
            return null;
        }

        public void PickupCollider(Collider collider)
        {
            if (!ropeItem) return;

            var ropeNodeTo = ropeItem.RopeNodeFrom;

            if (connected.Contains(collider))
            {
                var otherNode = ropeNodeTo.Find(x => x.name == collider.name && connected.Contains(x));
                connected.Remove(collider);
                connected.Remove(otherNode);
            }

            var toCollider = ropeNodeTo.Find(x => x.name == collider.name && !connected.Contains(x));
            if (toCollider != null)
            {
                angleCtrl.UnNotice(anglePos);
                anglePos = toCollider.transform;
            }
        }

        public bool CanInstallCollider(Collider collider)
        {
            bool havePos = false;
            for (int i = 0; i < ropeItem.RopeNodeFrom.Count; i++)
            {
                if (ropeItem.RopeNodeFrom[i].name == collider.name && !connected.Contains(ropeItem.RopeNodeFrom[i]))
                {
                    havePos = true;
                }
            }
            return havePos;
        }

        internal void QuickInstallRopeItem(Collider clid)
        {
            if (!ropeItem) return;

            var ropeNodeTo = ropeItem.RopeNodeFrom;

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
            var id = ropeItem.RopeNodeFrom.IndexOf(collider);
            collider.transform.position = ropeNodeStartPos[id];
            NoticeOnePickupAbleNode();
        }

        public void PickDownAllCollider()
        {
            for (int i = 0; i < ropeItem.RopeNodeFrom.Count; i++)
            {
                ropeItem.RopeNodeFrom[i].transform.position = ropeNodeStartPos[i];
            }
        }

        internal void QuickInstallRopeNodes(List<Collider> ropeNodeFrom)
        {
            if (ropeItem == null) return;

            for (int i = 0; i < ropeNodeFrom.Count; i++)
            {
                var from = ropeNodeFrom[i];
                var obj = ropeItem.RopeNodeFrom.Find(x => x.name == from.name && !connected.Contains(x));

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

      
    }
}