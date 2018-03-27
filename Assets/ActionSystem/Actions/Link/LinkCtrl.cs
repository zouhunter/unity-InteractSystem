﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class LinkCtrl : OperateController
    {
        //private LinkObj linkObj { get; set; }
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
                if (linkItem &&! linkItem.Used)
                {
                    if(linkItem.linkObjects.Count > 0)
                    {
                        var linkObj = linkItem.linkObjects[0];
                        linkObj.TryActiveLinkPort(linkItem);
                        foreach (var item in linkItem.linkObjects)
                        {
                            linkObj = item;
                            if (linkObj)
                            {
                                linkConnectCtrl.SetState(linkObj.linkItems);
                                linkConnectCtrl.SetActiveItem(linkItem, false);
                            }
                        }
                        
                    }
                   
                }
            }
        }
        void OnPickTwince(PickUpAbleItem obj)
        {
            if (obj is LinkItem)
            {
                var linkItem = obj as LinkItem;
                if (linkItem && !linkItem.Used)
                {
                    foreach (var linkObj in linkItem.linkObjects)
                    {
                        linkConnectCtrl.SetState(linkObj.linkItems);
                        linkConnectCtrl.SetActiveItem(linkItem, true);
                    }
                   
                }
            }
        }
        void OnPickDown(PickUpAbleItem obj)
        {
            if (obj is LinkItem)
            {
                var linkItem = obj as LinkItem;
                linkConnectCtrl.SetDisableItem(obj as LinkItem);
                foreach (var item in linkItem.linkObjects)
                {
                    var linkObj = item;
                    if (linkObj)
                        linkObj.ActiveOneLinkItem();
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

        void OnConnected(LinkPort[] nodes)
        {
            foreach (var item in nodes)
            {
                var childNodes = item.Body.ChildNodes;
                foreach (var node in childNodes)
                {
                    highter.UnHighLightTarget(node.gameObject);
                }
            }
        }
    }
}