using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem.Structure
{
    public abstract class ExecuteState
    {
        public ActionStateMechine stateMechine { get; set; }
        internal abstract void Execute(ExecuteUnit unit);
    }
}