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
        public float duration { get; set; }
        public UnityAction onAutoPlayEnd { get; set; }
    }

}
