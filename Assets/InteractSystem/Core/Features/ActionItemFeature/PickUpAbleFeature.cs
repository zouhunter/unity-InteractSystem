using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem
{
    public class PickUpAbleFeature:ActionItemFeature{
        protected Collider _collider;
        private PickUpAbleItem pickUpAbleItem;

        public Collider collider { get { return _collider; } set { InitColider(value);_collider = value; } }
        private void InitColider(Collider collider)
        {
            //this.collider = collider;
            pickUpAbleItem = collider.GetComponent<PickUpAbleItem>();

            if (pickUpAbleItem == null)
            {
                pickUpAbleItem = collider.gameObject.AddComponent<PickUpAbleItem>();
                pickUpAbleItem.onPickUp = new UnityEvent();
                pickUpAbleItem.onPickDown = new UnityEvent();
                pickUpAbleItem.onPickStay = new UnityEvent();
            }
        }
        public void RegistOnPickDown(UnityAction onPickDown)
        {
            pickUpAbleItem.onPickDown.AddListener(onPickDown);
        }
        public void RegistOnPickStay(UnityAction onPickStay)
        {
            pickUpAbleItem.onPickStay.AddListener(onPickStay);
        }
        public void RegistOnPickUp(UnityAction onPickUp)
        {
            pickUpAbleItem.onPickUp.AddListener(onPickUp);
        }
        public void RemoveOnPickDown(UnityAction onPickDown)
        {
            pickUpAbleItem.onPickDown.RemoveListener(onPickDown);
        }
        public void RemoveOnPickStay(UnityAction onPickStay)
        {
            pickUpAbleItem.onPickStay.RemoveListener(onPickStay);
        }
        public void RemoveOnPickUp(UnityAction onPickUp)
        {
            pickUpAbleItem.onPickUp.RemoveListener(onPickUp);
        }

        public void RegistOnSetPosition(UnityAction<Vector3> onSetPosition)
        {
            pickUpAbleItem.onSetPosition += onSetPosition;
        }
        public void RegistOnSetViweForward(UnityAction<Vector3> onSetViewForward)
        {
            pickUpAbleItem.onSetViewForward += onSetViewForward;
        }
        public void RemoveOnSetPosition(UnityAction<Vector3> onSetPosition)
        {
            pickUpAbleItem.onSetPosition -= onSetPosition;
        }
        public void RemoveOnSetViweForward(UnityAction<Vector3> onSetViewForward)
        {
            pickUpAbleItem.onSetViewForward -= onSetViewForward;
        }

        public override void StepActive()
        {
            PickUpAble = true;
        }

        public override void StepComplete()
        {
            PickUpAble = false;
        }

        public override void StepUnDo()
        {
            PickUpAble = true;
        }

        public bool PickUpAble
        {
            get
            {
                return pickUpAbleItem.PickUpAble;
            }
            set
            {
                pickUpAbleItem.PickUpAble = value;
            }
        }
    }

}