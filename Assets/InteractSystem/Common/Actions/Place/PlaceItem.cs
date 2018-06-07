using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractSystem.Common.Actions
{
    public class PlaceItem:ActionItem
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

    }
}