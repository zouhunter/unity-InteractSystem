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
        public static void UpdateBrotherPos(LinkItem target, List<LinkItem> context)
        {
            if (target.ChildNodes == null || target.ChildNodes.Count == 0) return;

            context.Add(target);

            foreach (var item in target.ChildNodes)
            {
                if (item.ConnectedNode != null && !context.Contains(item.ConnectedNode.Body))
                {
                    var targetNode = item.ConnectedNode.Body;
                    var linkInfo = item.ConnectedNode.connectAble.Find(x => x.itemName == target.Name && x.nodeId == item.NodeID);

                    LinkUtil.ResetTargetTranform(targetNode, target, linkInfo.relativePos, linkInfo.relativeDir);
                    UpdateBrotherPos(targetNode, context);
                }
            }
        }

        public static void RecordTransform(LinkInfo nodeArecored, LinkInfo nodeBrecored, Transform ourItem, Transform otherItem)
        {
            var parent = ourItem.parent;
            ourItem.SetParent(otherItem);
            //当ourItem作为otherItem子物体时的坐标
            nodeArecored.relativePos = ourItem.localPosition;
            nodeArecored.relativeDir = ourItem.localEulerAngles;
            ourItem.SetParent(parent);
            parent = otherItem.parent;
            otherItem.SetParent(ourItem);
            //当otherItem作为ourItem子物体时的坐标
            nodeBrecored.relativePos = otherItem.localPosition;
            nodeBrecored.relativeDir = otherItem.localEulerAngles;
            otherItem.SetParent(parent);
        }

        public static void GetWorldPosFromTarget(LinkItem target, Vector3 rPos, Vector3 rdDir,out Vector3 position, out Vector3 dir)
        {
            var temp = new GameObject("temp");
            temp.transform.SetParent(target.transform);
            temp.transform.localPosition = rPos;
            temp.transform.localEulerAngles = rdDir;

            position = temp.transform.position;
            dir = temp.transform.eulerAngles;
            UnityEngine.Object.Destroy(temp);
        }
        public static void ResetTargetTranform(LinkItem target, LinkItem otherParent, Vector3 rPos, Vector3 rdDir)
        {
            var parent = target.Trans.parent;
            target.Trans.SetParent(otherParent.Trans);
            //当target作为otherParent子物体并移开的坐标
            target.Trans.localPosition = rPos;
            target.Trans.localEulerAngles = rdDir;
            target.Trans.SetParent(parent);
        }

        private static Vector3 InverseTransformDirection(Vector3 parent,Vector3 current)
        {
            return parent - current;
        }
        private static Vector3 TransformDirection(Vector3 parent,Vector3 current)
        {
            return parent + current;
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
            Collider[] colliders = Physics.OverlapSphere(item.Pos, item.Range, LayerMask.GetMask(Layers.linknodeLayer));
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
                    var linkInfo = item.connectAble.Find((x) => x.itemName == tempNode.Body.Name && x.nodeId == tempNode.NodeID);
                    if (linkInfo != null)
                    {
                        //同属于一个源
                        node = tempNode;
                        LinkUtil.ResetTargetTranform(item.Body, node.Body, linkInfo.relativePos, linkInfo.relativeDir);
                        return true;
                    }
                }
            }
            node = null;
            return false;
        }
    }
}