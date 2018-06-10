using UnityEngine;
using System.Collections.Generic;
using InteractSystem.Graph;
using System;
using System.Linq;

namespace InteractSystem
{
    /// <summary>
    ///顺序执行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericActionNode<T> : RuntimeCollectNode<T> where T : ActionItem, ISupportElement
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            currents.Clear();
            finalGroup = null;
        }

        protected override void OnAddedToPool(T arg0)
        {
            base.OnAddedToPool(arg0);

            if (Statu == ExecuteStatu.Executing)
            {
                if(arg0 is GenericActionItem<T>)
                {
                    (arg0 as GenericActionItem<T>).RegistOnComplete(TryComplete);
                }
            }
        }
        protected override void OnRemovedFromPool(T arg0)
        {
            base.OnRemovedFromPool(arg0);
            if (arg0 is GenericActionItem<T>)
            {
                (arg0 as GenericActionItem<T>).RemoveOnComplete(TryComplete);
            }
        }
        protected virtual void TryComplete(T item)
        {
            if (statu != ExecuteStatu.Executing) return;//没有执行
            if (!item.OperateAble) return;//目标无法点击
            if (currents.Count >= itemList.Count) return;//超过需要

            if (itemList[currents.Count] == item.Name)
            {
                currents.Add(item);
                item.RecordPlayer(this);
            }

            if (currents.Count >= itemList.Count)
            {
                finalGroup = currents.ToArray();
                OnEndExecute(false);
            }
        }


        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            FindOperateAbleItems();
            if (auto)
            {
                AutoCompleteItems();
            }
        }

        /// <summary>
        /// 将所能点击的目标设置为激活状态
        /// </summary>
        private void FindOperateAbleItems()
        {
            if (itemList.Count > currents.Count)
            {
                var key = itemList[currents.Count];
                var elements = elementPool.FindAll(x => x.Name == key && x.OperateAble);
                elements.ForEach(element =>
                {
                    element.StepActive();
                    (element as GenericActionItem<T>).RegistOnComplete(TryComplete);
                });
            }
        }
        /// <summary>
        /// 自动点击目标元素
        /// </summary>
        protected abstract void AutoCompleteItems();

        protected override void CompleteElements(bool undo)
        {
            base.CompleteElements(undo);

            if (undo)
            {
                foreach (var item in currents)
                {
                    item.StepUnDo();
                    item.RemovePlayer(this);
                }
                currents.Clear();
            }
            else
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (currents.Count <= i)
                    {
                        var element = elementPool.Find(x => x.Name == itemList[i] && x.OperateAble);
                        if (element != null)
                        {
                            element.RecordPlayer(this);
                            element.StepComplete();
                            currents.Add(element);
                        }
                        else
                        {
                            Debug.LogError("缺少：" + itemList[i]);
                        }
                    }
                    else
                    {
                        var item = currents[i];
                        if (item.Active)
                        {
                            currents[i].StepComplete();
                        }
                        currents[i].RecordPlayer(this);
                    }
                }
            }
        }


    }
}
