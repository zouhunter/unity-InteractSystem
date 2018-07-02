using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem
{
    [Serializable]
    public class PickUpAbleFeature: ClickAbleFeature{
        private PickUpAbleComponent _pickUpAbleItem;
      
        [SerializeField]
        private bool pickUpAble = true;
        private PickUpAbleComponent pickUpAbleItem
        {
            get
            {
                if (_pickUpAbleItem == null)
                {
                    _pickUpAbleItem = collider.gameObject.GetComponent<PickUpAbleComponent>();
                    if (_pickUpAbleItem == null)
                    {
                        _pickUpAbleItem = collider.gameObject.AddComponent<PickUpAbleComponent>();
                        _pickUpAbleItem.onPickUp = new UnityEvent();
                        _pickUpAbleItem.onPickDown = new UnityEvent();
                        _pickUpAbleItem.onPickStay = new UnityEvent();
                    }
                }
                return _pickUpAbleItem;
            }
        }

 
        public override string LayerName
        {
            get
            {
                _layerName = PickUpAbleItem.layer;
                return _layerName;
            }

            set
            {
                _layerName = value;
            }
        }
		
        public void OnPickDown()
        {
            pickUpAbleItem.OnPickDown();
        }
        public void OnPickUp()
        {
            pickUpAbleItem.OnPickUp();
        }
        public void OnPickStay()
        {
            pickUpAbleItem.OnPickStay();
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
            base.StepActive();
            if (pickUpAble)
            {
                PickUpAble = true;
            }
        }

        public override void StepComplete()
        {
            base.StepComplete();
            if (pickUpAble)
            {
                PickUpAble = false;
            }
        }

        public override void StepUnDo()
        {
            base.StepUnDo();
            if (pickUpAble)
                PickUpAble = true;
        }
		
        public bool PickUpAble
        {
            get
            {
                return pickUpAble && pickUpAbleItem.PickUpAble;
            }
            set
            {
                pickUpAbleItem.PickUpAble = value;
            }
        }
    }

}