using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    public abstract class DetachRule : ScriptableObject
    {
        public abstract void OnDetach(DetachItem target);

        public abstract void UnDoDetach();
    }
}