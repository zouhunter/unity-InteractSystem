using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class LinkCtrl
    {
        [Range(0.1f, 1f)]
        public float nodeUpdateSpanTime = 0.5f;
        [Range(0.02f, 0.1f)]
        public float pickUpSpantime = 0.02f;
        [Range(10, 60)]
        public int scrollSpeed = 20;
        [Range(0, 1)]
        public float sphereRange = 0.1f;
        [Range(3, 15)]
        public float distence = 1f;

        public Dictionary<LinkItem, List<LinkPort>> ConnectedDic { get { return nodeConnectCtrl.ConnectedDic; } }


        private PickUpController pickCtrl { get { return ActionSystem.Instence.pickUpCtrl; } }
        private LinkNodeConnectController nodeConnectCtrl;
        private IHighLightItems highter;
        public void Start()
        {
            highter = new ShaderHighLight();
            nodeConnectCtrl = new LinkNodeConnectController(sphereRange, nodeUpdateSpanTime);
            pickCtrl.RegistOnPickup(OnPickUp);
            pickCtrl.RegistOnPickDown(OnPickDown);
            pickCtrl.RegistOnPickStatu(OnPickStatu);
            nodeConnectCtrl.onDisMatch += OnDisMath;
            nodeConnectCtrl.onMatch += OnMatch;
            nodeConnectCtrl.onConnected += OnConnected;
            nodeConnectCtrl.onDisconnected += OnDisConnected;
        }

        public void Update()
        {
            if (UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
            if (pickCtrl != null) pickCtrl.Update();
            if (nodeConnectCtrl != null) nodeConnectCtrl.Update();
        }

        void OnMatch(LinkPort item)
        {
        }
        void OnDisMath(LinkPort item)
        {
        }
        void OnPickUp(IPickUpAbleItem obj)
        {
            nodeConnectCtrl.SetActiveItem(obj as LinkItem);
        }

        void OnPickDown(IPickUpAbleItem obj)
        {
            nodeConnectCtrl.SetDisableItem(obj as LinkItem);
        }

        void OnConnected(LinkPort[] nodes)
        {
            foreach (var item in nodes)
            {
            }
        }
        void OnDisConnected(LinkPort[] nodes)
        {
            foreach (var item in nodes)
            {
            }
        }
        void OnPickStatu(IPickUpAbleItem go)
        {
            nodeConnectCtrl.TryConnect();
        }
    }
}