using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace InteractSystem.Actions
{
    public class RopeElement : PickUpAbleItem
    {
        [SerializeField,Attributes.CustomField("绳子")]
        private UltimateRope rope;
        [SerializeField]
        private List<Collider> ropeNodeFrom = new List<Collider>();
        private List<Collider> ropecolliderListInternal = new List<Collider>();//用于重新生成绳子 
        private List<float> lengthList = new List<float>();
        public RopeItem BindingTarget { get { if (targets.Count == 0) return null; return targets[0] as RopeItem; } }
        public override bool OperateAble
        {
            get
            {
                return BindingTarget == null;
            }
        }
        public bool Used { get; set; }
        public List<Collider> RopeNodeFrom { get { return ropeNodeFrom; } }
        private List<UnityAction<RopeElement>> onPlaceActions = new List<UnityAction<RopeElement>>();
        public const string ropeItemLayer = "i:ropeItem";
        private Vector3[] ropeNodeStartPos;
        private Vector3[] oringalStartPos;

        protected override void Start()
        {
            base.Start();
            RegistNodes();
            RegestRopeList();
        }

        protected override void RegistPickupableEvents()
        {
            pickUpableFeature.RegistOnPickStay(OnPickStay);
            pickUpableFeature.RegistOnSetPosition(OnSetPosition);
        }
        
        protected void OnPickStay()
        {
            if (onPlaceActions.Count > 0)
            {
                foreach (var action in onPlaceActions)
                {
                    action.Invoke(this);
                }
            }
            else
            {
                Debug.LogError("have no onPlace Action!",this);
            }

        }

        private void RegistNodes()
        {
            oringalStartPos = new Vector3[ropeNodeFrom.Count];
            for (int i = 0; i < ropeNodeFrom.Count; i++)
            {
                ropeNodeFrom[i].gameObject.layer = LayerMask.NameToLayer(ropeItemLayer);
                oringalStartPos[i] = ropeNodeFrom[i].transform.position;
            }
        }

        private void RegestRopeList()
        {
            ropecolliderListInternal.Add(rope.RopeStart.GetComponent<Collider>());
            for (int i = 0; i < rope.RopeNodes.Count; i++)
            {
                ropecolliderListInternal.Add(rope.RopeNodes[i].goNode.GetComponent<Collider>());
                lengthList.Add(rope.RopeNodes[i].fLength);
            }
        }

        internal void UnNoticeAll()
        {
            for (int i = 0; i < ropeNodeFrom.Count; i++)
            {
                UnNotice(ropeNodeFrom[i].transform);
            }
        }

        protected void OnSetPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        protected override void OnSetActive(UnityEngine.Object target)
        {
            base.OnSetActive(target);
            if (pickUpableFeature.PickUpAble)
            {
                OnPlaceRopeElement(false);
            }
            else
            {
                OnPlaceRopeElement(true);
            }
        }

        public bool TryMoveToPos(Collider collider, Vector3 pos)
        {
            if (rope.RopeNodes.Count == 0) return false;
            var id = ropecolliderListInternal.IndexOf(collider);
            if (id != -1)
            {
                bool canMove = true;
                var lastid = id - 1;
                var nextid = id + 1;
                if (lastid >= 0)
                {
                    var lastNode = ropecolliderListInternal[lastid];
                    canMove &= Vector3.Distance(pos, lastNode.transform.position) < 2 * lengthList[lastid];
                }
                if (nextid < ropecolliderListInternal.Count)
                {
                    var nextNode = ropecolliderListInternal[nextid];
                    canMove &= Vector3.Distance(pos, nextNode.transform.position) < 2 * lengthList[id];
                }
                if (canMove)
                {
                    collider.transform.position = pos;
                }
                return canMove;
            }
            return false;
        }

        protected void OnPlaceRopeElement(bool defultState = false)
        {
            ropeNodeStartPos = new Vector3[ropecolliderListInternal.Count];
            for (int i = 0; i < ropeNodeStartPos.Length; i++)
            {
                ropeNodeStartPos[i] = RopeNodeFrom[i].transform.position;
            }

            if (PickUpAble)
            {
                pickUpableFeature.collider.enabled = !defultState;
            }

            foreach (var item in ropeNodeFrom)
            {
                item.enabled = defultState;
            }
        }


        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            ResetStartPos();
        }

        protected void ResetStartPos()
        {
            for (int i = 0; i < ropeNodeFrom.Count; i++)
            {
                ropeNodeFrom[i].transform.position = oringalStartPos[i];
            }
            rope.Regenerate(false);
        }

        public void RegenerateRope()
        {
            rope.Regenerate(false);
        }
        public override void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void RegistOnPlace(UnityAction<RopeElement> action)
        {
            if (!onPlaceActions.Contains(action))
            {
                onPlaceActions.Add(action);
            }
        }

       

    }

}