using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem.Actions;
using System;
using System.Linq;

namespace InteractSystem
{
    /// <summary>
    /// 顺序执行一组元素
    /// </summary>
    [System.Serializable]
    public class QueueCollectNodeFeature : CollectNodeFeature
    {
        protected UnityAction onComplete { get; private set; }
        protected List<ISupportElement> currents = new List<ISupportElement>();
        public QueueCollectNodeFeature(System.Type type) : base(type, false) { }
        public event UnityAction<CompleteAbleItemFeature> onBeforeAutoExecute;
        public override void OnEnable()
        {
            base.OnEnable();
            currents.Clear();
            finalGroup = null;
        }

        protected override void OnAddedToPool(ISupportElement arg0)
        {
            base.OnAddedToPool(arg0);
            if (SupportType(arg0.GetType()))
            {
                RegistComplete(arg0);
            }
        }

        public override void SetTarget(Graph.OperaterNode node)
        {
            base.SetTarget(node);
            onComplete = () =>
            {
                node.OnEndExecute(false);
            };
        }

        protected override void OnRemovedFromPool(ISupportElement arg0)
        {
            base.OnRemovedFromPool(arg0);
            if (!SupportType(arg0.GetType())) return;
            RemoveComplete(arg0 as ActionItem);
        }

        protected void AutoComplete(int index)
        {
            Debug.Assert(index < itemList.Count);
            var key = itemList[index];
            var item = elementPool.Find(x => x.Name == key && (x.OperateAble || x.HavePlayer(target))) as ActionItem;
            if (item != null)
            {
                if (!item.Actived) ActiveElement(item);

                var completeFeature = item.RetriveFeature<CompleteAbleItemFeature>();
                if (completeFeature != null)
                {
                    completeFeature.RegistOnCompleteSafety(target, TryComplete);
                    if (onBeforeAutoExecute != null)
                        onBeforeAutoExecute.Invoke(completeFeature);
                    completeFeature.AutoExecute(target);
                }
            }
            else
            {
                Debug.LogError(index + ":have no active useful element Name:" + key);
            }

        }

        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);

            if (onComplete != null)
            {
                onComplete.Invoke();
            }
            else
            {
                Debug.LogError("onComplete not registed:" + target);
            }
        }


        protected virtual void TryComplete(UnityEngine.Object context, ActionItem actionItem)
        {
            if (context != target)
            {
                if(log) Debug.Log("目标不一至");
                return;
            }
            if(target.Statu != ExecuteStatu.Executing)
            {
                if(log) Debug.Log("没有执行");
                return;
            }
            if(!actionItem.OperateAble &&!actionItem.HavePlayer(target))
            {
                if (log) Debug.Log("目标无法操作");
                return;
            }
            if(currents.Count >= itemList.Count)
            {
                if (log) Debug.Log("超过需要");
                return;
            }


            if (itemList[currents.Count] == actionItem.Name)
            {
                if (log) Debug.Log("add:" + actionItem);
                currents.Add(actionItem);
                actionItem.RecordPlayer(target);

                SetInActiveElement(actionItem);
            }

            if (currents.Count >= itemList.Count)
            {
                finalGroup = currents.ToArray();
                OnEndExecute(false);
            }
            else
            {
                ActiveElements();
                if (autoExecute)
                {
                    AutoComplete(currents.Count);
                }
            }
        }

        /// <summary>
        /// 将所能点击的目标设置为激活状态
        /// </summary>
        protected override void ActiveElements()
        {
            if (itemList.Count > currents.Count)
            {
                var key = itemList[currents.Count];

                var elements = elementPool.FindAll(x => x.Name == key && (x as ActionItem).OperateAble);

                elements.ForEach(element =>
                {
                    if (element.OperateAble && target.Statu == ExecuteStatu.Executing)
                    {
                        ActiveElement(element);
                    }

                    var feature = (element as ActionItem).RetriveFeature<CompleteAbleItemFeature>();

                    if (feature != null)
                    {
                        feature.RegistOnCompleteSafety(target, TryComplete);
                    }
                    else
                    {
                        Debug.LogError("element have no completeAble feature:", element as ActionItem);
                    }
                });
            }
        }

        /// <summary>
        /// 自动点击目标元素
        /// </summary>
        public virtual void AutoCompleteItems()
        {
            if(itemList.Count > 0) AutoComplete(0);
            else
            {
                Debug.Log("empty step!");
                OnEndExecute(false);
            }
        }

        protected override void UnDoActivedElement()
        {
            ForEachElement((item) =>
            {
                UndoElement(item);
                item.RemovePlayer(target);
            });
            currents.Clear();
        }

        protected override void InActivedElements()
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                var elementName = itemList[i];
                var elements = elementPool.FindAll(x => x.Name == elementName);

                foreach (var element in elements) {
                    SetInActiveElement(element);
                }

                ///锁定元素并结束
                if (currents.Count <= i)
                {
                    var element = elements.FirstOrDefault();
                    if (element != null)
                    {
                        currents.Add(element);
                    }
                    else
                    {
                        Debug.LogError("缺少：" + elementName);
                    }
                }
                var item = currents[i];
                (item as ActionItem).RecordPlayer(target);
            }
        }
        /// <summary>
        /// 注册结束事件（仅元素在本步骤开始后创建时执行注册）
        /// </summary>
        /// <param name="arg0"></param>
        protected void RegistComplete(ISupportElement arg0)
        {
            if (target.Statu == ExecuteStatu.Executing)
            {
                if (arg0 is ActionItem)
                {
                    // 注册元素结束事件
                    var feature = (arg0 as ActionItem).RetriveFeature<CompleteAbleItemFeature>();
                    feature.RegistOnCompleteSafety(target, TryComplete);
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
                if (feature == null)
                {
                    Debug.Log(arg0 + "中没有:CompleteAbleItemFeature");
                }
                else
                {
                    feature.RemoveOnComplete(target);
                }
            }
        }

    }
}
