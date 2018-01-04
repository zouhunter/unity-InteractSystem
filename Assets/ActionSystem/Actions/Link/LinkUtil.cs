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
        public static void RecordToDic(Dictionary<LinkItem, List<LinkPort>> ConnectedDic, LinkPort port)
        {
            var item = port.Body;

            if (!ConnectedDic.ContainsKey(item))
            {
                ConnectedDic[item] = new List<LinkPort>();
            }

            ConnectedDic[item].Add(port);
        }
        public static void DetachConnectedPorts(Dictionary<LinkItem, List<LinkPort>> dic, Transform parent)
        {
            foreach (var item in dic)
            {
                var linkItem = item.Key;
                var ports = item.Value;
                linkItem.transform.SetParent(parent);
                foreach (var port in ports){
                    port.ConnectedNode = null;
                }
            }
        }
    }
}