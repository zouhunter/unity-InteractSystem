using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    public static class LinkUtil
    {
        public static void UpdateBrotherPos(LinkItem target,List<LinkItem> context)
        {
            if (target.ChildNodes == null || target.ChildNodes.Count == 0) return;

            context.Add(target);

            foreach (var item in target.ChildNodes)
            {
                if (item.ConnectedNode != null)
                {
                    var targetNode = item.ConnectedNode.Body;
                    var linkInfo = item.ConnectedNode.connectAble.Find(x => x.itemName == target.Name && x.nodeId == item.NodeID);
                    LinkUtil.ResetTargetTranform(targetNode, target, linkInfo.relativePos, linkInfo.relativeDir);

                    if (!context.Contains(targetNode))
                    {
                        UpdateBrotherPos(targetNode, context);
                    }
                }
            }
        }

        internal static void ResetTargetTranform(LinkItem target, LinkItem otherParent,Vector3 rPos,Vector3 rdDir)
        {
            target.Trans.position = otherParent.Trans.TransformPoint(rPos);
            target.Trans.forward = otherParent.Trans.TransformDirection(rdDir);
        }

        public static void AttachNodes(LinkPort moveAblePort, LinkPort staticPort)
        {
            moveAblePort.ConnectedNode = staticPort;
            staticPort.ConnectedNode = moveAblePort;
            moveAblePort.ResetTransform();
        }

        public static void DetachNodes(LinkPort moveAblePort, LinkPort staticPort)
        {
            moveAblePort.ConnectedNode = null;
            staticPort.ConnectedNode = null;
        }
        public static void RecordToDic(LinkHold[] ConnectedDic, LinkPort port)
        {
            var item = Array.Find(ConnectedDic, x => x.linkItem == port.Body);
            if(item == null)
            {
                item = Array.Find(ConnectedDic, x => x.linkItem == null);
                item.linkItem = port.Body;
            }
            
            if (item != null)
            {
                if(!item.linkedPorts.Contains(port))
                {
                    item.linkedPorts.Add(port);
                }
            }
        }
        public static void DetachConnectedPorts(LinkHold[] dic, Transform parent)
        {
            foreach (var item in dic)
            {
                var linkItem = item.linkItem;
                var ports = item.linkedPorts;
                linkItem.transform.SetParent(parent);
                foreach (var port in ports)
                {
                    port.ConnectedNode = null;
                }
            }
        }

        public static void ClampRotation(Transform target)
        {
            Vector3 newRot = target.eulerAngles;
            System.Func<float, float> clamp = (value) =>
            {
                float newValue = value % 360;
                if (newValue < 45)
                {
                    newValue = 0;
                }
                else
                {
                    if (newValue < 135)
                    {
                        newValue = 90;
                    }
                    else
                    {
                        if (newValue < 225)
                        {
                            newValue = 180;
                        }
                        else
                        {
                            if (newValue < 315)
                            {
                                newValue = -90;
                            }
                        }
                    }
                }
                return newValue;
            };

            newRot.x = clamp(newRot.x);
            newRot.y = clamp(newRot.y);
            newRot.z = clamp(newRot.z);

            target.eulerAngles = newRot;
        }
        /// <summary>
        /// 空间查找触发的点
        /// </summary>
        /// <param name="item"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static bool FindTriggerNodes(LinkPort item, out List<LinkPort> nodes)
        {
            nodes = null;
            Collider[] colliders = Physics.OverlapSphere(item.Pos, item.Range, LayerMask.GetMask( Layers.linknodeLayer));
            if (colliders != null && colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    LinkPort tempNode = collider.GetComponentInParent<LinkPort>();
                    if (tempNode == null || tempNode == item || tempNode.Body == item.Body)
                    {
                        continue;
                    }
                    else
                    {
                        if (nodes == null)
                            nodes = new List<WorldActionSystem.LinkPort>();
                        nodes.Add(tempNode);
                    }

                }
            }
            return nodes != null && nodes.Count > 0;
        }

     

        /// <summary>
        /// 空间查找可以可以连接的点
        /// </summary>
        /// <param name="item"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool FindInstallableNode(LinkPort item, out LinkPort node)
        {
            if (item.ConnectedNode != null)
            {
                node = item.ConnectedNode;
                return false;
            }

            Collider[] colliders = Physics.OverlapSphere(item.Pos, item.Range, LayerMask.GetMask(Layers.linknodeLayer));
            if (colliders != null && colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    LinkPort tempNode = collider.GetComponentInParent<LinkPort>();
                    if (tempNode == null || tempNode == item || tempNode.Body == item.Body || tempNode.ConnectedNode != null)
                    {
                        continue;
                    }
                    //主被动动连接点，非自身点，相同名，没有建立连接
                    if (tempNode.connectAble.Find((x) => x.itemName == item.Body.Name && x.nodeId == item.NodeID) != null)
                    {
                        //同属于一个源
                        if(tempNode.Body.BindingTarget == item.Body.BindingTarget)
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
    }
}