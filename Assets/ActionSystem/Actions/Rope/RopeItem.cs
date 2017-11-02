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
        private List<int> lockId = new List<int>();

        private void Awake()
        {
            ropeNodesStartPos = new Vector3[ropeNode.Count];
            for (int i = 0; i < ropeNodesStartPos.Length; i++)
            {
                ropeNodesStartPos[i] = ropeNode[i].transform.position;
                ropeNode[i].gameObject.layer = Setting.ropeNodeLayer;
            }
        }
        public override void StepComplete()
        {
            base.StepComplete();
            RegistNodesInstallPos();
        }

        private void RegistNodesInstallPos()
        {
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
            if (rope.RopeNodes.Count <= 1) return false;
            var id = rope.RopeNodes.FindIndex(x => x.goNode == collider.gameObject);
            if (id != -1)
            {
                if (id == 0)
                {
                    var node1 = rope.RopeNodes[id + 1];
                    var node0 = rope.RopeNodes[0];
                    var b1 = Vector3.Distance(pos, node1.goNode.transform.position) < node1.fLength;
                    var b2 = Vector3.Distance(pos, rope.RopeStart.transform.position) < node0.fLength;
                    if (b1 && b2)
                    {
                        collider.transform.position = pos;
                        return true;
                    }
                }
                else if (id == rope.RopeNodes.Count - 1)
                {
                    var node1 = rope.RopeNodes[id - 1];
                    if (Vector3.Distance(pos, node1.goNode.transform.position) < node1.fLength)
                    {
                        collider.transform.position = pos;
                        return true;
                    }
                }
                else
                {
                    var node1 = rope.RopeNodes[id + 1];
                    var node0 = rope.RopeNodes[id];
                    var b1 = Vector3.Distance(pos, node1.goNode.transform.position) < node1.fLength;
                    var b2 = Vector3.Distance(pos, node0.goNode.transform.position) < node0.fLength;
                    if (b1 && b2)
                    {
                        collider.transform.position = pos;
                        return true;
                    }
                }
            }
            else if(collider.gameObject == rope.RopeStart)
            {
                var node1 = rope.RopeNodes[0];
                if (Vector3.Distance(pos, node1.goNode.transform.position) < node1.fLength)
                {
                    collider.transform.position = pos;
                    return true;
                }
            }
            return false;
        }
        /// 放回
        /// </summary>
        public void PickDownCollider(Collider collider)
        {
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