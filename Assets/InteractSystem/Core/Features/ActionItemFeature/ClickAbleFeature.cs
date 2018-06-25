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
        [SerializeField, Attributes.DefultCollider]
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

        public override void Awake()
        {
            base.Awake();
            InitLayer();
        }

        private void InitLayer()
        {
            collider.gameObject.layer = LayerMask.NameToLayer(LayerName);
            collider.enabled = false;
        }

        internal void Init(ActionItem actionItem, string itemLayer)
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