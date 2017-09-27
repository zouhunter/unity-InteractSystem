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
        public float lineWight = 0.1f;
        public Material lineMaterial;
        [System.Serializable]
        public class PointGroup
        {
            public int p1;
            public int p2;
        }
        public List<PointGroup> connectGroup;
        private List<Collider> nodes = new List<Collider>();
        private Dictionary<int, LineRenderer> lineRenders = new Dictionary<int, LineRenderer>();
        private Dictionary<int, Vector3[]> positionDic = new Dictionary<int, Vector3[]>();
        protected override void Start()
        {
            base.Start();
            RegistNodes();
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            positionDic.Clear();
            ResetLinRenders();
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
            if (nodes.Contains(collider1) && nodes.Contains(collider2))
            {
                var id1 = nodes.IndexOf(collider1);
                var id2 = nodes.IndexOf(collider2);
                if (CanConnect(Mathf.Min(id1, id2), Mathf.Max(id1, id2)))
                {
                    var id = 1 << id1 | 1 << id2;
                    positionDic[id] = positions;
                    RefeshState(id);
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
            if (allConnected)
            {
                OnEndExecute();
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
                RefeshState(id);
                return true;
            }
            else
            {
                return false;
            }

        }
        private void RefeshState(int id)
        {
            if (!positionDic.ContainsKey(id)) return;

            var positionList = positionDic[id];

            var lineRender = GetLineRender(id);
#if UNITY_5_6_OR_NEWER
            lineRender.positionCount = positionList.Count;
#else
            lineRender.SetVertexCount(positionList.Length);
#endif
            lineRender.SetPositions(positionList);
        }
        private bool CanConnect(int min, int max)
        {
            return connectGroup.Find(x =>
            {
                return Mathf.Min(x.p2, x.p1) == min && Mathf.Max(x.p2, x.p1) == max;
            }) != null;
        }
        private LineRenderer GetLineRender(int index)
        {
            if (lineRenders.ContainsKey(index))
            {
                return lineRenders[index];
            }
            else
            {
                var obj = new GameObject(index.ToString());
                obj.transform.SetParent(transform);
                var lineRender = obj.AddComponent<LineRenderer>();
                lineRender.material = lineMaterial;
#if UNITY_5_6_OR_NEWER
                 lineRender.startWidth = lineWight;
                 lineRender.endWidth = lineWight;
                 lineRender.positionCount = 1;
#else
                lineRender.SetWidth(lineWight, lineWight);
                lineRender.SetVertexCount(1);
#endif
                lineRenders.Add(index, lineRender);
                return lineRender;
            }


        }
        private void ResetLinRenders()
        {
            foreach (var lineRender in lineRenders)
            {
#if UNITY_5_6_OR_NEWER
                 lineRender.Value..positionCount = 1;
#else
                lineRender.Value.SetVertexCount(1);
#endif
            }
        }
    }
}