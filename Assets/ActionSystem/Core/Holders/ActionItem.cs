using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class ActionItem : MonoBehaviour, ISupportElement
    {
        [SerializeField]
        protected bool startactive = true;
        [SerializeField]
        protected bool endactive = true;
        [SerializeField, Attributes.DefultName]
        protected string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return name;
                }
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public GameObject Body
        {
            get
            {
                return gameObject;
            }
        }
        public bool IsRuntimeCreated { get; set; }
        protected bool _active;
        public virtual bool Active { get { return _active; } protected set { _active = value; } }
        protected List<UnityEngine.Object> targets = new List<UnityEngine.Object>();
        //子类actionItem(用于优先执行)
        protected ActionItem[] subActions;
#if ActionSystem_G
        [HideInInspector]
#endif
        public UnityEvent onActive,onInActive;

        protected virtual void Awake() { }
        protected virtual void OnEnable() {
            targets.Clear();
            subActions = GetComponentsInChildren<ActionItem>();
        }
        protected virtual void Start()
        {
            ElementController.Instence.RegistElement(this);
        }
        protected virtual void OnDestroy()
        {
            ElementController.Instence.RemoveElement(this);
        }
        protected virtual void Update() { }
        protected virtual void OnDisable() { }
        public virtual void SetVisible(bool visible)
        {
            Body.SetActive(visible);
        }
        public virtual void RecordPlayer(UnityEngine.Object target)
        {
            if (!targets.Contains(target))
            {
                this.targets.Add(target);
            }
        }
        public virtual void RemovePlayer(UnityEngine.Object target)
        {
            if (targets.Contains(target))
            {
                this.targets.Remove(target);
            }
        }
        public virtual void StepActive()
        {
            Active = true;
            onActive.Invoke();
        }
        public virtual void StepComplete()
        {
            ElementController.Instence.SetPriority(subActions);
            Active = false;
            onInActive.Invoke();
        }
        public virtual void StepUnDo()
        {
            Active = false;
            onInActive.Invoke();
        }
    }
}