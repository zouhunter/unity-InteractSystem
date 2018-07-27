using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

namespace InteractSystem
{
    [System.Serializable]
    public abstract class CollectNodeFeature : OperateNodeFeature
    {
        [SerializeField, HideInInspector]
        public List<string> itemList = new List<string>();
        public Type type { get; private set; }
        public readonly ElementPool<ISupportElement> elementPool = new ElementPool<ISupportElement>();
        public ISupportElement[] finalGroup { get; set; }
        protected static ElementController elementCtrl { get { return ElementController.Instence; } }
        public event UnityAction<ISupportElement> onAddToPool;
        public event UnityAction<ISupportElement> onRemoveFromPool;
        protected bool autoActive;
        protected bool autoExecute;
        public CollectNodeFeature(Type type, bool autoActive = true)
        {
            this.type = type;
            this.autoActive = autoActive;
            elementCtrl.onRegistElememt += OnRegistElement;
            elementCtrl.onRemoveElememt += OnRemoveElement;
        }

        public virtual void SetTarget(Graph.OperaterNode node)
        {
            target = node;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            elementPool.onAdded = OnAddedToPool;
            elementPool.onRemoved = OnRemovedFromPool;
        }
        protected virtual void OnRemovedFromPool(ISupportElement arg0)
        {
            if (onRemoveFromPool != null && SupportType(arg0.GetType()))
            {
                onRemoveFromPool.Invoke(arg0);
            }
        }

        protected virtual void OnAddedToPool(ISupportElement arg0)
        {
            if(arg0 is IActiveAble)
            {
                var activeAble = arg0 as IActiveAble;
                if (target.Statu == ExecuteStatu.Executing && !activeAble.Actived &&activeAble.OperateAble)
                {
                    if (autoActive)
                    {
                        ActiveElement(activeAble);
                    }
                }
            }
           

            if (onAddToPool != null)
                onAddToPool.Invoke(arg0);
        }

        public override void OnStartExecute(bool auto = false)
        {
            ActiveElements();
            autoExecute = auto;
        }

        protected abstract void ActiveElements();
        public override void OnUnDoExecute()
        {
            UnDoActivedElement();
        }
        public override void OnBeforeEnd(bool force)
        {
            InActivedElements();
        }

        /// <summary>
        /// 注册可点击元素
        /// </summary>
        /// <param name="arg0"></param>
        protected void OnRegistElement(ISupportElement arg0)
        {
            if (SupportType(arg0.GetType()) && itemList.Contains(arg0.Name))
            {
                if (!elementPool.Contains(arg0))
                {
                    elementPool.ScureAdd(arg0);
                }
            }
        }

        /// <summary>
        /// 移除可点击元素
        /// </summary>
        /// <param name="arg0"></param>
        protected void OnRemoveElement(ISupportElement arg0)
        {
            if (SupportType(arg0.GetType()) && itemList.Contains(arg0.Name))
            {
                if (elementPool.Contains(arg0))
                {
                    elementPool.ScureRemove(arg0);
                }
            }
        }

        protected bool SupportType(Type targetType)
        {
            return this.type == null ||
                type.IsAssignableFrom(targetType) ||
                targetType.IsSubclassOf(type) ||
                type == targetType;
        }
        protected void ForEachElement(UnityAction<ISupportElement> onFind)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                var element = itemList[i];
                var objs = elementPool.FindAll(x => x.Name == element);
                Debug.Assert(objs != null, "no element name:" + element);
                foreach (var item in objs)
                {
                    onFind.Invoke(item);
                }
            }
        }
        protected abstract void UnDoActivedElement();
        protected abstract void InActivedElements();

    }
}