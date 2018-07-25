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
        [SerializeField, Attributes.CustomField("可 交 互")]
        protected bool interactAble = true;
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

        [SerializeField]
        protected UnityEvent onClickEvent;

        public override void Awake()
        {
            base.Awake();
            InitLayer();
        }

        public void Click()
        {
            Debug.Log("click:" + target);
            Debug.Assert(interactAble, target + " can not click!");

            if (onClickEvent != null)
            {
                onClickEvent.Invoke();
            }
        }

        public void RegistOnClick(UnityAction onClick)
        {
            if(onClick != null)
            {
                onClickEvent.AddListener(onClick);
            }
        }
        internal void RemoveOnClick(UnityAction onClick)
        {
            if (onClick != null)
            {
                onClickEvent.RemoveListener(onClick);
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
        public override void SetActive(UnityEngine.Object target)
        {
            base.SetActive(target);
            if (interactAble)
            {
                collider.enabled = true;
            }
        }
        public override void SetInActive(UnityEngine.Object target)
        {
            base.SetInActive(target);
            if (interactAble) {
                collider.enabled = false;
            }
        }
        public override void UnDo(UnityEngine.Object target)
        {
            base.UnDo(target);
            if (interactAble){
                collider.enabled = false;
            }
            onClickEvent.RemoveAllListeners();
        }
      
    }
}