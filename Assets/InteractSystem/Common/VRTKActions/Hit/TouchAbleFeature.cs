using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractSystem;
using UnityEngine.Events;
using VRTK;

namespace InteractSystem.VRTKActions
{
	[System.Serializable]
	public class TouchAbleFeature : ClickAbleFeature
	{
		private VRTK_InteractableObject _interactableObject;
		private UnityAction onTouch{get;set;}
		
        [SerializeField]
        private bool touchAble = true;
		
        public bool TouchAble{get{return touchAble;}set{touchAble = value;}}
		
        private VRTK_InteractableObject interactableObject
        {
            get
            {
                if (_interactableObject == null)
                {
                    _interactableObject = collider.gameObject.GetComponent<VRTK_InteractableObject>();
                    if (_interactableObject == null)
                    {
                        _interactableObject = collider.gameObject.AddComponent<VRTK_InteractableObject>();
                    }
                }
                return _interactableObject;
            }
        }
		protected virtual void OnInteractableObjectUsed(object sender, InteractableObjectEventArgs e){
			Debug.Log("OnInteractableObjectUsed");
			if(touchAble && onTouch != null){
				onTouch.Invoke();
			}
		}

        public override void StepActive()
        {
            base.StepActive();
            if (!touchAble)
            {
                touchAble = true;
            }
        }

        public override void StepComplete()
        {
            base.StepComplete();
            if (touchAble)
            {
                touchAble = false;
            }
        }

        public override void StepUnDo()
        {
            base.StepUnDo();
            if (touchAble)
                touchAble = true;
        }
		
        public void RegistOnTouch(UnityAction onTouch){
			this.onTouch = onTouch;
			interactableObject.InteractableObjectUsed += OnInteractableObjectUsed;
		}
		
		
	}
}