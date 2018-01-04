using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class LinkConnectController
    {
        public UnityAction<LinkPort[]> onConnected { get; set; }
        public UnityAction<LinkPort> onMatch { get; set; }
        public UnityAction<LinkPort> onDisMatch { get; set; }
        public UnityAction<LinkPort[]> onDisconnected { get; set; }
        private Dictionary<LinkItem, List<LinkPort>> ConnectedDic { get; set; }
        private Transform Parent { get; set; }
        private float timer;
        private const float spanTime = 0.5f;
        private LinkItem pickedUpItem;
        private LinkPort activeNode;
        private LinkPort targetNode;

        public void Update()
        {
            timer += Time.deltaTime;
            if (pickedUpItem != null && timer > spanTime)
            {
                timer = 0f;

                if (FindConnectableObject())
                {
                    if (onMatch != null)
                    {
                        onMatch(activeNode);
                        onMatch(targetNode);
                    }
                }
                else
                {
                    if (targetNode != null)
                    {
                        onDisMatch.Invoke(targetNode);
                    }
                    if (activeNode != null)
                    {
                        onDisMatch.Invoke(activeNode);
                    }
                    activeNode = null;
                    targetNode = null;
                }
            }
        }

        public bool FindConnectableObject()
        {
            if (pickedUpItem != null)
            {
                LinkPort tempNode;
                foreach (var item in pickedUpItem.ChildNodes)
                {
                    if (FindInstallableNode(item, out tempNode))
                    {
                        activeNode = item;
                        targetNode = tempNode;
                        return true;
                    }
                }
            }

            return false;
        }
        private bool FindInstallableNode(LinkPort item, out LinkPort node)
        {
            if (item.ConnectedNode != null)
            {
                node = item.ConnectedNode;
                return false;
            }

            Collider[] colliders = Physics.OverlapSphere(item.Pos, item.Range, 1 << Layers.nodeLayer);
            if (colliders != null && colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    LinkPort tempNode = collider.GetComponentInParent<LinkPort>();
                    if (tempNode == null)
                    {
                        //Debug.Log(collider + " have no iportItem");
                        continue;
                    }
                    //主被动动连接点，非自身点，相同名，没有建立连接
                    if (tempNode.Body != item.Body && tempNode.ConnectedNode == null)
                    {
                        if (tempNode.connectAble.Find((x) => x.itemName == item.Body.Name && x.nodeId == item.NodeID) != null)
                        {
                            node = tempNode;
                            return true;
                        }
                    }
                }
            }
            node = null;
            return false;
        }

        public void SetActiveItem(LinkItem item)
        {
            this.pickedUpItem = item;

            List<LinkPort> disconnected = new List<LinkPort>();

            if (ConnectedDic.ContainsKey(item))
            {
                LinkPort[] connectedPort = ConnectedDic[item].ToArray();
                for (int i = 0; i < connectedPort.Length; i++)
                {
                    LinkPort port = ConnectedDic[item][i];
                    LinkPort otherPort = port.ConnectedNode;

                    if (otherPort.Body.transform.IsChildOf(item.transform))
                    {
                        continue;//子对象不用清除
                    }
                    else
                    {
                        ConnectedDic[item].Remove(port);
                        ConnectedDic[otherPort.Body].Remove(otherPort);
                        LinkUtil.DetachNodes(port, otherPort, Parent);
                        disconnected.Add(port);
                        disconnected.Add(otherPort);
                    }
                }

                if (onDisconnected != null)
                    onDisconnected.Invoke(disconnected.ToArray());
            }
        }

        public void SetState(Transform parent, Dictionary<LinkItem, List<LinkPort>> ConnectedDic)
        {
            this.ConnectedDic = ConnectedDic;
            this.Parent = parent;
        }

        public void SetDisableItem(LinkItem item)
        {
            pickedUpItem = null;
            targetNode = null;
            activeNode = null;
        }

        public void TryConnect()
        {
            if (activeNode != null && targetNode != null && !targetNode.Body.transform.IsChildOf(activeNode.Body.transform))
            {
                LinkUtil.RecordToDic(ConnectedDic,activeNode);

                LinkUtil.RecordToDic(ConnectedDic, targetNode);

                LinkUtil.AttachNodes(activeNode, targetNode);

                if (onConnected != null)
                    onConnected.Invoke(new LinkPort[] { activeNode, targetNode });

                activeNode = null;
                targetNode = null;
            }
        }

       

    }
}