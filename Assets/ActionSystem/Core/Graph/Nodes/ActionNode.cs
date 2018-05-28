using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;

namespace WorldActionSystem.Graph
{
    public abstract class ActionNode : Node
    {
        protected virtual void OnEnable() { }
        protected virtual void OnDestroy() { }
    }
}