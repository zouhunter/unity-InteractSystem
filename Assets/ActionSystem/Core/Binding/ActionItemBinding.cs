using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem.Binding
{
    public class ActionItemBinding : MonoBehaviour
    {
        protected ActionItem actionItem;

        protected virtual void Awake()
        {
            actionItem = gameObject.GetComponent<ActionItem>();
            actionItem.onActive.AddListener(OnActive);
            actionItem.onInActive.AddListener(OnInActive);
        }
        protected virtual void OnDestroy()
        {
            if (actionItem)
            {
                actionItem.onActive.RemoveListener(OnActive);
                actionItem.onInActive.RemoveListener(OnInActive);
            }
        }
        protected virtual void OnActive() { }
        protected virtual void OnInActive() { }
    }
}