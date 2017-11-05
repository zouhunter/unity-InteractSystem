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
        public UltimateRope rope;
        public List<Collider> ropeNode;
        private Vector3[] ropeNodesInstallPos;
        private Vector3[] ropeNodesStartPos;
        private List<Collider> ropeList = new List<Collider>();
        private List<float> lengthList = new List<float>();

        private void Awake()
        {
            RegestRopeList();
            RecordStartPosAndSetLayer();
        }

        private void RecordStartPosAndSetLayer()
        {
            ropeNodesStartPos = new Vector3[ropeNode.Count];
            for (int i = 0; i < ropeNodesStartPos.Length; i++)
            {
                ropeNodesStartPos[i] = ropeNode[i].transform.position;
                ropeNode[i].gameObject.layer = Setting.ropeNodeLayer;
            }
        }

        private void RegestRopeList()
        {
            ropeList.Add(rope.RopeStart.GetComponent<BoxCollider>());
            for (int i = 0; i < rope.RopeNodes.Count; i++)
            {
                ropeList.Add(rope.RopeNodes[i].goNode.GetComponent<BoxCollider>());
                lengthList.Add(rope.RopeNodes[i].fLength);
            }
            
        }

        public void RegistNodesInstallPos()
        {
            Debug.Log("RegistNodesInstallPos");
            ropeNodesInstallPos = new Vector3[ropeNode.Count];
            for (int i = 0; i < ropeNodesInstallPos.Length; i++)
            {
                ropeNodesInstallPos[i] = ropeNode[i].transform.position;
            }
        }
        /// <summary>
        /// 防止线被拉太长
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool TryMoveToPos(Collider collider, Vector3 pos)
        {
            if (rope.RopeNodes.Count == 0) return false;
            var id = ropeList.IndexOf(collider);
            if (id != -1)
            {
                bool canMove = true;
                var lastid = id - 1;
                var nextid = id + 1;
                if(lastid >= 0)
                {
                    var lastNode = ropeList[lastid];
                    canMove  &= Vector3.Distance(pos, lastNode.transform.position) < lengthList[lastid];
                }
                if (nextid < ropeList.Count)
                {
                    var nextNode = ropeList[nextid];
                    canMove &= Vector3.Distance(pos, nextNode.transform.position) < lengthList[id];
                }
                if(canMove)
                {
                    collider.transform.position = pos;
                }
                return canMove;
            }
            return false;
        }
        /// 放回
        /// </summary>
        public void PickDownCollider(Collider collider)
        {
            Debug.Assert(collider != null);
            var id = ropeNode.IndexOf(collider);
            collider.transform.position = ropeNodesInstallPos[id];
        }

        public void PickDownAllCollider()
        {
            for (int i = 0; i < ropeNode.Count; i++)
            {
                ropeNode[i].transform.position = ropeNodesInstallPos[i];
            }
        }

        public void QuickUnInstallAllRopeNode()
        {
            for (int i = 0; i < ropeNode.Count; i++)
            {
                ropeNode[i].transform.position = ropeNodesStartPos[i];
            }
        }

        internal void QuickInstallRopeNodes(List<Collider> ropeNode)
        {
            foreach (var target in ropeNode)
            {
                var obj = this.ropeNode.Find(x => x.name == target.name);
                if(obj != null)
                {
                    obj.transform.position = target.transform.position;
                }
            }
        }
    }
}