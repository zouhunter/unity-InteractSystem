using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem
{
    [System.Serializable]
    public class CompleteAbleNodeFeature : OperateNodeFeature
    {
        public UnityAction onComplete { get; set; }
        [SerializeField]
        public string elementName;
        public ActionItem actionItem { get; set; }
        protected ElementPool<ActionItem> elementPool = new ElementPool<ActionItem>();
        protected ElementController elementCtrl { get { return ElementController.Instence; } }
        protected System.Type type;
        public CompleteAbleNodeFeature(System.Type type)
        {
            this.type = type;
        }
        public override void OnEnable()
        {
            base.OnEnable();
            actionItem = null;

            elementPool.onAdded = OnAddedToPool;
            elementPool.onRemoved = OnRemovedFromPool;

            elementCtrl.onRegistElememt += OnRegistElement;
            elementCtrl.onRemoveElememt += OnRemoveElement;
        }

        protected void OnAddedToPool(ActionItem arg0)
        {
            RegistComplete(arg0);
        }

        protected void OnRegistElement(ISupportElement arg0)
        {
            if (arg0.GetType() == type && arg0 is ActionItem && arg0.Name == elementName)
            {
                var element = arg0 as ActionItem;
                if (!elementPool.Contains(element))
                {
                    elementPool.ScureAdd(element);
                }
            }
        }

        protected void OnRemovedFromPool(ActionItem arg0)
        {
            RemoveComplete(arg0);
        }

        protected void AutoCompleteItems()
        {
            if (target.Statu != ExecuteStatu.Completed)
            {
                var key = elementName;
                var item = elementPool.Find(x => x.Name == key && x.Active && x.OperateAble && x is ActionItem) as ActionItem;
                if (item != null)
                {
                    var completeFeature = item.RetriveFeature<CompleteAbleItemFeature>();
                    if (completeFeature != null)
                    {
                        completeFeature.RegistOnCompleteSafety(OnAutoComplete);
                        completeFeature.AutoExecute(target);
                    }
                }
                else
                {
                    Debug.LogError("have no active useful element Name:" + key);
                }
            }
        }

        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);

            if (onComplete != null)
                onComplete.Invoke();
        }

        private void OnAutoComplete(CompleteAbleItemFeature arg0)
        {
            arg0.RemoveOnComplete(OnAutoComplete);
            AutoCompleteItems();
        }

        /// <summary>
        /// 移除可点击元素
        /// </summary>
        /// <param name="arg0"></param>
        protected void OnRemoveElement(ISupportElement arg0)
        {
            if (arg0.GetType() == type && arg0.Name == elementName)
            {
                var element = arg0 as ActionItem;
                if (elementPool.Contains(element))
                {
                    elementPool.ScureRemove(element);
                }
            }
        }
        protected virtual void TryComplete(CompleteAbleItemFeature item)
        {
            if (target.Statu != ExecuteStatu.Executing) return;//没有执行
            if (!item.target.OperateAble) return;//目标无法点击
            if (actionItem != null) return;

            actionItem = item.target as ActionItem;
            item.target.RecordPlayer(target);
            item.target.StepComplete();
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
            var elements = elementPool.FindAll(x => x.Name == elementName && x.OperateAble);
            elements.ForEach(element =>
            {
                element.StepActive();
                var feature = element.RetriveFeature<CompleteAbleItemFeature>();
                if (feature != null)
                {
                    feature.RegistOnCompleteSafety(TryComplete);
                }
            });
        }
        /// <summary>
        ///尝试将元素设置为结束状态
        /// </summary>
        /// <param name="undo"></param>
        protected void CompleteElements(bool undo)
        {
            if (undo)
            {
                if(actionItem != null)
                {
                    actionItem.StepUnDo();
                    actionItem.RemovePlayer(target);
                    actionItem = null;
                }
            }
            else
            {
                var element = elementPool.Find(x => x.Name == elementName && x.OperateAble);
                if (element != null)
                {
                    element.RecordPlayer(target);
                    element.StepComplete();
                    actionItem = element;
                }
                else
                {
                    Debug.LogError("缺少：" + elementName);
                }
            }
        }
        /// <summary>
        /// 注册结束事件（仅元素在本步骤开始后创建时执行注册）
        /// </summary>
        /// <param name="arg0"></param>
        protected void RegistComplete(ActionItem arg0)
        {
            if (target.Statu == ExecuteStatu.Executing)
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
