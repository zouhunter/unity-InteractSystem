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
        public const string layer = "i:pickupable";


        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features =base.RegistFeatures();
            pickUpableFeature.Init(this, layer);
            RegistPickupableEvents();
            features.Add(pickUpableFeature);
            return features;
        }

        protected abstract void RegistPickupableEvents();
    }
}