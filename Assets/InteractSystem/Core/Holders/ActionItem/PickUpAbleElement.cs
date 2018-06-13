using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem
{
    public abstract class PickUpAbleElement : ClickAbleCompleteAbleActionItem
    {
        private PickUpAbleItem pickUpAbleItem;
        public PickUpAbleItem PickUpItem { get { return pickUpAbleItem; } }
        protected override void Awake()
        {
            base.Awake();
            InitPickupAbleComponent();
        }

        protected virtual void InitPickupAbleComponent()
        {
            pickUpAbleItem = GetComponent<PickUpAbleItem>();
            if (pickUpAbleItem == null){
                pickUpAbleItem = Collider.gameObject.AddComponent<PickUpAbleItem>();
                pickUpAbleItem.onPickUp = new UnityEvent();
                pickUpAbleItem.onPickDown = new UnityEvent();
                pickUpAbleItem.onPickStay = new UnityEvent();
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
        public override void StepActive()
        {
            base.StepActive();
            PickUpItem.PickUpAble = true;
        }
        public override void StepComplete()
        {
            base.StepComplete();
            PickUpItem.PickUpAble = false;
        }
        public override void StepUnDo()
        {
            base.StepUnDo();
            PickUpItem.PickUpAble = false;
        }
        protected virtual void OnPickUp() { }
        protected virtual void OnPickStay() { }
        protected virtual void OnPickDown() { }
    }
}