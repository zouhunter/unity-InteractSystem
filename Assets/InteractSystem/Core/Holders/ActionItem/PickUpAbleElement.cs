using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using InteractSystem.Graph;

namespace InteractSystem
{
    public abstract class PickUpAbleItem : ActionItem
    {
        [SerializeField]
        protected PickUpAbleFeature pickUpableFeature = new PickUpAbleFeature();//可拿起
        public bool PickUpAble
        {
            get
            {
                return pickUpableFeature.PickUpAble;
            }
            set
            {
                pickUpableFeature.PickUpAble = value;
            }
        }

        protected override List<ActionItemFeature> RegistFeatures()
        {
            pickUpableFeature.target = this;
            RegistPickupableEvents();
            return new List<ActionItemFeature>() { pickUpableFeature };
        }

        protected abstract void RegistPickupableEvents();
    }
}