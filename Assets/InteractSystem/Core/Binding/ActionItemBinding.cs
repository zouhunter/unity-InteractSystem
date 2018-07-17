using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Binding
{
    public class ActionItemBinding : ScriptableObject
    {
        public static bool log;
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void OnActive(ActionItem target) { }
        public virtual void OnInActive(ActionItem target) { }
    }
}