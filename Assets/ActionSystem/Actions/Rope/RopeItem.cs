using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    public class RopeItem : PickUpAbleElement
    {
        public bool completeHide;
        [SerializeField]
        private List<Collider> ropeNodeFrom = new List<Collider>();
   
        private List<Collider> ropeList = new List<Collider>();
        [SerializeField]
        private UltimateRope rope;
        public bool Used { get; set; }
        private List<float> lengthList = new List<float>();
        public List<Collider> RopeNodeFrom { get { return ropeNodeFrom; } }
        public RopeObj BindingTarget { get; internal set; }


        protected override void Awake()
        {
            base.Awake();
            RegistNodes();
            RegestRopeList();
            ElementController.Instence.RegistElement(this);
        }

        protected virtual void Destroy()
        {
            ElementController.Instence.RemoveElement(this);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            rope.Regenerate(true);
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
        public override void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        public override void StepActive()
        {
            PickUpAble = true;
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
            GetComponent<Collider>().enabled = startState;
            foreach (var item in ropeNodeFrom)
            {
                item.enabled = !startState;
            }
        }

        public override void StepComplete()
        {
            if (completeHide)
            {
                rope.gameObject.SetActive(false);
            }

            if (completeHide)
            {
                Invoke("TryRegenerate", 0.1f);
            }

            Active = false;
            PickUpAble = false;
            OnPlace(false);
        }

        public override void StepUnDo()
        {
            if (completeHide)
            {
                rope.gameObject.SetActive(true);
            }
            Active = false;
            PickUpAble = false;
            OnPlace(true);
        }
        private void TryRegenerate()
        {
            rope.Regenerate(false);
        }
    }

}