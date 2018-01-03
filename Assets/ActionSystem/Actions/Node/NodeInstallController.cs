using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class NodeConnectController
    {
        public event UnityAction<PortItemBehaiver[]> onConnected;
        public event UnityAction<PortItemBehaiver> onMatch;
        public event UnityAction<PortItemBehaiver> onDisMatch;
        public event UnityAction<PortItemBehaiver[]> onDisconnected;
        public Dictionary<PortParentBehaiver, List<PortItemBehaiver>> ConnectedDic { get { return connectedNodes; } }

        private float timeSpan;
        private float spanTime;
        private float sphereRange = 0.0001f;
        private PortParentBehaiver pickedUpItem;
        private PortItemBehaiver activeNode;
        private PortItemBehaiver targetNode;
        private Dictionary<PortParentBehaiver, List<PortItemBehaiver>> connectedNodes = new Dictionary<PortParentBehaiver, List<PortItemBehaiver>>();
        public NodeConnectController(float sphereRange, float spanTime)
        {
            this.spanTime = spanTime;
            this.sphereRange = sphereRange;
        }

        public void Update()
        {
            timeSpan += Time.deltaTime;
            if (pickedUpItem != null && timeSpan > spanTime)
            {
                timeSpan = 0f;
               
                if (!FindConnectableObject())
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
                PortItemBehaiver tempNode;
                foreach (var item in pickedUpItem.ChildNodes)
                {
                    if (FindInstallableNode(item, out tempNode))
                    {
                        activeNode = item;
                        targetNode = tempNode;
                        if (onMatch != null) {
                            onMatch(activeNode);
                            onMatch(targetNode);
                        }
                        return true;
                    }
                }
            }
         
            return false;
        }

        private bool FindInstallableNode(PortItemBehaiver item, out PortItemBehaiver node)
        {
            Collider[] colliders = Physics.OverlapSphere(item.Pos, sphereRange, 1 << Layers.nodeLayer);
            if (colliders != null && colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    PortItemBehaiver tempNode = collider.GetComponent<PortItemBehaiver>();
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

        public void SetActiveItem(PortParentBehaiver item)
        {
            this.pickedUpItem = item;
            List<PortItemBehaiver> olditems = new List<PortItemBehaiver>();
            if (connectedNodes.ContainsKey(item))
            {
                List<PortItemBehaiver> needClear = new List<PortItemBehaiver>();
                for (int i = 0; i < connectedNodes[item].Count; i++)
                {
                    PortItemBehaiver nodeItem = connectedNodes[item][i];
                    needClear.Add(nodeItem);

                    PortItemBehaiver target = nodeItem.ConnectedNode;
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

        public void SetDisableItem(PortParentBehaiver item)
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
                        connectedNodes[pickedUpItem] = new List<PortItemBehaiver>();
                    }

                    connectedNodes[pickedUpItem].Add(activeNode);

                    if (!connectedNodes.ContainsKey(targetNode.Body))
                    {
                        connectedNodes[targetNode.Body] = new List<PortItemBehaiver>();
                    }

                    connectedNodes[targetNode.Body].Add(targetNode);

                    if (onConnected != null) onConnected.Invoke(new PortItemBehaiver[] { activeNode, targetNode });
                    activeNode = null;
                    targetNode = null;
                }
            }
        }
    }
}