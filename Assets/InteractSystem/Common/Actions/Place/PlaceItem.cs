using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractSystem.Common.Actions
{
    public abstract class PlaceItem:GenericActionItem<PlaceItem>
    {
        public string elementName;
        protected override string LayerName
        {
            get
            {
               return Layers.placePosLayer;
            }
        }
        public override bool OperateAble
        {
            get
            {
                return Active;
            }
        }
        public abstract bool CanPlace(PickUpAbleItem element, out string why);
        public abstract void PlaceObject(PlaceElement pickup);
    }
}