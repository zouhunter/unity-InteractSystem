using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace InteractSystem
{
    public abstract class ActionItem : MonoBehaviour, ISupportElement
    {
        [SerializeField, Attributes.DefultName]
        protected string _name;
        [SerializeField]
        protected bool startactive = true;
        [SerializeField]
        protected bool endactive = true;
        [SerializeField]
        private List<Binding.ActionItemBinding> bindings = new List<Binding.ActionItemBinding>();

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
        public abstract bool OperateAble { get; }
        public virtual bool Active { get { return _active; } protected set { _active = value; } }
        protected bool _active;
        protected List<UnityEngine.Object> targets = new List<UnityEngine.Object>();
        //子类actionItem(用于优先执行)
        protected ActionItem[] subActions;
        protected List<ActionItemFeature> actionItemFeatures;
#if ActionSystem_G
        [HideInInspector]
#endif
        public UnityEvent onActive, onInActive;
        public static bool log = false;

        protected virtual void Awake()
        {
            actionItemFeatures = RegistFeatures();
            TryExecuteFeatures((feature) => { feature.Awake(); });
        }

     
        protected virtual void Start()
        {
            ElementController.Instence.RegistElement(this);
            InitBindingScripts();
            TryExecuteFeatures((feature) => { feature.Start(); });
            TryExecuteBindings((binding) => binding.Start());
            gameObject.SetActive(startactive);
        }
        protected virtual void OnEnable()
        {
            targets.Clear();
            subActions = GetComponentsInChildren<ActionItem>().Where(x => x != this).ToArray();
            TryExecuteFeatures((feature) => { feature.OnEnable(); });
        }
        protected virtual void OnDestroy()
        {
            ElementController.Instence.RemoveElement(this);
            TryExecuteFeatures((feature) => { feature.OnDestroy(); });
        }
        protected virtual void Update()
        {
            TryExecuteFeatures((feature) => { feature.Update(); });
            TryExecuteBindings((binding) => binding.Update());
        }
        protected virtual void OnDisable()
        {
            TryExecuteFeatures((feature) => { feature.OnDisable(); });
        }

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
            gameObject.SetActive(true);
            Active = true;
            onActive.Invoke();
            TryExecuteFeatures((feature) => { feature.StepActive(); });
            TryExecuteBindings((binding) => binding.OnActive(this));
        }
        public virtual void StepComplete()
        {
            Active = false;
            onInActive.Invoke();
            ElementController.Instence.SetPriority(subActions);
            TryExecuteFeatures((feature) => { feature.StepComplete(); });
            TryExecuteBindings((binding) => binding.OnInActive(this));
            gameObject.SetActive(endactive);
        }
        public virtual void StepUnDo()
        {
            Active = false;
            onInActive.Invoke();
            TryExecuteFeatures((feature) => { feature.StepUnDo(); });
            TryExecuteBindings((binding) => binding.OnInActive(this));
            gameObject.SetActive(startactive);
        }

        protected virtual void InitBindingScripts()
        {
            if (Config.actionItemBindings != null)
            {
                foreach (var item in Config.actionItemBindings)
                {
                    var instence = Instantiate(item);
                    bindings.Add(instence);
                }
            }
        }

        public T RetriveFeature<T>() where T : ActionItemFeature
        {
            if (actionItemFeatures == null)
            {
                return null;
            }
            else
            {
                return actionItemFeatures.Find(x => x is T) as T;
            }
        }

        protected virtual List<ActionItemFeature> RegistFeatures() { return null; }

        protected void TryExecuteFeatures(UnityAction<ActionItemFeature> featureAction)
        {
            if (actionItemFeatures != null && featureAction != null)
            {
                actionItemFeatures.ForEach(feature =>
                {
                    featureAction(feature);
                });
            }
        }
        protected void TryExecuteBindings(UnityAction<Binding.ActionItemBinding> bindingAction)
        {
            if (bindings != null && bindingAction != null)
            {
                bindings.ForEach(binding =>
                {
                    bindingAction(binding);
                });
            }
        }
    }
}