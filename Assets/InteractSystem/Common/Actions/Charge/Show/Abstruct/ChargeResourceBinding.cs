using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
    [RequireComponent(typeof(ChargeResource))]
    public abstract class ChargeResourceBinding : ChargeBinding
    {
        protected ChargeResource target;
        protected virtual void Awake()
        {
            target = GetComponent<ChargeResource>();
            target.onChange = OnCharge;
        }
        protected abstract void OnCharge(Vector3 center, ChargeData data, UnityAction onComplete);
    }
}