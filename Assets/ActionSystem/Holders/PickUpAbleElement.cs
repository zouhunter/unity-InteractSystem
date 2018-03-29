using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public abstract class PickUpAbleElement : PickUpAbleItem, ISupportElement
    {
        public virtual GameObject Body
        {
            get
            {
                return gameObject;
            }
        }
        public virtual bool IsRuntimeCreated { get; set; }
        public virtual bool Active { get; set; }
        protected override void Start()
        {
            base.Start();
            ElementController.Instence.RegistElement(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ElementController.Instence.RemoveElement(this);
        }
        public abstract void StepActive();
        public abstract void StepComplete();
        public abstract void StepUnDo();
    }
}