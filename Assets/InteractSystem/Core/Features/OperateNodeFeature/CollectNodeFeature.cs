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
            if (target.Statu == ExecuteStatu.Executing && !arg0.Actived && arg0.OperateAble)
            {
                if (autoActive){
                    ActiveElement(arg0);
                }
            }

            if (onAddToPool != null)
                onAddToPool.Invoke(arg0);
        }

        public override void OnStartExecute(bool auto = false)
        {
            UpdateElementPool();
        }
        public override void OnUnDoExecute()
        {
            CompleteElements(true);
        }
        public override void OnBeforeEnd(bool force)
        {
            CompleteElements(false);
        }

        /// <summary>
        /// 从场景中找到已经存在的元素
        /// </summary>
        protected void UpdateElementPool()
        {
            List<string> elementKeys = new List<string>();
            for (int i = 0; i < itemList.Count; i++)
            {
                var elementName = itemList[i];
                if (!elementKeys.Contains(elementName))
                {
                    elementKeys.Add(elementName);
                    var elements = elementCtrl.GetElements<ISupportElement>(elementName, false);
                    if (elements != null)
                    {
                        elements = elements.Where((x => SupportType(x.GetType()))).ToList();
                        
                        foreach (var item in elements)
                        {
                            if(elementPool.Contains(item))
                            {
                                if (target.Statu == ExecuteStatu.Executing && !item.Actived && item.OperateAble)
                                {
                                    if (autoActive)
                                    {
                                        ActiveElement(item);
                                    }
                                }
                            }
                            else
                            {
                                elementPool.ScureAdd(item);
                            }
                        }

                    }
                    else
                    {
                        Debug.Log("have no element name:" + elementName);
                    }

                }
            }
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
        /// <summary>
        /// 选择性结束element
        /// </summary>
        /// <param name="undo"></param>
        protected abstract void CompleteElements(bool undo);

    }
}