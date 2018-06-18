using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace InteractSystem
{
    [System.Serializable]
    public class CollectNodeFeature : OperateNodeFeature
    {
        [SerializeField]
        protected List<string> itemList = new List<string>();
        public Type type { get; private set; }
        protected ElementPool<ActionItem> elementPool = new ElementPool<ActionItem>();
        protected ActionItem[] finalGroup { get; set; }
        protected List<ActionItem> currents = new List<ActionItem>();
        protected ElementController elementCtrl { get { return ElementController.Instence; } }
        public static bool log = false;
        public CollectNodeFeature(Type type)
        {
            this.type = type;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            elementPool.onAdded = OnAddedToPool;
            elementPool.onRemoved = OnRemovedFromPool;

            elementCtrl.onRegistElememt += OnRegistElement;
            elementCtrl.onRemoveElememt += OnRemoveElement;
        }
        protected virtual void OnRemovedFromPool(ActionItem arg0)
        {
           
        }

        protected virtual void OnAddedToPool(ActionItem arg0)
        {
            if (target.Statu == ExecuteStatu.Executing)
            {
                arg0.StepActive();
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
                    var elements = elementCtrl.GetElements<ActionItem>(elementName, false);
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
            if (arg0.GetType().IsSubclassOf(type)&& arg0 is ActionItem && itemList.Contains(arg0.Name))
            {
                var element = arg0 as ActionItem;
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
            if (arg0.GetType().IsSubclassOf(type) && itemList.Contains(arg0.Name))
            {
                var element = arg0 as ActionItem;
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