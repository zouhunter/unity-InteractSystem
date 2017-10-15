using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ConnectCtrl:ActionCtroller
    {
        private ConnectObj[] objs;
        public UnityAction<string> onError;
        public UnityAction<Collider> onSelectItem;
        public UnityAction<Collider> onHoverItem;
        private List<Vector3> positons = new List<Vector3>();
        private Ray ray;
        private RaycastHit hit;
        private Collider firstCollider;
        private LineRenderer line;
        private float pointDistence;
        private float hitDistence;
        private Camera objCamera { get; set; }

        public ConnectCtrl(ActionCommand trigger,ConnectObj[] objs, Material lineMaterial, float lineWight,float hitDistence, float pointDistence):base(trigger)
        {
            this.objs = objs;
            this.objCamera = trigger.viewCamera;
            this.hitDistence = hitDistence;
            this.pointDistence = pointDistence;
            var lineRender = trigger. GetComponent<LineRenderer>();
            if (lineRender == null) lineRender = trigger.gameObject.AddComponent<LineRenderer>();
            this.line = lineRender;
            InitConnectObj(lineMaterial, lineWight);
        }

        private void InitConnectObj(Material lineMaterial, float lineWight)
        {
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

        public override IEnumerator Update()
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
            if (Physics.Raycast(ray, out hit, hitDistence, 1 << Setting.connectItemLayer))
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
            if(Input.GetMouseButtonDown(0))
            {
                ClearLineRender();
            }
            else
            {
                ray = objCamera.ScreenPointToRay(Input.mousePosition);
                Vector3 hitPosition = GeometryUtil.LinePlaneIntersect(ray.origin, ray.direction, firstCollider.transform.position, ray.direction);
                if (positons.Count > 0)
                {
                    if (Vector3.Distance(positons[positons.Count - 1], hitPosition) > pointDistence)
                    {
                        positons.Add(hitPosition);
#if UNITY_5_6_OR_NEWER
                    line.positionCount = positons.Count;

#else
                        line.SetVertexCount(positons.Count);
#endif
                        line.SetPositions(positons.ToArray());
                    }
                }
            }
          
        }

        private void TryConnect(Collider collider)
        {
            if (!Input.GetMouseButtonDown(0)) return;
            string element1 = firstCollider.name;
            string element2 = collider.name;
            bool canConnect = false;
            foreach (var item in objs)
            {
                if (item.TryConnectNode(collider, firstCollider))
                {
                    canConnect = true;
                    break;
                }
            }
            ClearLineRender();
            if (!canConnect && onError != null) onError.Invoke(string.Format("{0}和{1}两点不需要连接", element1, element2));
        }

        private void ClearLineRender()
        {
            firstCollider = null;
            positons.Clear();
#if UNITY_5_6_OR_NEWER
            line.positionCount = 1;
#else
            line.SetVertexCount(1);
#endif
        }
        public override void OnEndExecute()
        {
            base.OnEndExecute(); 
            ClearLineRender();
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            ClearLineRender();
        }
    }
}
