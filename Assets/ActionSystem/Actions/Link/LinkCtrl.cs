using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

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
        private AngleCtroller angleCtrl { get { return ActionSystem.Instence.angleCtrl; } }

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
            pickCtrl.onPickTwince += (OnPickTwince);
        }

        public override void Update()
        {
            if (linkConnectCtrl != null)
                linkConnectCtrl.Update();
        }

        void OnMatch(LinkPort item)
        {
            highter.HighLightTarget(item.gameObject, Color.green);
        }
        void OnDisMath(LinkPort item)
        {
            highter.UnHighLightTarget(item.gameObject);
        }

        void OnPickUp(PickUpAbleItem obj)
        {
            if (obj is LinkItem)
            {
                var linkItem = obj as LinkItem;
                if (linkItem)
                {
                    linkConnectCtrl.SetActiveItem(linkItem, false);
                    //显示可以所有可以安装的点
                    //TryActiveLinkPort(linkItem);
                }
            }
        }

        /// <summary>
        /// 解除连接
        /// </summary>
        /// <param name="obj"></param>
        void OnPickTwince(PickUpAbleItem obj)
        {
            if (obj is LinkItem)
            {
                var linkItem = obj as LinkItem;
                if (linkItem && !linkItem.Used)
                {
                    linkConnectCtrl.SetActiveItem(linkItem, true);
                }
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
                var linkItem = obj as LinkItem;
                if (linkItem)
                {
                    linkConnectCtrl.SetDisableItem();
                    //显示可以操作的元素
                    //ActiveOneLinkItem();
                }
            }
        }

        void OnPickStay(PickUpAbleItem go)
        {
            if (go is LinkItem)
            {
                linkConnectCtrl.TryConnect();
                pickCtrl.PickDown();
                Debug.Log("OnPickStatu");
            }
        }

        //void ActiveAngle()
        //{
        //    var notLinked = linkPool.Where(x => x != null && !HaveConnected(x)).FirstOrDefault();
        //    if (notLinked != null)
        //    {
        //        angleCtrl.UnNotice(anglePos);
        //        anglePos = notLinked.transform;
        //    }
        //}

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