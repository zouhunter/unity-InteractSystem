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
        private LinkObj linkObj { get; set; }
        private IHighLightItems highter;

        private LinkConnectController linkConnectCtrl;
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Link;
            }
        }
        private PickUpController pickCtrl { get { return ActionSystem.Instence.pickUpCtrl; } }

        public LinkCtrl()
        {
            highter = new ShaderHighLight();
            linkConnectCtrl = new LinkConnectController();
            linkConnectCtrl.onDisMatch = OnDisMath;
            linkConnectCtrl.onMatch = OnMatch;
            linkConnectCtrl.onConnected = OnConnected;
            linkConnectCtrl.onDisconnected = OnDisConnected;
            pickCtrl.onPickup += (OnPickUp);
            pickCtrl.onPickdown += (OnPickDown);
            pickCtrl.onPickStay += (OnPickStay);
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
        void OnPickUp(IPickUpAbleItem obj)
        {
            if (obj is LinkItem)
            {
                var linkItem = obj as LinkItem;
                if (linkItem)
                {
                    linkObj = linkItem.GetComponentInParent<LinkObj>();
                    if (linkObj)
                    {
                        linkConnectCtrl.SetState(linkObj.transform, linkObj.ConnectedDic);
                        linkConnectCtrl.SetActiveItem(linkItem);
                    }
                }
            }
        }

        void OnPickDown(IPickUpAbleItem obj)
        {
            if (obj is LinkItem)
            {
                linkConnectCtrl.SetDisableItem(obj as LinkItem);
                linkObj.TryComplete();
            }
        }
        void OnPickStay(IPickUpAbleItem go)
        {
            linkConnectCtrl.TryConnect();
            pickCtrl.PickDown();
            Debug.Log("OnPickStatu");
        }

        void OnConnected(LinkPort[] nodes)
        {
            foreach (var item in nodes)
            {
                highter.UnHighLightTarget(item.gameObject);
            }
        }
        void OnDisConnected(LinkPort[] nodes)
        {
            foreach (var item in nodes)
            {
                highter.UnHighLightTarget(item.gameObject);
            }
        }

    }
}