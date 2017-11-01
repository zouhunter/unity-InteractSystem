using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    public class RopeItem : PickUpAbleElement
    {
        public UltimateRope rope;
        public Collider[] ropeNode;
        private RopeObj obj;
        private void Awake()
        {
            RegistNodes();
        }

        private void RegistNodes()
        {
            foreach (var item in ropeNode)
            {
                item.gameObject.layer = Setting.pickUpElementLayer;
                
            }
        }
    }
}