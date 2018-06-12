using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem
{
    [RequireComponent(typeof(PickUpAbleItem))]
    public abstract class PickUpAbleElement : ClickAbleActionItem
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
            pickUpAbleItem.onPickUp.AddListener(OnPickUp);
            pickUpAbleItem.onPickStay.AddListener(OnPickStay);
            pickUpAbleItem.onSetPosition += OnSetPosition;
            pickUpAbleItem.onSetViewForward += OnSetViewForward;
        }
        protected virtual void OnSetPosition(Vector3 arg0)
        {
            transform.position = arg0;
        }
        protected virtual void OnSetViewForward(Vector3 arg0)
        {
            transform.forward = arg0;
        }
        protected abstract void OnPickUp();
        protected abstract void OnPickStay();
        protected abstract void OnPickDown();
    }
}