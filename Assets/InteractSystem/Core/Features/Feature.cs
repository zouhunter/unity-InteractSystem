using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace InteractSystem
{
    public abstract class Feature
    {
        public static bool log = false;
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnDestroy() { }
        protected abstract UnityEngine.Object Target { get; }
        public event UnityAction<ISupportElement> onActiveElement;
        public event UnityAction<ISupportElement> onUnDoElement;
        public event UnityAction<ISupportElement> onInActiveElement;

        protected void ActiveElement(ISupportElement element)
        {
            element.SetActive(Target);
            if (onActiveElement != null)
                onActiveElement.Invoke(element);
        }

        protected void UndoElement(ISupportElement element)
        {
            element.UnDoChanges(Target);
            if (onUnDoElement != null)
                onUnDoElement.Invoke(element);
        }

        protected void SetInActiveElement(ISupportElement element)
        {
            element.SetInActive(Target);
            if (onInActiveElement != null)
                onInActiveElement.Invoke(element);
        }
    }

    public abstract class OperateNodeFeature : Feature
    {
        protected override UnityEngine.Object Target
        {
            get
            {
                return target;
            }
        }
        public Graph.OperaterNode target { get; protected set; }
        public virtual void OnStartExecute(bool auto) { }
        public virtual void OnBeforeEnd(bool force) { }
        public virtual void OnUnDoExecute() { }
        public virtual void OnEndExecute(bool force) { }
        public virtual void CoreEndExecute() { }

    }

    public abstract class ActionItemFeature : Feature
    {
        protected override UnityEngine.Object Target
        {
            get
            {
                return target;
            }
        }
        public ActionItem target { get; protected set; }
        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void OnSetActive(UnityEngine.Object target) { }
        public virtual void OnSetInActive(UnityEngine.Object target) { }
        public virtual void OnUnDo(UnityEngine.Object target) { }
    }
}