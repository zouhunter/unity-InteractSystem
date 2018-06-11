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
    public abstract class ClickAbleActionNode : RuntimeCollectNode<ClickAbleActionItem> 
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            currents.Clear();
            finalGroup = null;
        }
      
        protected override void OnAddedToPool(ClickAbleActionItem arg0)
        {
            base.OnAddedToPool(arg0);
            RegistComplete(arg0);
        }

     
        protected override void OnRemovedFromPool(ClickAbleActionItem arg0)
        {
            base.OnRemovedFromPool(arg0);
            RemoveComplete(arg0);            
        }

      
        protected virtual void TryComplete(ClickAbleActionItem item)
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
            if (auto){
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
                foreach (var item in elementPool)
                {
                    Debug.Log(item.Name + ":" + item.OperateAble);
                }
                var elements = elementPool.FindAll(x => x.Name == key && x.OperateAble);
                elements.ForEach(element =>
                {
                    element.StepActive();
                    (element as ClickAbleActionItem).RegistOnCompleteSafety(TryComplete);
                });
            }
        }
        /// <summary>
        /// 自动点击目标元素
        /// </summary>
        protected abstract void AutoCompleteItems();

        /// <summary>
        ///尝试将元素设置为结束状态
        /// </summary>
        /// <param name="undo"></param>
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
        /// <summary>
        /// 注册结束事件（仅元素在本步骤开始后创建时执行注册）
        /// </summary>
        /// <param name="arg0"></param>
        protected void RegistComplete(ClickAbleActionItem arg0)
        {
            if (Statu == ExecuteStatu.Executing)
            {
                if (arg0 is ClickAbleActionItem)
                {
                    // 注册元素结束事件
                    (arg0 as ClickAbleActionItem).RegistOnCompleteSafety(TryComplete);
                }
            }
        }
        /// <summary>
        /// 注销结束事件（仅元素被销毁后执行）
        /// </summary>
        /// <param name="arg0"></param>
        protected void RemoveComplete(ClickAbleActionItem arg0)
        {
            if (arg0 is ClickAbleActionItem)
            {
                (arg0 as ClickAbleActionItem).RemoveOnComplete(TryComplete);
            }
        }


    }
}
