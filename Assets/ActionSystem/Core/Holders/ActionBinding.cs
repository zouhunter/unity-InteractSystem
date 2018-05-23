using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class ActionBinding : ScriptableObject
    {
        protected virtual void OnStartExecuteInternal(Graph.ActionNode node,bool auto)
        {

        }
        protected virtual void OnBeforeEnd(Graph.ActionNode node, bool force)
        {

        }
        protected virtual void OnUnDoExecuteInternal(Graph.ActionNode node)
        {

        }
    }
}