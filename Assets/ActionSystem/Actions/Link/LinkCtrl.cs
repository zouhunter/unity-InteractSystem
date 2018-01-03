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
        private LinkObj linkObj;
        public Dictionary<LinkItem, List<LinkPort>> ConnectedDic { get { return nodeConnectCtrl.ConnectedDic; } }
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Link;
            }
        }

        private PickUpController pickCtrl { get { return ActionSystem.Instence.pickUpCtrl; } }
        private LinkNodeConnectController nodeConnectCtrl;
        private IHighLightItems highter;

        public LinkCtrl()
        {
            highter = new ShaderHighLight();
            nodeConnectCtrl = new LinkNodeConnectController();
            pickCtrl.onPickup += (OnPickUp);
            pickCtrl.onPickdown += (OnPickDown);
            pickCtrl.onPickStay += (OnPickStay);
            nodeConnectCtrl.onDisMatch += OnDisMath;
            nodeConnectCtrl.onMatch += OnMatch;
            nodeConnectCtrl.onConnected += OnConnected;
            nodeConnectCtrl.onDisconnected += OnDisConnected;
        }

        public override void Update()
        {
            if (nodeConnectCtrl != null)
                nodeConnectCtrl.Update();
        }

        void OnMatch(LinkPort item)
        {
            highter.HighLightTarget(item.gameObject, Color.green);
            Debug.Log("OnMatch:" + item);
        }
        void OnDisMath(LinkPort item)
        {
            highter.UnHighLightTarget(item.gameObject);
            Debug.Log("OnDisMath:" + item);
        }
        void OnPickUp(IPickUpAbleItem obj)
        {
            if (obj is LinkItem)
            {
                nodeConnectCtrl.SetActiveItem(obj as LinkItem);
                linkObj = (obj as LinkItem).GetComponentInParent<LinkObj>();
            }
        }

        void OnPickDown(IPickUpAbleItem obj)
        {
            if (obj is LinkItem)
            {
                nodeConnectCtrl.SetDisableItem(obj as LinkItem);
                TryComplete();
            }
        }
        void OnPickStay(IPickUpAbleItem go)
        {
            nodeConnectCtrl.TryConnect();
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

        void TryComplete()
        {
            if (linkObj != null)
            {
                if (linkObj.LinkItems.Length == ConnectedDic.Count)
                {
                    linkObj.OnEndExecute(false);
                }
            }
        }

    }
}