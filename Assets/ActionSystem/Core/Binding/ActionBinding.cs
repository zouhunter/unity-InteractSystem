using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem.Binding
{
    public class ActionBinding : ScriptableObject
    {
        public virtual void OnStartExecuteInternal(Graph.OperateNode node,bool auto)
        {

        }
        public virtual void OnBeforeEnd(Graph.OperateNode node, bool force)
        {

        }
        public virtual void OnUnDoExecuteInternal(Graph.OperateNode node)
        {

        }
    }
}