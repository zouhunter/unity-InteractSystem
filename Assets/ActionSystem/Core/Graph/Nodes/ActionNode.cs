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
        protected bool _completed;
        protected bool _started;
        public virtual bool Completed { get { return _completed; } protected set { _completed = value; } }
        public virtual bool Started { get { return _started; } protected set { _started = value; } }

        protected virtual void OnEnable() { }
        protected virtual void OnDestroy() { }
    }
}