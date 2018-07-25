using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using InteractSystem.Actions;

namespace InteractSystem
{
    [System.Serializable]
    public class RuntimeCollectNodeFeature<T> : OperateNodeFeature where T:ISupportElement
    {
        protected List<string> itemList = new List<string>();
        public readonly ElementPool<T> elementPool = new ElementPool<T>();
        public T[] finalGroup { get; set; }
        protected List<T> currents = new List<T>();
        protected static ElementController elementCtrl { get { return ElementController.Instence; } }
        public UnityAction<T> onAddToPool { get; set; }
        public UnityAction<T> onRemoveFromPool { get; set; }
        public UnityAction<T> onUpdateElement { get; set; }

        public RuntimeCollectNodeFeature(params string[] itemList)
        {
            this.itemList.AddRange(itemList);
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
        protected virtual void OnRemovedFromPool(T arg0)
        {
            if (onRemoveFromPool != null)
            {
                onRemoveFromPool.Invoke(arg0);
            }
        }

        protected virtual void OnAddedToPool(T arg0)
        {
            if (onUpdateElement != null)
                onUpdateElement.Invoke(arg0);

            if (onAddToPool != null)
                onAddToPool.Invoke(arg0);
        }

        public override void OnStartExecute(bool auto = false)
        {
            UpdateElementPool();
        }
        public override void OnUnDoExecute()
        {
            //CompleteElements(true);
        }
        public override void OnBeforeEnd(bool force)
        {
            CompleteElements();
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
                    var elements = elementCtrl.GetElements<T>(elementName, false);
                    if (elements != null)
                    {
                        foreach (var item in elements)
                        {
                            if (elementPool.Contains(item))
                            {
                                if (onUpdateElement != null)
                                {
                                    onUpdateElement.Invoke(item);
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
            if (arg0 is T && itemList.Contains(arg0.Name))
            {
                if (!elementPool.Contains((T)arg0))
                {
                    elementPool.ScureAdd((T)arg0);
                }
            }
        }

        /// <summary>
        /// 移除可点击元素
        /// </summary>
        /// <param name="arg0"></param>
        protected void OnRemoveElement(ISupportElement arg0)
        {
            if (arg0 is T && itemList.Contains(arg0.Name))
            {
                if (elementPool.Contains((T)arg0))
                {
                    elementPool.ScureRemove((T)arg0);
                }
            }
        }

        /// <summary>
        /// 选择性结束element
        /// </summary>
        /// <param name="undo"></param>
        protected virtual void CompleteElements()
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
                            //if (undo)
                            //{
                            //    Debug.Log("UnDoElements:" + element + objs[i].Active);
                            //}
                            //else
                            {
                                Debug.Log("CompleteElements:" + element + objs[i].Active);
                            }
                        }

                        if (objs[i].Active)
                        {
                            //if (undo)
                            //{
                            //    objs[i].UnDo();
                            //}
                            //else
                            {
                                objs[i].SetInActive(target);
                            }
                        }
                    }
                }
            }

        }
    }
}