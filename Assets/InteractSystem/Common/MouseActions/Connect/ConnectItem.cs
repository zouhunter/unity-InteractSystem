using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    public class ConnectItem : ActionItem
    {
        [SerializeField]
        protected ClickAbleFeature clickableFeature = new ClickAbleFeature();
        [SerializeField,Attributes.CustomField("可连接数")]
        protected int connectableCount = 100;
        protected List<ConnectItem> connected = new List<ConnectItem>();

        public ConnectItem[] Connected { get { return connected.ToArray() ; } }

        public override bool OperateAble
        {
            get
            {
                return connected.Count < connectableCount;
            }
        }
        public const string layer = "i:connectitem";

        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            clickableFeature.LayerName = layer;
            features.Add(clickableFeature);
            return features;
        }
        public void OnConnectTo(ConnectItem item)
        {
            if(!connected.Contains(item))
            {
                connected.Add(item);
            }
        }

        public void OnDisConnectTo(ConnectItem item)
        {
            Debug.Log("OnDisConnectTo:" + item);
            if(connected.Contains(item))
            {
                connected.Remove(item);
            }
        }
    }
}