using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem
{
    [System.Serializable]
    public class CompleteAbleCollectNodeFeature: CollectNodeFeature
    {
        public CompleteAbleCollectNodeFeature(System.Type type) : base(type) { }

        public override void OnEnable()
        {
            base.OnEnable();
            currents.Clear();
            finalGroup = null;
        }

        protected override void OnAddedToPool(ActionItem arg0)
        {
            base.OnAddedToPool(arg0);
            RegistComplete(arg0);
        }


        protected override void OnRemovedFromPool(ActionItem arg0)
        {
            base.OnRemovedFromPool(arg0);
            RemoveComplete(arg0);
        }

        protected void TryAutoComplete(int index)
        {
            if (index < itemList.Count)
            {
                var key = itemList[index];
                var item = elementPool.Find(x => x.Name == key && x.Active && x.OperateAble && x is ActionItem) as ActionItem;
                if (item != null)
                {
                    var completeFeature = item.RetriveFeature<CompleteAbleItemFeature>();
                    if(completeFeature != null)
                    {
                        completeFeature.RegistOnCompleteSafety(OnAutoComplete);
                        completeFeature.AutoExecute();
                    }
                }
                else
                {
                    Debug.LogError("have no active useful element Name:" + key);
                }
            }
        }

        private void OnAutoComplete(CompleteAbleItemFeature arg0)
        {
            arg0.RemoveOnComplete(OnAutoComplete);
            TryAutoComplete(currents.Count);
        }

        protected virtual void TryComplete(CompleteAbleItemFeature item)
        {
            if (target.Statu != ExecuteStatu.Executing) return;//没有执行
            if (!item.target.OperateAble) return;//目标无法点击
            if (currents.Count >= itemList.Count) return;//超过需要

            if (itemList[currents.Count] == item.target.Name)
            {
                currents.Add(item.target as ActionItem);
                item.target.RecordPlayer(target);
                item.target.StepComplete();
            }

            if (currents.Count >= itemList.Count)
            {
                finalGroup = currents.ToArray();
                OnEndExecute(false);
            }
            else
            {
                FindOperateAbleItems();
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
                foreach (var item in elementPool)
                {
                    Debug.Log(item.Name + ":" + item.OperateAble);
                }
                var elements = elementPool.FindAll(x => x.Name == key && x.OperateAble);
                elements.ForEach(element =>
                {
                    element.StepActive();
                    var feature = element.RetriveFeature<CompleteAbleItemFeature>();
                    if(feature!= null)
                    {
                        feature.RegistOnCompleteSafety(TryComplete);
                    }
                });
            }
        }
        /// <summary>
        /// 自动点击目标元素
        /// </summary>
        protected virtual void AutoCompleteItems()
        {
            TryAutoComplete(0);
        }

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
                    item.RemovePlayer(target);
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
                            element.RecordPlayer(target);
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
                        currents[i].RecordPlayer(target);
                    }
                }
            }
        }
        /// <summary>
        /// 注册结束事件（仅元素在本步骤开始后创建时执行注册）
        /// </summary>
        /// <param name="arg0"></param>
        protected void RegistComplete(ActionItem arg0)
        {
            if (target. Statu == ExecuteStatu.Executing)
            {
                if (arg0 is ActionItem)
                {
                    // 注册元素结束事件
                    var feature = arg0.RetriveFeature<CompleteAbleItemFeature>();
                    feature.RegistOnCompleteSafety(TryComplete);
                }
            }
        }
        /// <summary>
        /// 注销结束事件（仅元素被销毁后执行）
        /// </summary>
        /// <param name="arg0"></param>
        protected void RemoveComplete(ActionItem arg0)
        {
            if (arg0 is ActionItem)
            {
                var feature = arg0.RetriveFeature<CompleteAbleItemFeature>();
                feature.RemoveOnComplete(TryComplete);
            }
        }

    }
}
