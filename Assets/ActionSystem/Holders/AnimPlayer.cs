using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public abstract class AnimPlayer: PickUpAbleElement
    {
        [SerializeField]
        protected bool _reverse;
        [SerializeField]
        protected float _duration;
        public AnimObj BindingTarget { get; set; }
        public virtual float duration { get { return _duration; }set { _duration = value; } }
        public virtual bool reverse { get { return _reverse; } set { _reverse = value; } }
        public UnityAction onAutoPlayEnd { get; set; }
    }

}
