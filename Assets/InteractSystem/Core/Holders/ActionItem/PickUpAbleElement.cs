using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem
{
    [RequireComponent(typeof(PickUpAbleItem))]
    public abstract class PickUpAbleElement : ActionItem
    {
        private PickUpAbleItem pickUpAbleItem;

        protected override void Awake()
        {
            base.Awake();
            InitPickupAbleComponent();
        }

        protected virtual void InitPickupAbleComponent()
        {
            pickUpAbleItem = GetComponent<PickUpAbleItem>();
            if (pickUpAbleItem == null)
            {
                pickUpAbleItem = gameObject.AddComponent<PickUpAbleItem>();
            }
            pickUpAbleItem.onPickDown.AddListener(OnPickDown);
        }

        protected abstract void OnPickDown();
    }
}