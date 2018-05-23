using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class CommandBinding : ScriptableObject
    {
        public virtual void OnBeforeActionsStart(ActionCommand command) { }
        public virtual void OnBeforeActionsUnDo(ActionCommand command) { }
        public virtual void OnBeforeActionsPlayEnd(ActionCommand command) { }
    }
}