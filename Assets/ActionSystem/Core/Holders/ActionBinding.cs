using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class ActionBinding : ScriptableObject
    {
        protected virtual void OnStartExecuteInternal(Graph.OperateNode node,bool auto)
        {

        }
        protected virtual void OnBeforeEnd(Graph.OperateNode node, bool force)
        {

        }
        protected virtual void OnUnDoExecuteInternal(Graph.OperateNode node)
        {

        }
    }
}