using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;


namespace InteractSystem
{
    public abstract class ClickAbleActionItem : ActionItem
    {
        [SerializeField,Attributes.DefultCollider]
        protected Collider _collider;
        public Collider Collider { get { return _collider; } protected set { _collider = value; } }
        private List<UnityAction<ClickAbleActionItem>> onCompleteActions = new List<UnityAction<ClickAbleActionItem>>();

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

        public void RegistOnCompleteSafety(UnityAction<ClickAbleActionItem> onClicked)
        {
            if (!onCompleteActions.Contains(onClicked))
            {
                onCompleteActions.Add(onClicked);
            }
        }
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
        public void OnComplete()
        {
            if (onCompleteActions.Count > 0)
            {
                var actions = onCompleteActions.ToArray();
                foreach (var onClicked in actions)
                {
                    onClicked.Invoke(this as ClickAbleActionItem);
                }
            }
        }

        public void RemoveOnComplete(UnityAction<ClickAbleActionItem> onClicked)
        {
            if (onCompleteActions.Contains(onClicked))
            {
                onCompleteActions.Remove(onClicked);
            }
        }
    }
}