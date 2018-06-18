using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem
{
    [System.Serializable]
    public class ClickAbleFeature : ActionItemFeature
    {
        [SerializeField, Attributes.DefultCollider]
        protected Collider _collider;

        public Collider Collider
        {
            get
            {
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
            if (_collider == null)
                Collider = target.GetComponentInChildren<Collider>();
            Collider.gameObject.layer = LayerMask.NameToLayer(LayerName);
            Collider.enabled = false;
        }

        public string LayerName { get; set; }


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