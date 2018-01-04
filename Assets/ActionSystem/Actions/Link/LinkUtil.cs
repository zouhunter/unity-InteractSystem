using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public static class LinkUtil
    {

        public static void AttachNodes(LinkPort moveAblePort, LinkPort staticPort)
        {
            moveAblePort.ConnectedNode = staticPort;
            staticPort.ConnectedNode = moveAblePort;
            moveAblePort.ResetTransform();
            moveAblePort.Body.transform.SetParent(staticPort.Body.transform);
        }

        public static void DetachNodes(LinkPort moveAblePort, LinkPort staticPort, Transform parent)
        {
            moveAblePort.ConnectedNode = null;
            staticPort.ConnectedNode = null;
            moveAblePort.transform.SetParent(parent);
        }
        public static void DetachConnectedPorts(Dictionary<LinkItem, List<LinkPort>> dic, Transform parent)
        {

        }
    }
}