using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    public class RopeElement : PickUpAbleItem
    {
        [SerializeField,Attributes.CustomField("绳子")]
        private UltimateRope rope;
        [SerializeField]
        private List<Collider> ropeNodeFrom = new List<Collider>();

        private List<Collider> ropeList = new List<Collider>();
        private List<float> lengthList = new List<float>();
        public RopeItem bindingTarget { get; set; }
        public override bool OperateAble
        {
            get
            {
                return bindingTarget == null;
            }
        }
        public bool Used { get; set; }
        public List<Collider> RopeNodeFrom { get { return ropeNodeFrom; } }
        public bool completeHide { get; set; }
        private List<UnityAction<RopeElement>> onPlaceActions = new List<UnityAction<RopeElement>>();
        public const string ropeItemLayer = "i:ropeItem";

        protected override void Awake()
        {
            base.Awake();
            RegistNodes();
            RegestRopeList();
            ElementController.Instence.RegistElement(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            rope.Regenerate(true);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ElementController.Instence.RemoveElement(this);
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
            for (int i = 0; i < ropeNodeFrom.Count; i++)
            {
                ropeNodeFrom[i].gameObject.layer = LayerMask.NameToLayer(ropeItemLayer);
            }
        }

        private void RegestRopeList()
        {
            ropeList.Add(rope.RopeStart.GetComponent<Collider>());
            for (int i = 0; i < rope.RopeNodes.Count; i++)
            {
                ropeList.Add(rope.RopeNodes[i].goNode.GetComponent<Collider>());
                lengthList.Add(rope.RopeNodes[i].fLength);
            }
        }
        protected void OnSetPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        public override void SetActive(UnityEngine.Object target)
        {
            base.SetActive(target);
            if(pickUpableFeature.PickUpAble)
            {
                OnPlace(true);
            }
            else
            {
                OnPlace(false);
            }
        }

        public bool TryMoveToPos(Collider collider, Vector3 pos)
        {
            if (rope.RopeNodes.Count == 0) return false;
            var id = ropeList.IndexOf(collider);
            if (id != -1)
            {
                bool canMove = true;
                var lastid = id - 1;
                var nextid = id + 1;
                if (lastid >= 0)
                {
                    var lastNode = ropeList[lastid];
                    canMove &= Vector3.Distance(pos, lastNode.transform.position) < 2 * lengthList[lastid];
                }
                if (nextid < ropeList.Count)
                {
                    var nextNode = ropeList[nextid];
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

        internal void OnPlace(bool startState = false)
        {
            if (PickUpAble)
            {
                pickUpableFeature.collider.enabled = startState;
            }

            foreach (var item in ropeNodeFrom)
            {
                item.enabled = !startState;
            }
        }

        public override void SetInActive(UnityEngine.Object target)
        {
            base.SetInActive(target);
            if (completeHide)
            {
                rope.gameObject.SetActive(false);
            }

            if (completeHide)
            {
                Invoke("TryRegenerate", 0.1f);
            }

            OnPlace(false);
        }

        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            if (completeHide)
            {
                rope.gameObject.SetActive(true);
            }
            OnPlace(true);
        }
        private void TryRegenerate()
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