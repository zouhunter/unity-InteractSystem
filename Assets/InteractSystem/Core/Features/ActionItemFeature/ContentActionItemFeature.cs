using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InteractSystem.Actions;
using System;

namespace InteractSystem
{
    [System.Serializable]
    public class ContentActionItemFeature : ActionItemFeature
    {
        [SerializeField]
        protected string elementName;
        protected ElementPool<ActionItem> elementPool = new ElementPool<ActionItem>();
        protected static List<ActionItem> startedList = new List<ActionItem>();
        protected ActionItem element;
        protected ElementController elementCtrl { get { return ElementController.Instence; } }
        public System.Type type { get; private set; }
        public static bool log = false;

        public ActionItem Element { get { return element; } set { element = value; } }
        public string ElementName { get { return elementName; } }

        public ContentActionItemFeature(System.Type type)
        {
            this.type = type;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            elementPool = new ElementPool<ActionItem>();
            elementPool.onAdded = OnAddedToPool;
            elementCtrl.onRegistElememt += OnRegistElement;
            elementCtrl.onRemoveElememt += OnRemoveElement;
        }

        public void Init(ActionItem actionItem)
        {
            target = actionItem;
        }

        /// <summary>
        /// 注册可点击元素
        /// </summary>
        /// <param name="arg0"></param>
        protected void OnRegistElement(ISupportElement arg0)
        {
            if (arg0 is ActionItem)
            {
                var element = arg0 as ActionItem;
                elementPool.ScureAdd(element);
            }
        }

        /// <summary>
        /// 移除可点击元素
        /// </summary>
        /// <param name="arg0"></param>
        protected void OnRemoveElement(ISupportElement arg0)
        {
            if (arg0 is ActionItem)
            {
                var element = arg0 as ActionItem;
                elementPool.ScureRemove(element);
            }
        }


        protected virtual void OnAddedToPool(ActionItem arg0)
        {
            if (target.Active && !arg0.Active && arg0.OperateAble)
            {
                arg0.StepActive();
            }
        }

        /// <summary>
        /// 从场景中找到已经存在的元素
        /// </summary>
        protected void UpdateElementPool()
        {
            var elements = elementCtrl.GetElements<ActionItem>(elementName, false);
            if (elements != null)
            {
                elementPool.ScureAdd(elements.ToArray());
                foreach (var item in elements)
                {
                    if (target.Active && !item.Active && item.OperateAble)
                    {
                        item.StepActive();
                    }
                }
            }
        }
        /// <summary>
        /// 选择性结束element
        /// </summary>
        /// <param name="undo"></param>
        protected virtual void CompleteElements(bool undo)
        {
            var active = from item in startedList
                         let f = item.RetriveFeature<ContentActionItemFeature>()
                         where f != null
                         where f.elementName == elementName
                         select item;

            if (active == null)
            {
                var objs = elementPool.FindAll(x => x.Name == elementName);
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


        public override void StepActive()
        {
            base.StepActive();
            if (!startedList.Contains(this.target))
            {
                startedList.Add(this.target);
            }
            UpdateElementPool();
        }
        public override void StepComplete()
        {
            base.StepComplete();
            if (startedList.Contains(this.target))
            {
                startedList.Remove(this.target);
            }
            CompleteElements(false);
        }
        public override void StepUnDo()
        {
            base.StepUnDo();
            if (startedList.Contains(this.target))
            {
                startedList.Remove(this.target);
            }
            CompleteElements(true);
        }
    }
}