using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InteractSystem.Actions
{
    public class LinkCtrl : PCOperateCtrl<LinkCtrl>,IUpdateAble
    {
        private IHighLightItems highter;
        private LinkConnectController linkConnectCtrl;
        private PickUpController pickCtrl { get { return PickUpController.Instence; } }
        private LinkItem linkItem;

        public LinkCtrl()
        {
            highter = new ShaderHighLight();
            linkConnectCtrl = new LinkConnectController();
            linkConnectCtrl.onDisMatch = OnDisMath;
            linkConnectCtrl.onMatch = OnMatch;
            linkConnectCtrl.onConnected = OnConnected;

            pickCtrl.onPickup += (OnPickUp);
            pickCtrl.onPickdown += (OnPickDown);
            pickCtrl.onPickStay += (OnPickStay);
            pickCtrl.onPickTwinceLeft += (OnPickTwinceLeft);
            pickCtrl.onPickTwinceRight += OnPickTwinceRight;
        }

        public void Update()
        {
            if (!Active) return;

            if (linkItem != null && linkConnectCtrl != null) { 
                linkConnectCtrl.Update();
            }
        }

        void OnMatch(LinkPort itemA, LinkPort itemB)
        {
            if (!Active) return;

            highter.HighLightTarget(itemA.gameObject, Color.green);
            var linkInfo = itemA.connectAble.Find(x => x.itemName == itemB.Body.Name && x.nodeId == itemB.NodeID);
            LinkUtil.ResetTargetTranform(itemA.Body, itemB.Body, linkInfo.relativePos, linkInfo.relativeDir);
            LinkUtil.UpdateBrotherPos(itemA.Body, new List<LinkItem>());

            if (linkItem)
            {
                linkItem.isMatching = true;
            }
        }
        void OnDisMath(LinkPort itemA, LinkPort itemB)
        {
            if (!Active) return;

            highter.UnHighLightTarget(itemA.gameObject);
            highter.UnHighLightTarget(itemB.gameObject);

            if (linkItem)
            {
                linkItem.isMatching = false;
            }

        }

        void OnPickUp(PickUpAbleComponent obj)
        {
            if (!Active) return;

            var item = obj.GetComponentInParent<LinkItem>();
            if (item)
            {
                this.linkItem = item;
                if (linkItem)
                {
                    linkConnectCtrl.SetActiveItem(linkItem, false);
                    //显示可以所有可以安装的点
                    LinkUtil.TryActiveLinkPorts(linkItem);
                }
            }
        }

        /// <summary>
        /// 解除连接
        /// </summary>
        /// <param name="obj"></param>
        void OnPickTwinceLeft(PickUpAbleComponent obj)
        {
            if (!Active) return;

            var item = obj.GetComponentInParent<LinkItem>();

            if (item)
            {
                this.linkItem = item;
                if (linkItem && !linkItem.Used)
                {
                    linkConnectCtrl.SetActiveItem(linkItem, true);
                }
            }
        }

        private void OnPickTwinceRight(PickUpAbleComponent obj)
        {
            if (!Active) return;

            var item = obj.GetComponentInParent<LinkItem>();

            if (item)
            {
                ElementController.Instence.ClearExtraCreated();
            }

            linkItem = null;
        }

        /// <summary>
        /// 放下
        /// </summary>
        /// <param name="obj"></param>
        void OnPickDown(PickUpAbleComponent obj)
        {
            if (!Active) return;

            var item = obj.GetComponentInParent<LinkItem>();

            if (item)
            {
                LinkUtil.ClearActivedLinkPort(linkItem);
                if (linkItem)
                {
                    linkConnectCtrl.SetDisableItem();
                }
            }
            this.linkItem = null;
        }

        void OnPickStay(PickUpAbleComponent obj)
        {
            if (!Active) return;

            var item = obj.GetComponentInParent<LinkItem>();

            if (item)
            {
                LinkUtil.ClearActivedLinkPort(item);
                linkConnectCtrl.TryConnect();
                pickCtrl.PickStay();
            }

            linkItem = null;
        }

        void OnConnected(LinkPort[] nodes)
        {
            if (!Active) return;

            foreach (var item in nodes)
            {
                //item.Body.OnConnected();
                TryCompleteLinkNode();
                var childNodes = item.Body.ChildNodes;
                foreach (var node in childNodes)
                {
                    highter.UnHighLightTarget(node.gameObject);
                }
            }

          if(log)  Debug.Log("Connected");
        }

        private void TryCompleteLinkNode()
        {
            var list = lockList.ToArray();
            foreach (var item in list)
            {
                (item as LinkNode).TryComplete();
            }
        }

    }
}