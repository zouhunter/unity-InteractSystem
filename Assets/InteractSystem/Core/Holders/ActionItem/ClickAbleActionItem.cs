using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;


namespace InteractSystem
{
    /// <summary>
    /// 可点击操作对象
    /// </summary>
    public abstract class ClickAbleActionItem : CompleteAbleActionItem
    {
        [SerializeField,Attributes.DefultCollider]
        protected Collider _collider;
        public Collider Collider { get { return _collider; } protected set { _collider = value; } }
        protected override void Awake()
        {
            base.Awake();
            InitLayer();
        }

        private void InitLayer()
        {
            Collider = GetComponentInChildren<Collider>();
            Collider.gameObject.layer = LayerMask.NameToLayer(LayerName);
            Collider.enabled = false;
        }

        protected abstract string LayerName { get; }

     
        public override void StepActive()
        {
            base.StepActive();
            Collider.enabled = true;
        }
        public override void StepUnDo()
        {
            base.StepUnDo();
            Collider.enabled = false;
        }
        public override void StepComplete()
        {
            base.StepComplete();
            Collider.enabled = false;
        }
    }
}