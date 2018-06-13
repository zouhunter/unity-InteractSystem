using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Common.Actions
{
    public class LinkConnectController
    {
        public UnityAction<LinkPort[]> onConnected { get; set; }
        public UnityAction<LinkPort, LinkPort> onMatch { get; set; }
        public UnityAction<LinkPort, LinkPort> onDisMatch { get; set; }
        public UnityAction<LinkPort[]> onDisconnected { get; set; }
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
                        onMatch(activeNode, targetNode);
                    }
                }
                else
                {
                    if (onDisMatch != null && activeNode != null && targetNode != null){
                        onDisMatch.Invoke(targetNode, activeNode);
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
                foreach (var item in pickedUpItem.GroupNodes)
                {
                    if (LinkUtil.FindInstallableNode(item, out tempNode))
                    {
                        activeNode = item;
                        targetNode = tempNode;
                        return true;
                    }
                }
            }

            return false;
        }


        public void SetActiveItem(LinkItem item, bool detach)
        {
            this.pickedUpItem = item;

            ///如果目标已经被使用，阻止和其他元素断开
            if (item.Used)
            {
                List<LinkPort> disconnected = new List<LinkPort>();
                LinkPort[] connectedPort = item.GetLinkedPorts();

                if (detach)
                {
                    for (int i = 0; i < connectedPort.Length; i++)
                    {
                        LinkPort port = connectedPort[i];
                        LinkPort otherPort = port.ConnectedNode;

                        LinkUtil.DetachNodes(port, otherPort);
                        disconnected.Add(port);
                        disconnected.Add(otherPort);
                    }

                    if (onDisconnected != null)
                        onDisconnected.Invoke(disconnected.ToArray());
                }
            }
        }

        public void SetDisableItem()
        {
            pickedUpItem = null;
            targetNode = null;
            activeNode = null;
        }

        public void TryConnect()
        {
            if (activeNode != null && targetNode != null)
            {
                LinkUtil.AttachNodes(activeNode, targetNode);

                if (onConnected != null)
                    onConnected.Invoke(new LinkPort[] { activeNode, targetNode });

                activeNode = null;
                targetNode = null;
            }
        }
    }
}