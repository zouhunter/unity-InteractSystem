using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractSystem.Common.Actions
{
    public abstract class PlaceItem:ActionItem
    {
        public Collider Collider { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InitLayer();
        }
        private void InitLayer()
        {
            Collider = GetComponentInChildren<Collider>();
            Collider.gameObject.layer = LayerMask.NameToLayer(Layers.placePosLayer);
            Collider.enabled = false;
        }

        public abstract bool CanPlace(PickUpAbleItem element, out string why);

        public abstract void PlaceObject(PlaceElement pickup);
    }
}