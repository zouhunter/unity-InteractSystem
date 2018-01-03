using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class PortParentBehaiver : MonoBehaviour
    {
        private List<PortItemBehaiver> _childNodes = new List<PortItemBehaiver>();
        public List<PortItemBehaiver> ChildNodes
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

        public void ResetBodyTransform(PortParentBehaiver otherParent, Vector3 rPos, Vector3 rdDir)
        {
            transform.position = otherParent.Trans.TransformPoint(rPos);
            transform.forward = otherParent.Trans.TransformDirection(rdDir);
        }

        private void Awake()
        {
            var nodeItems = GetComponentsInChildren<PortItemBehaiver>(true);
            _childNodes.AddRange(nodeItems);

            foreach (var item in nodeItems)
            {
                item.Body = this;
            }
        }
    }

}