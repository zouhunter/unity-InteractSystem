using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractSystem
{
    public abstract class CompleteAbleContentActionItem<S> : ClickAbleCompleteAbleActionItem where S: CompleteAbleActionItem
    {
        [SerializeField]
        protected string elementName;
        protected ElementPool<S> elementPool = new ElementPool<S>();
        protected static List<ActionItem> startedList = new List<ActionItem>();
        protected S element;
        protected ElementController elementCtrl { get { return ElementController.Instence; } }

        protected override void OnEnable()
        {
            base.OnEnable();
            elementPool.onAdded = OnAddedToPool;
            elementPool.onRemoved = OnRemovedFromPool;

            elementCtrl.onRegistElememt += OnRegistElement;
            elementCtrl.onRemoveElememt += OnRemoveElement;
        }
        /// <summary>
        /// 注册可点击元素
        /// </summary>
        /// <param name="arg0"></param>
        protected void OnRegistElement(ISupportElement arg0)
        {
            if (arg0 is S)
            {
                var element = arg0 as S;
                elementPool.ScureAdd(element);
            }
        }

        /// <summary>
        /// 移除可点击元素
        /// </summary>
        /// <param name="arg0"></param>
        protected void OnRemoveElement(ISupportElement arg0)
        {
            if (arg0 is S)
            {
                var element = arg0 as S;
                elementPool.ScureRemove(element);
            }
        }


        protected virtual void OnRemovedFromPool(S arg0)
        {

        }

        protected virtual void OnAddedToPool(S arg0)
        {
            if (Active)
            {
                arg0.StepActive();
            }
        }

        /// <summary>
        /// 从场景中找到已经存在的元素
        /// </summary>
        protected void UpdateElementPool()
        {
            var elements = elementCtrl.GetElements<S>(elementName, false);
            if (elements != null)
            {
                elementPool.ScureAdd(elements.ToArray());
                foreach (var item in elements)
                {
                    if(!item.Active && item.OperateAble)
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
            //找到所有被激活的对象
            var active = startedList.Find(x => x is CompleteAbleContentActionItem<S> && (x as CompleteAbleContentActionItem<S>).elementName == elementName);

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
            if (!startedList.Contains(this)){
                startedList.Add(this);
            }
            UpdateElementPool();
        }
        public override void StepComplete()
        {
            base.StepComplete();
            if (startedList.Contains(this))
            {
                startedList.Remove(this);
            }
            CompleteElements(false);
        }
        public override void StepUnDo()
        {
            base.StepUnDo();
            if (startedList.Contains(this))
            {
                startedList.Remove(this);
            }
            CompleteElements(true);
        }
    }

}
