using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace InteractSystem
{
    public abstract class ActionItem : MonoBehaviour, ISupportElement, IActiveAble, IVisiable, ILimitUse
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
                    if (name.EndsWith("(Clone)"))
                    {
                        name = name.Replace("(Clone)", "");
                    }
                    return name;
                }
                return _name;
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
        public virtual bool IsPlaying { get; protected set; }
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

        #region unity3d Api
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
        #endregion

        #region 可视部分显示效果
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
                foreach (var item in actionNotice)
                {
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

        #endregion

        #region 使用限定
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
        public virtual bool HavePlayer(UnityEngine.Object target)
        {
            if (targets.Contains(target))
            { return true; }
            return false;
        }
        #endregion

        #region 状态激活与取消激活
        public void SetActive(UnityEngine.Object target)
        {
            if(this is Actions.InstallItem && target is Actions.InstallItem) Debug.LogError(target);
            if (!lockList.Contains(target))
            {
                lockList.Add(target);
            }

            if (lockList.Count == 1)
            {
                if (log)
                    Debug.Log("StepActive:" + this);

                OnSetActive(target);
            }
        }

        protected virtual void OnSetActive(UnityEngine.Object target)
        {
            gameObject.SetActive(true);
            onActive.Invoke();
            TryExecuteFeatures((feature) => { feature.OnSetActive(target); });
            TryExecuteBindings((binding) => binding.OnActive(this));
        }

        public void SetInActive(UnityEngine.Object target)
        {
            if (lockList.Contains(target))
            {
                lockList.Remove(target);
            }

            //两种条件可以设置元素为激活状态:
            //1.已经没有目标在使用该元素了
            //2.元素已经无法操作并且没有在播放
            if (lockList.Count == 0 || (!OperateAble && !IsPlaying))
            {
                if (log)
                    Debug.Log("SetInActive:" + gameObject);

                OnSetInActive(target);
            }
        }

        protected virtual void OnSetInActive(UnityEngine.Object target)
        {
            onInActive.Invoke();
            ElementController.Instence.SetPriority(subActions);
            TryExecuteFeatures((feature) => { feature.OnSetInActive(target); });
            TryExecuteBindings((binding) => binding.OnInActive(this));
            if (!OperateAble){
                gameObject.SetActive(endactive);
            }
        }
        public virtual void UnDoChanges(UnityEngine.Object target)
        {
            IsPlaying = false;
            if (log){
                Debug.LogFormat("StepUnDo from {0} :{1}", target, gameObject, gameObject);
            }

            if (lockList.Contains(target))
                lockList.Remove(target);

            if (lockList.Count == 0)
            {
                TryExecuteBindings((binding) => binding.OnInActive(this));
                gameObject.SetActive(startactive);
            }

            TryExecuteFeatures((feature) => { feature.OnUnDo(target); });
        }
        #endregion

        #region 同步执行的
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

        protected virtual void OnAddActionItemBinding(Binding.ActionItemBinding binding)
        {
            bindings.Add(Instantiate(binding));
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
        #endregion

        #region 属性注册与执行
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
            if (log) Debug.Log("regist features" + this, gameObject);
            return new List<ActionItemFeature>(Config.Instence.actionItemFeatures);
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
        #endregion
    }
}