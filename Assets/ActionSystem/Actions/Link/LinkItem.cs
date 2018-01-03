using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class LinkItem : MonoBehaviour,IPickUpAbleItem
    {
        private List<LinkPort> _childNodes = new List<LinkPort>();
        public List<LinkPort> ChildNodes
        {
            get
            {
                return _childNodes;
            }
        }

        public string Name {
            get
            {
                return name;
            }
        }

        public Transform Trans
        {
            get
            {
                return transform;
            }
        }

        public bool PickUpAble { get; set; }

        public Collider Collider { get; private set; }

        public void OnPickDown()
        {
            
        }

        public void OnPickUp()
        {
            
        }

        public void ResetBodyTransform(LinkItem otherParent, Vector3 rPos, Vector3 rdDir)
        {
            transform.position = otherParent.Trans.TransformPoint(rPos);
            transform.forward = otherParent.Trans.TransformDirection(rdDir);
        }

        public void SetPosition(Vector3 pos)
        {
           
        }

        private void Awake()
        {
            var nodeItems = GetComponentsInChildren<LinkPort>(true);
            _childNodes.AddRange(nodeItems);

            foreach (var item in nodeItems)
            {
                item.Body = this;
            }
        }
    }

}