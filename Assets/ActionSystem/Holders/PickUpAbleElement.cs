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
            Debug.Log("Start" + this);
            ElementController.Instence.RegistElement(this);
            base.Start();
        }
        protected override void OnDestroy()
        {
            Debug.Log("OnDestroy" + this);
            ElementController.Instence.RemoveElement(this);
            base.OnDestroy();
        }
        public abstract void StepActive();
        public abstract void StepComplete();
        public abstract void StepUnDo();
        public abstract void SetVisible(bool visible);
    }
}