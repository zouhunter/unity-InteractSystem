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
        [SerializeField, Attributes.DefultName("关键字")]
        protected string _name;
        [SerializeField]
        [Attributes.CustomField("显示（初始）")]
        protected bool startactive = true;
        [SerializeField]
        [Attributes.CustomField("显示（禁用）")]
        protected bool endactive = true;
        [SerializeField]
        protected List<Binding.ActionItemBinding> bindings = new List<Binding.ActionItemBinding>();
        [SerializeField]
        protected List<Notice.ActionNotice> actionNotice = new List<InteractSystem.Notice.ActionNotice>();

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
        protected UnityEngine.Object firstLock { get { return lockList.Count > 0 ? lockList[0] : null; } }
        public bool Actived { get { return lockList.Count > 0; } }
        private bool _active;
        //临时激活对象列表
        protected List<UnityEngine.Object> lockList = new List<UnityEngine.Object>();
        //关联使用者
        protected List<UnityEngine.Object> targets = new List<UnityEngine.Object>();
        //子类actionItem(用于优先执行)
        protected ActionItem[] subActions;
        protected List<ActionItemFeature> actionItemFeatures;

        [HideInInspector]//激活及关闭事件
        public UnityEvent onActive, onInActive;
        public static bool log = false;

        protected virtual void Awake()
        {
            actionItemFeatures = RegistFeatures();
            InitBindingScripts();
            Config.Instence.onAddActionItemBinding += OnAddActionItemBinding;
            InitActionNoticeScripts();
            TryExecuteFeatures((feature) => { feature.Awake(); });
            ElementController.Instence.RegistElement(this);
        }

        protected virtual void Start()
        {
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
            TryUpdateNotice();
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

        public virtual void Notice(Transform target)
        {
            if (Config.Instence.actionItemNotice)
            {
                foreach (var item in actionNotice)
                {
                    item.Notice(target);
                }
            }
        }

        public virtual void TryUpdateNotice()
        {
            if (Config.Instence.actionItemNotice)
            {
                foreach (var item in actionNotice){
                    item.Update();
                }
            }
        }

        public virtual void UnNotice(Transform target)
        {
            if (Config.Instence.actionItemNotice)
            {
                foreach (var item in actionNotice)
                {
                    item.UnNotice(target);
                }
            }
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

        public void SetActive(UnityEngine.Object target)
        {
            if(!lockList.Contains(target))
                lockList.Add(target);

            if (lockList.Count == 1)
            {
                if (log)
                    Debug.Log("StepActive:" + this);
              
                OnSetActive(target);
            }
            else
            {
                Debug.LogWarning("allreadly actived:" + this, gameObject);
            }
        }

        protected virtual void OnSetActive(UnityEngine.Object target) {
            gameObject.SetActive(true);
            onActive.Invoke();
            TryExecuteFeatures((feature) => { feature.OnSetActive(target); });
            TryExecuteBindings((binding) => binding.OnActive(this));
        }

        public void SetInActive(UnityEngine.Object target)
        {
            if (lockList.Contains(target))
                lockList.Remove(target);

            if (lockList.Count == 0)
            {
                if (log)
                    Debug.Log("SetInActive:" + gameObject);
              
                OnSetInActive(target);
            }
            else
            {
                Debug.LogWarning("can`t inactived:" + this, gameObject);
            }
        }

        protected virtual void OnSetInActive(UnityEngine.Object target)
        {
            onInActive.Invoke();
            ElementController.Instence.SetPriority(subActions);
            TryExecuteFeatures((feature) => { feature.OnSetInActive(target); });
            TryExecuteBindings((binding) => binding.OnInActive(this));
            UpdateState();
        }

        public virtual void UnDoChanges(UnityEngine.Object target)
        {
            if (log){
                Debug.LogFormat("StepUnDo from {0} :{1}",target,gameObject, gameObject);
            }

            if (lockList.Contains(target))
                lockList.Remove(target);

            if (lockList.Count == 0)
            {
                TryExecuteBindings((binding) => binding.OnInActive(this));
                UpdateState();
            }
            TryExecuteFeatures((feature) => { feature.OnUnDo(target); });
        }

        protected virtual void InitBindingScripts()
        {
            var clampedBindings = new List<Binding.ActionItemBinding>();
            foreach (var item in bindings)
            {
                clampedBindings.Add(Instantiate(item));
            }
            if (Config.Instence.actionItemBindings != null)
            {
                foreach (var item in Config.Instence.actionItemBindings)
                {
                    var instence = Instantiate(item);
                    clampedBindings.Add(instence);
                }
            }
            bindings = clampedBindings;
        }

        private void InitActionNoticeScripts()
        {
            var clampedNotices = new List<Notice.ActionNotice>();
            foreach (var item in actionNotice)
            {
                clampedNotices.Add(Instantiate(item));
            }
            if (Config.Instence.actionNotices != null)
            {
                foreach (var item in Config.Instence.actionNotices)
                {
                    var instence = Instantiate(item);
                    clampedNotices.Add(instence);
                }
            }
            actionNotice = clampedNotices;
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

        protected virtual List<ActionItemFeature> RegistFeatures()
        {
            if(log) Debug.Log("regist features" + this,gameObject);
            return new List<ActionItemFeature>(Config.Instence.actionItemFeatures);
        }

        protected virtual void OnAddActionItemBinding(Binding.ActionItemBinding binding)
        {
            bindings.Add(Instantiate(binding));
        }

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

        private void UpdateState()
        {
            if (OperateAble)
            {
                gameObject.SetActive(startactive);
            }
            else
            {
                gameObject.SetActive(endactive);
            }
        }
    }
}