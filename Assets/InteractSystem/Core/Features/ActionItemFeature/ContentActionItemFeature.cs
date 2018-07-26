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
        [SerializeField,Attributes.CustomField("接收元素")]
        protected string elementName;
        protected ElementPool<ActionItem> elementPool = new ElementPool<ActionItem>();
        protected static List<ActionItem> startedList = new List<ActionItem>();
        protected ActionItem element;
        protected ElementController elementCtrl { get { return ElementController.Instence; } }
        public System.Type type { get; private set; }

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
            if (target.Actived  && arg0.OperateAble)
            {
                arg0.SetActive(target);
            }
        }
        protected void ForEachElement(UnityAction<ISupportElement> onFind)
        {
            var objs = elementPool.FindAll(x => x.Name == elementName);
            Debug.Assert(objs != null, "no element name:" + elementName);
            foreach (var item in objs)
            {
                onFind.Invoke(item);
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

                foreach (var arg0 in elements)
                {
                    if (target.Actived &&  arg0.OperateAble)
                    {
                        ActiveElement(arg0);
                    }
                }
              
            }
        }
        protected void UnDoActivedElement()
        {
            ForEachElement((element) => {
                UndoElement(element);
            });
        }

        protected void InActivedElements()
        {
            ForEachElement((element) => {
                SetInActiveElement(element);
            });
        }

        public override void OnSetActive(UnityEngine.Object target)
        {
            base.OnSetActive(target);
            if (!startedList.Contains(this.target))
            {
                startedList.Add(this.target);
            }
            UpdateElementPool();
        }
        public override void OnSetInActive(UnityEngine.Object target)
        {
            base.OnSetInActive(target);
            if (startedList.Contains(this.target)){
                startedList.Remove(this.target);
            }
            InActivedElements();
        }
        public override void OnUnDo(UnityEngine.Object target)
        {
            base.OnUnDo(target);
            if (startedList.Contains(this.target)) {
                startedList.Remove(this.target);
            }
            UnDoActivedElement();
        }
    }
}