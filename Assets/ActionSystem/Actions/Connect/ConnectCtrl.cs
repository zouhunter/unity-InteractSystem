using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ConnectCtrl
    {
        private MonoBehaviour holder;
        private ConnectObj[] objs;
        public UnityAction onComplete;
        public UnityAction<Collider> onSelectItem;
        public UnityAction<Collider> onHoverItem;
        private List<Vector3> positons = new List<Vector3>();
        private Coroutine coroutine;
        private Ray ray;
        private RaycastHit hit;
        private Collider firstCollider;
        private LineRenderer line;
        private float pointDistence;
        private Camera objCamera;

        public ConnectCtrl(MonoBehaviour holder, ConnectObj[] objs, Material lineMaterial, float lineWight, float pointDistence, Camera camera = null)
        {
            this.holder = holder;
            this.objs = objs;
            this.objCamera = camera ?? Camera.main;
            this.pointDistence = pointDistence;
            InitConnectObj(lineMaterial, lineWight);
        }

        private void InitConnectObj(Material lineMaterial, float lineWight)
        {
            line = holder.GetComponent<LineRenderer>();
            if (line == null) line = holder.gameObject.AddComponent<LineRenderer>();
#if UNITY_5_6_OR_NEWER
            line.positionCount = 1;
            line.startWidth = lineWight;
            line.endWidth = lineWight * 0.8f;
#else
            line.SetVertexCount(1);
            line.SetWidth(lineWight,lineWight*0.8f);
#endif
            line.material = lineMaterial;
        }

        internal void StartConnecter()
        {
            if (coroutine != null) coroutine = holder.StartCoroutine(ConnectLoop());
        }

        IEnumerator ConnectLoop()
        {
            while (true)
            {
                if (firstCollider != null)
                {
                    Collider collider;
                    if (TryHitNode(out collider))
                    {
                        if (collider != null && collider != firstCollider)
                        {
                            TryConnect(collider);
                        }
                    }
                    else
                    {
                        UpdateLine();
                    }
                }
                else
                {
                    if (TryHitNode(out firstCollider))
                    {
                        positons.Clear();
                        positons.Add(firstCollider.transform.position);
                    }
                }
                yield return null;
            }
        }

        private bool TryHitNode(out Collider collider)
        {
            ray = objCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 10, 1 << Setting.connectItemLayer))
            {
                if (onHoverItem != null) onHoverItem(hit.collider);
                if (Input.GetMouseButtonDown(0))
                {
                    collider = hit.collider;
                    if (onSelectItem != null) onSelectItem(collider);
                    return true;
                }
            }
            collider = null;
            return false;
        }
        private void UpdateLine()
        {
            ray = objCamera.ScreenPointToRay(Input.mousePosition);
            Vector3 mousePosition = GeometryUtil.LinePlaneIntersect(ray.origin, ray.direction, firstCollider.transform.position, firstCollider.transform.forward);
            if (positons.Count > 0)
            {
                if (Vector3.Distance(positons[positons.Count - 1], mousePosition) > pointDistence)
                {
                    positons.Add(mousePosition);
#if UNITY_5_6_OR_NEWER
                    line.positionCount = positons.Count;

#else
                    line.SetVertexCount(positons.Count);
#endif
                    line.SetPositions(positons.ToArray());
                }
            }
        }

        private void TryConnect(Collider collider)
        {
            if (!positons.Contains(collider.transform.position))
            {
                positons.Add(collider.transform.position);
            }
            foreach (var item in objs)
            {
                if (item.TryConnectNode(collider, firstCollider, positons.ToArray()))
                {
                    firstCollider = null;
                    positons.Clear();
#if UNITY_5_6_OR_NEWER
                    line.positionCount = 1;

#else
                    line.SetVertexCount(1);

#endif
                    break;
                }
            }
        }

        internal void StopConnecter()
        {
            holder.StopCoroutine(coroutine);
            //throw new NotImplementedException();
        }

        internal void UnDoConnectItems()
        {
            //throw new NotImplementedException();
        }
    }
}
