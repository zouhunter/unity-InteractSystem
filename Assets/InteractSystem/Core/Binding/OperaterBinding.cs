using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Binding
{
    public class OperaterBinding : ScriptableObject
    {
        public virtual void OnStartExecuteInternal(Graph.OperaterNode node,bool auto)
        {

        }
        public virtual void OnBeforeEnd(Graph.OperaterNode node, bool force)
        {

        }
        public virtual void OnUnDoExecuteInternal(Graph.OperaterNode node)
        {

        }
    }
}