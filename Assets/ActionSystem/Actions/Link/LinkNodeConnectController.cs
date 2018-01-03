using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class LinkNodeConnectController
    {
        public event UnityAction<LinkPort[]> onConnected;
        public event UnityAction<LinkPort> onMatch;
        public event UnityAction<LinkPort> onDisMatch;
        public event UnityAction<LinkPort[]> onDisconnected;
        public Dictionary<LinkItem, List<LinkPort>> ConnectedDic { get { return connectedNodes; } }

        private float timer;
        private const float spanTime = 0.5f;
        private LinkItem pickedUpItem;
        private LinkPort activeNode;
        private LinkPort targetNode;
        private Dictionary<LinkItem, List<LinkPort>> connectedNodes = new Dictionary<LinkItem, List<LinkPort>>();


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
                else {
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
            List<LinkPort> olditems = new List<LinkPort>();
            if (connectedNodes.ContainsKey(item))
            {
                List<LinkPort> needClear = new List<LinkPort>();
                for (int i = 0; i < connectedNodes[item].Count; i++)
                {
                    LinkPort nodeItem = connectedNodes[item][i];
                    needClear.Add(nodeItem);

                    LinkPort target = nodeItem.ConnectedNode;
                    connectedNodes[target.Body].Remove(target);
                    Debug.Log(connectedNodes[item][i].Detach());

                    olditems.Add(nodeItem);
                    olditems.Add(target);
                }

                for (int i = 0; i < needClear.Count; i++)
                {
                    connectedNodes[item].Remove(needClear[i]);
                }
                if (onDisconnected != null) onDisconnected.Invoke(needClear.ToArray());
            }
        }

        public void SetDisableItem(LinkItem item)
        {
            pickedUpItem = null;
            targetNode = null;
            activeNode = null;
        }

        public void TryConnect()
        {
            if (activeNode != null && activeNode != null)
            {
                if (targetNode.Attach(activeNode))
                {
                    activeNode.ResetTransform();

                    if (!connectedNodes.ContainsKey(pickedUpItem))
                    {
                        connectedNodes[pickedUpItem] = new List<LinkPort>();
                    }

                    connectedNodes[pickedUpItem].Add(activeNode);

                    if (!connectedNodes.ContainsKey(targetNode.Body))
                    {
                        connectedNodes[targetNode.Body] = new List<LinkPort>();
                    }

                    connectedNodes[targetNode.Body].Add(targetNode);

                    if (onConnected != null) onConnected.Invoke(new LinkPort[] { activeNode, targetNode });
                    activeNode = null;
                    targetNode = null;
                }
            }
        }
    }
}