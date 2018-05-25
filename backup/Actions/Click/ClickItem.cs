using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem.Actions
{
    public class ClickItem : ActionItem
    {
        [SerializeField]
        protected int playableCount = 1;
        [SerializeField]
        protected Collider _collider;

        public Collider Collider { get { return _collider; }protected set { _collider = value; } }
        public bool ClickAble { get { return playableCount > targets.Count; } }

        private event UnityAction onClicked;

        protected override void Start()
        {
            base.Start();
            InitLayer();
        }

        private void InitLayer()
        {
            if(!Collider)
            {
                Collider = GetComponentInChildren<Collider>();
            }
            Collider.gameObject.layer = LayerMask.NameToLayer(Layers.clickItemLayer);
        }

        public void RegistOnClick(UnityAction onClicked)
        {
            this.onClicked += onClicked;
        }

        public void OnClick()
        {
            if(onClicked != null)
            {
                onClicked.Invoke();
            }
        }

        public void RemoveOnClicked(UnityAction onClicked)
        {
            this.onClicked -= onClicked;
        }
    }
}