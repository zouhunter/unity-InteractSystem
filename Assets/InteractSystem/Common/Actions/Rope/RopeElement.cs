using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
    public class RopeElement : ActionItem
    {
        [SerializeField]
        private List<Collider> ropeNodeFrom = new List<Collider>();
        [SerializeField]
        private UltimateRope rope;

        [SerializeField]
        private ClickAbleFeature clickAbleFeature;

        private List<Collider> ropeList = new List<Collider>();
        private List<float> lengthList = new List<float>();
        private PickUpAbleFeature pickUpFeature;
        public RopeItem bindingTarget;
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
        public List<UnityAction<RopeElement>> onPlaceActions = new List<UnityAction<RopeElement>>();

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

        protected override List<ActionItemFeature> RegistFeatures()
        {
            clickAbleFeature.LayerName = Layers.pickUpElementLayer;
            pickUpFeature = new PickUpAbleFeature(clickAbleFeature.Collider);
            pickUpFeature.target = this;
            pickUpFeature.RegistOnPickStay(OnPickStay);
            return new List<ActionItemFeature>() { pickUpFeature, clickAbleFeature };
        }

        private void OnPickStay()
        {
            if (onPlaceActions.Count > 0)
            {
                foreach (var action in onPlaceActions)
                {
                    action.Invoke(this);
                }
            }
        }

        private void RegistNodes()
        {
            for (int i = 0; i < ropeNodeFrom.Count; i++)
            {
                ropeNodeFrom[i].gameObject.layer = LayerMask.NameToLayer(Layers.ropeNodeLayer);
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

        public override void StepActive()
        {
            base.StepActive();
            Active = true;
            OnPlace(true);
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
            clickAbleFeature.Collider.enabled = startState;
            foreach (var item in ropeNodeFrom)
            {
                item.enabled = !startState;
            }
        }

        public override void StepComplete()
        {
            base.StepComplete();
            if (completeHide)
            {
                rope.gameObject.SetActive(false);
            }

            if (completeHide)
            {
                Invoke("TryRegenerate", 0.1f);
            }

            Active = false;
            OnPlace(false);
        }

        public override void StepUnDo()
        {
            base.StepUnDo();
            if (completeHide)
            {
                rope.gameObject.SetActive(true);
            }
            Active = false;
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
            if (onPlaceActions.Contains(action))
            {
                onPlaceActions.Add(action);
            }
        }
    }

}