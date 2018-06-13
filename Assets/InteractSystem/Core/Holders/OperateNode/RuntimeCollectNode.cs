using UnityEngine;
using System.Collections.Generic;
using InteractSystem.Graph;

namespace InteractSystem
{
    /// <summary>
    /// 【动态执行节点】
    /// 收集指定元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RuntimeCollectNode<T> : OperaterNode where T : ISupportElement
    {
        [SerializeField, HideInInspector]
        protected List<string> itemList = new List<string>();

        protected ElementPool<T> elementPool = new ElementPool<T>();
        protected T[] finalGroup { get; set; }
        protected List<T> currents = new List<T>();
        protected ElementController elementCtrl { get { return ElementController.Instence; } }

        protected override void OnEnable()
        {
            base.OnEnable();
            elementPool.onAdded = OnAddedToPool;
            elementPool.onRemoved = OnRemovedFromPool;
            
            elementCtrl.onRegistElememt += OnRegistElement;
            elementCtrl.onRemoveElememt += OnRemoveElement;
        }
        protected virtual void OnRemovedFromPool(T arg0)
        {

        }

        protected virtual void OnAddedToPool(T arg0)
        {
            if (statu == ExecuteStatu.Executing)
            {
                arg0.StepActive();
            }
        }

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            UpdateElementPool();
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            CompleteElements(true);
        }
        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);
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
                    var elements = elementCtrl.GetElements<T>(elementName,false);
                    if (elements != null)
                    {
                        elementPool.ScureAdd(elements.ToArray());
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
            //LinkObj
            if (arg0 is T && itemList.Contains(arg0.Name))
            {
                var element = (T)arg0;
                if (!elementPool.Contains(element))
                {
                    elementPool.ScureAdd(element);
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
                var element = (T)arg0;
                if (elementPool.Contains(element))
                {
                    elementPool.ScureRemove(element);
                }
            }
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
                var active = startedList.Find(x => x is RuntimeCollectNode<T> && (x as RuntimeCollectNode<T>).itemList.Contains(element));

                if (active == null)
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
