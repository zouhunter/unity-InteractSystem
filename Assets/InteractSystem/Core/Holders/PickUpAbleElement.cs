//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Collections;
//using System.Collections.Generic;
//namespace InteractSystem
//{
//    [RequireComponent(typeof(PickUpAbleItem))]
//    public abstract class PickUpAbleElement : ActionItem
//    {
//        protected override void Start()
//        {
//            base.Start();
//            ElementController.Instence.RegistElement(this);
//        }
//        protected virtual void OnDestroy()
//        {

//            ElementController.Instence.RemoveElement(this);
//        }
//        public abstract void StepActive();
//        public abstract void StepComplete();
//        public abstract void StepUnDo();
//        public abstract void SetVisible(bool visible);
//    }
//}