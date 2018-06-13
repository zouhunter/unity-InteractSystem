using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
    public abstract class AutoAppearRule : ScriptableObject
    {
        public abstract void OnCreate(ISupportElement element);
    }
}
