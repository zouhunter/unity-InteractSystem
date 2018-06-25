using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using InteractSystem.Common.Actions;

namespace InteractSystem
{
    [System.Serializable]
    public class CollectNodeFeature : OperateNodeFeature
    {
        [SerializeField,HideInInspector]
        public List<string> itemList = new List<string>();
        public Type type { get; private set; }
        public readonly ElementPool<ISupportElement> elementPool = new ElementPool<ISupportElement>();
        public ISupportElement[] finalGroup { get; set; }
        protected List<ISupportElement> currents = new List<ISupportElement>();
        protected ElementController elementCtrl { get { return ElementController.Instence; } }
        public UnityAction<ISupportElement> onAddToPool { get; set; }
        public UnityAction<ISupportElement> onRemoveFromPool { get; set; }
        public CollectNodeFeature(Type type)
        {
            this.type = type;
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

            elementCtrl.onRegistElememt += OnRegistElement;
            elementCtrl.onRemoveElememt += OnRemoveElement;
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
            if (target.Statu == ExecuteStatu.Executing)
            {
                arg0.StepActive();
            }
            if (onAddToPool != null)
            {
                onAddToPool.Invoke(arg0);
            }
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
                    if(elements != null)
                    {
                        elements = elements.Where((x => SupportType(x.GetType()))).ToList();
                        if (elements != null)
                        {
                            elementPool.ScureAdd(elements.ToArray());
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
        protected virtual void CompleteElements(bool undo)
        {
            foreach (var element in itemList)
            {
                //找到所有被激活的对象
                var active = from item in target.StartedList
                             let f = item.RetriveFeature<CollectNodeFeature>()
                             where f != null
                             where f.itemList.Contains(element)
                             select item;

                if (active == null || active.Count() == 0)
                {
                    var objs = elementPool.FindAll(x => x.Name == element);
                    if (objs == null) return;
                    for (int i = 0; i < objs.Count; i++)
                    {
                        if (log)
                        {
                            if (undo)
                            {
                                Debug.Log("UnDoElements:" + element + objs[i].Active);
                            }
                            else
                            {
                                Debug.Log("CompleteElements:" + element + objs[i].Active);
                            }
                        }

                        if (objs[i].Active)
                        {
                            if (undo)
                            {
                                objs[i].StepUnDo();
                            }
                            else
                            {
                                objs[i].StepComplete();
                            }
                        }
                    }
                }
            }

        }
    }
}