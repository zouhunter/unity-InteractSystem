using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public abstract class RuntimeObj : ActionObj
    {
        protected override void Awake()
        {
            base.Awake();
            elementCtrl.onRegistElememt += OnRegistElement;
            elementCtrl.onRemoveElememt += OnRemoveElement;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            elementCtrl.onRegistElememt -= OnRegistElement;
            elementCtrl.onRemoveElememt -= OnRemoveElement;
        }

        protected abstract void OnRegistElement(ISupportElement arg0);
        protected abstract void OnRemoveElement(ISupportElement arg0);
    }
}