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
        public AnimObj BindingTarget { get; set; }
        public virtual float duration { get; set; }
        public UnityAction onAutoPlayEnd { get; set; }
    }

}
