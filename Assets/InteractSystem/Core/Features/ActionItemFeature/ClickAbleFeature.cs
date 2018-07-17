using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem.Actions;
using System;

namespace InteractSystem
{
    [System.Serializable]
    public class ClickAbleFeature : ActionItemFeature
    {
        [SerializeField, Attributes.DefultCollider("碰 撞 体")]
        protected Collider _collider;
        protected string _layerName;
        public virtual Collider collider
        {
            get
            {
                if (_collider == null)
                    collider = target.GetComponentInChildren<Collider>();
                return _collider;
            }
            protected set
            {
                _collider = value;
            }
        }
        [HideInInspector]
        public UnityEvent onClick = new UnityEvent();

        public override void Awake()
        {
            base.Awake();
            InitLayer();
        }

        public void Click()
        {
            if(onClick != null)
            {
                onClick.Invoke();
            }
        }

        private void InitLayer()
        {
            collider.gameObject.layer = LayerMask.NameToLayer(LayerName);
            collider.enabled = false;
        }

        public virtual void Init(ActionItem actionItem, string itemLayer)
        {
           target = actionItem;
           LayerName = itemLayer;
        }

        public virtual string LayerName
        {
            get { return _layerName; }
            set { _layerName = value; }
        }
        public override void StepActive()
        {
            base.StepActive();
            collider.enabled = true;
        }
        public override void StepUnDo()
        {
            base.StepUnDo();
            collider.enabled = false;
        }
        public override void StepComplete()
        {
            base.StepComplete();
            collider.enabled = false;
        }
    }
}