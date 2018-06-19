using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem
{
    public abstract class Feature
    {
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnDestroy() { }
    }

    public abstract class OperateNodeFeature : Feature
    {
        public static bool log = false;
        public Graph.OperaterNode target { get; set; }
        public virtual void OnStartExecute(bool auto) { }
        public virtual void OnBeforeEnd(bool force) { }
        public virtual void OnUnDoExecute() { }
        public virtual void OnEndExecute(bool force) { }
        public virtual void CoreEndExecute() { }
    }

    public abstract class ActionItemFeature : Feature
    {
        public ActionItem target { get; set; }
        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void StepActive() { }
        public virtual void StepComplete() { }
        public virtual void StepUnDo() { }
    }
}