using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WorldActionSystem
{
    public class LinkCtrl : OperateController
    {
        private IHighLightItems highter;
        private LinkConnectController linkConnectCtrl;
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Link;
            }
        }
        private PickUpController pickCtrl { get { return ActionSystem.Instence.pickupCtrl; } }
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

        public override void Update()
        {
            if (linkConnectCtrl != null)
                linkConnectCtrl.Update();
        }

        void OnMatch(LinkPort itemA,LinkPort itemB)
        {
            highter.HighLightTarget(itemA.gameObject, Color.green);
            var linkInfo = itemA.connectAble.Find(x => x.itemName == itemB.Body.Name && x.nodeId == itemB.NodeID);
            LinkUtil.ResetTargetTranform(itemA.Body, itemB.Body, linkInfo.relativePos, linkInfo.relativeDir);
            LinkUtil.UpdateBrotherPos(itemA.Body, new List<LinkItem>());
            if (linkItem)
                linkItem.isMatching = true;
        }
        void OnDisMath(LinkPort itemA,LinkPort itemB)
        {
            highter.UnHighLightTarget(itemA.gameObject);
            highter.UnHighLightTarget(itemB.gameObject);
            if(linkItem) linkItem.isMatching = false;
        }

        void OnPickUp(PickUpAbleItem obj)
        {
            if (obj is LinkItem)
            {
                linkItem = obj as LinkItem;
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
        void OnPickTwinceLeft(PickUpAbleItem obj)
        {
            if (obj is LinkItem)
            {
                linkItem = obj as LinkItem;
                if (linkItem && !linkItem.Used)
                {
                    linkConnectCtrl.SetActiveItem(linkItem, true);
                }
            }
        }

        private void OnPickTwinceRight(PickUpAbleItem obj)
        {
            if (obj is LinkItem)
            {
                ElementController.Instence.ClearExtraCreated();
            }
        }

        /// <summary>
        /// 放下
        /// </summary>
        /// <param name="obj"></param>
        void OnPickDown(PickUpAbleItem obj)
        {
            if (obj is LinkItem)
            {
                LinkUtil.ClearActivedLinkPort(obj as LinkItem);
                var linkItem = obj as LinkItem;
                if (linkItem){
                    linkConnectCtrl.SetDisableItem();
                }
            }
        }

        void OnPickStay(PickUpAbleItem go)
        {
            if (go is LinkItem)
            {
                LinkUtil. ClearActivedLinkPort(go as LinkItem);
                linkConnectCtrl.TryConnect();
                pickCtrl.PickStay();
            }
        }

        void OnConnected(LinkPort[] nodes)
        {
            foreach (var item in nodes)
            {
                item.Body.OnConnected();
                var childNodes = item.Body.ChildNodes;
                foreach (var node in childNodes)
                {
                    highter.UnHighLightTarget(node.gameObject);
                }
            }

            Debug.Log("Connected");
        }
        
    }
}