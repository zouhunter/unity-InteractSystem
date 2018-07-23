using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem.Graph;

namespace InteractSystem.Actions
{
    public class ConnectCtrl : PCOperateCtrl<ConnectCtrl>, IUpdateAble
    {
        public UnityAction<string> onError { get; set; }
        public UnityAction<ConnectItem> onSelectItem { get; set; }
        public UnityAction<ConnectItem> onHoverItem { get; set; }

        private List<Vector3> positons = new List<Vector3>();
        private Ray ray;
        private RaycastHit hit;
        private ConnectItem connectObj;
        private ConnectItem firstItem;
        private LineRenderer line;

        private Material lineMaterial { get { if (currentMat != null) return currentMat; return Config.Instence.lineMaterial; } }
        private float lineWight { get { if (currentLineWidth > 0) return currentLineWidth; return Config.Instence.lineWidth; } }

        private GameObject lineHolder;
        private float hitDistence { get { return Config.Instence.hitDistence; } }
        private Material currentMat;
        private float currentLineWidth;
        private List<Transform> noticed = new List<Transform>();

        public ConnectCtrl()
        {
            InitConnectViewer();
        }

        private void InitConnectViewer()
        {
            lineHolder = new GameObject("lineHolder");
            lineHolder.hideFlags = HideFlags.HideInHierarchy;
            this.line = lineHolder.AddComponent<LineRenderer>();
            line.positionCount = 1;
            ConnectUtil.UpdateLineStyle(line,lineWight,lineMaterial);
        }

        public void Update()
        {
            if (firstItem != null)
            {
                ConnectItem collider;
                if (TryHitNode(out collider))
                {
                    if (collider != null && collider != firstItem)
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
                if (TryHitNode(out firstItem))
                {
                    WorpLineInfo(firstItem.Name);
                    positons.Clear();
                    positons.Add(firstItem.transform.position);
                    ClearNoticePoss(firstItem);
                    NoticeTargetPoss();
                }
            }
        }

        private void NoticeTargetPoss()
        {
            foreach (ConnectNode node in lockList)
            {
                var groups = Array.FindAll(node.connectGroup, x => x.p1 == firstItem.Name || x.p2 == firstItem.Name);
                if (groups != null)
                {
                    foreach (var group in groups)
                    {
                        var nameA = group.p1;
                        var nameB = group.p2;
                        var targetName = nameA == firstItem.Name ? nameB : nameA;
                        NoticeTargetPos(targetName);
                    }
                }
            }
        }

        private void NoticeTargetPos(string connectItemName)
        {
            var elements = ElementController.Instence.GetElements<ConnectItem>(connectItemName, true);
            Debug.Assert(elements != null);
            foreach (var item in elements)
            {
                if(!ConnectUtil.HaveConnected(item,firstItem))
                {
                    firstItem.Notice(item.transform);

                    if(!noticed.Contains(item.transform))
                        noticed.Add(item.transform);
                }
               
            }
        }

        private void ClearNoticePoss(ConnectItem content)
        {
            foreach (var item in noticed)
            {
                content.UnNotice(item);
            }
        }

        private bool TryHitNode(out ConnectItem connectItem)
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, hitDistence, LayerMask.GetMask(ConnectItem.layer)))
            {
                var hited = hit.collider.GetComponentInParent<ConnectItem>();
                if (hited == null)
                {
                    connectItem = null;
                    return false;
                }

                if (onHoverItem != null)
                {
                    onHoverItem(hited);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    connectItem = hited;
                    if (onSelectItem != null)
                        onSelectItem(connectItem);
                    return true;
                }
            }
            connectItem = null;
            return false;
        }


        private void WorpLineInfo(string itemA)
        {
            currentMat = null;
            currentLineWidth = 0;
            foreach (ConnectNode node in lockList)
            {
                var id = Array.FindIndex(node.elements,x=>x== itemA);
                if (id >=0)
                {
                    if(node.lineMaterial)
                    {
                        currentMat = node.lineMaterial;
                    }

                    if(node.lineWight > 0)
                    {
                        currentLineWidth = node.lineWight;
                    }
                }
            }
            ConnectUtil.UpdateLineStyle(line, lineWight, lineMaterial);
        }

        private ConnectNode SuarchOneNode(string itemA, string itemB, out ConnectNode.PointGroup groupInfo)
        {
            groupInfo = null;
            foreach (ConnectNode node in lockList)
            {
                groupInfo = node.GetConnectInfo(itemA, itemB);
                if(groupInfo != null)
                {
                    return node;
                }
            }
            return null;
        }

        private void UpdateLine()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ClearStarteds();
            }
            else
            {
                ray = viewCamera.ScreenPointToRay(Input.mousePosition);
                Vector3 hitPosition = GeometryUtil.LinePlaneIntersect(ray.origin, ray.direction, firstItem.transform.position, ray.direction);

                if (positons.Count > 1)
                {
                    positons[1] = hitPosition;
                }
                else
                {
                    positons.Add(hitPosition);
                }
#if UNITY_5_6_OR_NEWER
                line.positionCount = positons.Count;

#else
                line.SetVertexCount(positons.Count);
#endif
                line.SetPositions(positons.ToArray());
            }

        }

        private void TryConnect(ConnectItem otherItem)
        {
            if (!Input.GetMouseButtonDown(0)) return;
            ConnectItem element1 = firstItem;
            ConnectItem element2 = otherItem;
            bool canConnect = false;

            ConnectNode.PointGroup groupInfo = null;
            var node = SuarchOneNode(element1.Name, element2.Name, out groupInfo);
            if(node != null){
                canConnect = ConnectUtil.TryConnect(element1, element2, groupInfo);
                node.TryComplete();
            }
            ClearStarteds();
         
            if (!canConnect && onError != null) onError.Invoke(string.Format("{0}和{1}两点不需要连接", element1, element2));
        }

        private void ClearStarteds()
        {
            ClearNoticePoss(firstItem);
            firstItem = null;
            positons.Clear();
#if UNITY_5_6_OR_NEWER
            line.positionCount = 1;
#else
            line.SetVertexCount(1);
#endif
        }
    }
}
