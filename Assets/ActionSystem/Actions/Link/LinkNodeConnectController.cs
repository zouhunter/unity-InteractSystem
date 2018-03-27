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
        private LinkHold[] ConnectedDic { get; set; }
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
     

        public void SetActiveItem(LinkItem item,bool detach)
        {
            this.pickedUpItem = item;

            List<LinkPort> disconnected = new List<LinkPort>();
            var hold = Array.Find(ConnectedDic, x => x.linkItem == item);
            if (hold != null)
            {
                LinkPort[] connectedPort = hold.linkedPorts.ToArray();
                if (detach)
                {
                    for (int i = 0; i < connectedPort.Length; i++)
                    {
                        LinkPort port = connectedPort[i];
                        LinkPort otherPort = port.ConnectedNode;

                        hold.linkedPorts.Remove(port);
                        var other = Array.Find(ConnectedDic, x => x.linkItem == otherPort.Body);
                        other.linkedPorts.Remove(otherPort);
                        LinkUtil.DetachNodes(port, otherPort);
                        disconnected.Add(port);
                        disconnected.Add(otherPort);
                    }
                }
                   

                if (onDisconnected != null)
                    onDisconnected.Invoke(disconnected.ToArray());
            }
        }

        public void SetState(LinkHold[] ConnectedDic)
        {
            this.ConnectedDic = ConnectedDic;
        }

        public void SetDisableItem(LinkItem item)
        {
            pickedUpItem = null;
            targetNode = null;
            activeNode = null;
        }

        public void TryConnect()
        {
            if (activeNode != null && targetNode != null)
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