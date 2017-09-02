using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class ConnectObj : QueueIDObj, ISortAble
    {
        [System.Serializable]
        public class PointGroup
        {
            public int p1;
            public int p2;
        }
        public List<PointGroup> connectGroup;
        private List<Collider> nodes = new List<Collider>();
        private LineRenderer lineRender;
        private Dictionary<int, Vector3[]> positionDic = new Dictionary<int, Vector3[]>();
        protected override void Start(){
            base.Start();
            RegistNodes();
            lineRender = GetComponent<LineRenderer>();
        }

        public override void StartExecute(bool forceAuto = false)
        {
            base.StartExecute(forceAuto);
            if (forceAuto)
            {
                foreach (var item in connectGroup)
                {
                    var id1 = item.p1;
                    var id2 = item.p2;
                    Vector3[] positions = new Vector3[2];
                    positions[0] = nodes[id1].transform.position;
                    positions[1] = nodes[id2].transform.position;
                    if (id1 > id2) Array.Reverse(positions);
                    positionDic[1 << id1 | 1 << id2] = positions;
                }
                RefeshState();
            }
        }

        public override void UnDoExecute()
        {
            base.UnDoExecute();
            positionDic.Clear();
            RefeshState();
        }
        private void RegistNodes()
        {
            foreach (Transform child in transform)
            {
                var collider = child.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.gameObject.layer = Setting.connectItemLayer;
                    nodes.Add(collider);
                }
            }
        }
        public bool TryConnectNode(Collider collider1, Collider collider2, Vector3[] positions)
        {
            if(nodes.Contains(collider1) && nodes.Contains(collider2))
            {
                var id1 = nodes.IndexOf(collider1);
                var id2 = nodes.IndexOf(collider2);
                if (CanConnect(Mathf.Min(id1,id2), Mathf.Max(id1, id2)))
                {
                    if (id1 > id2) Array.Reverse(positions);
                    positionDic[1 << id1 | 1 << id2] = positions;
                    RefeshState();
                    OnOneNodeConnected();
                    return true;
                }
            }
            return false;
        }

        private void OnOneNodeConnected()
        {
            bool allConnected = true;
            foreach (var item in connectGroup)
            {
                var key = 1 << item.p1 | 1 << item.p2;
                allConnected &= positionDic.ContainsKey(key);
            }
            if (allConnected){
                EndExecute();
            }
        }

        public bool DeleteConnect(Collider collider1, Collider collider2)
        {
            if (nodes.Contains(collider1) && nodes.Contains(collider2))
            {
                var id1 = nodes.IndexOf(collider1);
                var id2 = nodes.IndexOf(collider2);
                var id = 1 << id1 | 1 << id2;
                if (positionDic.ContainsKey(id)) positionDic.Remove(id);
                RefeshState();
                return true;
            }
            else
            {
                return false;
            }
          
        }
        private void RefeshState()
        {
            var positionList = new List<Vector3>();
            foreach (var item in positionDic)
            {
                foreach (var pos in item.Value)
                {
                    positionList.Add(pos);
                }
            }
            lineRender.SetVertexCount(positionList.Count);
            lineRender.SetPositions(positionList.ToArray());
        }
        private bool CanConnect(int min,int max)
        {
            return connectGroup.Find(x => {
                return Mathf.Min(x.p2, x.p1) == min && Mathf.Max(x.p2, x.p1) == max;
            }) != null;
        }

    }
}