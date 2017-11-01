using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class RopeObj : ActionObj
    {
        private float autoTime { get { return Setting.autoExecuteTime; } }
        private List<Collider> ropeNode = new List<Collider>();
        private List<int> connected = new List<int>();

        [SerializeField]
        private RopeItem ropeItem;
        public bool Installed { get { return ropeItem != null; } }
        public bool Connected { get { return connected.Count == ropeNode.Count; } }

        private Coroutine antoCoroutine;

        private void Awake()
        {
            TryAutoRegistRopeItem();
            RegistNodes();
        }

        /// <summary>
        /// 利用控制器放置元素
        /// </summary>
        /// <param name="ropeItem"></param>
        /// <returns></returns>
        public bool TryRegistRopeItem(RopeItem ropeItem)
        {
            if (this.ropeItem == null && ropeItem != null)
            {
                this.ropeItem = ropeItem;
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 支持提前放置好元素
        /// </summary>
        private void TryAutoRegistRopeItem()
        {
            if (ropeItem){
                ropeItem.QuickMoveTo(gameObject);
            }
        }

        private void RegistNodes()
        {
            foreach (var item in ropeNode)
            {
                item.gameObject.layer = Setting.ropeNodeLayer;
            }
        }
    }
}