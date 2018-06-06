using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
namespace InteractSystem.Common.Actions
{
    [AddComponentMenu(MenuName.ClickObj)]
    [CustomNode("Operate/Click", 10, "ActionSystem")]
    public class ClickNode : RuntimeNode<ClickItem>
    {
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Click;
            }
        }
        public override List<string> NeedElements
        {
            get
            {
                return clickList;
            }
        }
        [SerializeField]
        private List<string> clickList = new List<string>();
        private ClickItem[] finalGroup { get; set; }
        private int clickedIndex = 0;
        private List<ClickItem> clickedItems = new List<ClickItem>();

        protected override void OnEnable()
        {
            base.OnEnable();
            clickedItems.Clear();
            finalGroup = null;
            clickedIndex = 0;
        }
        protected override void OnAdd(ClickItem arg0)
        {
            base.OnAdd(arg0);
            if (Statu == ExecuteStatu.Executing)
            {
                arg0.StepActive();
                arg0.RegistOnClick(TryComplete);
            }
        }
        protected override void OnRemove(ClickItem arg0)
        {
            base.OnRemove(arg0);
            arg0.RemoveOnClicked(TryComplete);
        }

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            FindClickableItems();
            if (auto)
            {
                AutoClickItems();
            }
            else
            {
                AngleOnActive();
            }
        }

        private void TryComplete(ClickItem item)
        {
            if (statu != ExecuteStatu.Executing) return;//没有执行
            if (!item.ClickAble) return;//目标无法点击
            if (clickedIndex >= NeedElements.Count) return;//超过需要

            if (NeedElements[clickedIndex] == item.Name)
            {
                clickedItems.Add(item);
                clickedIndex++;
                item.RecordPlayer(this);
            }

            if (clickedIndex == clickedItems.Count)
            {
                finalGroup = clickedItems.ToArray();
                OnEndExecute(false);
            }
        }

        /// <summary>
        /// 将所能点击的目标设置为激活状态
        /// </summary>
        private void FindClickableItems()
        {
            if (clickList.Count > clickedIndex)
            {
                var key = clickList[clickedIndex];
                var elements = elementPool.FindAll(x => x.Name == key && x.ClickAble);
                elements.ForEach(element =>
                {
                    element.StepActive();
                    element.RegistOnClick(TryComplete);
                });
            }
        }

        protected override void CompleteElements(bool undo)
        {
            base.CompleteElements(undo);

            if (undo)
            {
                foreach (var item in clickedItems)
                {
                    item.StepUnDo();
                    item.RemovePlayer(this);
                }
                clickedIndex = 0;
                clickedItems.Clear();
            }
            else
            {
                for (int i = 0; i < clickList.Count; i++)
                {
                    if (clickedItems.Count <= i)
                    {
                        var element = elementPool.Find(x => x.Name == clickList[i] && x.ClickAble);
                        if (element != null)
                        {
                            element.RecordPlayer(this);
                            element.StepComplete();
                            clickedItems.Add(element);
                        }
                        else
                        {
                            Debug.LogError("缺少：" + clickList[i]);
                        }
                    }
                    else
                    {
                        var item = clickedItems[i];
                        if(item.Active){
                            clickedItems[i].StepComplete();
                        }
                        clickedItems[i].RecordPlayer(this);
                    }
                }
            }
        }
        /// <summary>
        /// 箭头提醒一个其中一个目标
        /// </summary>
        private void AngleOnActive()
        {

        }

        /// <summary>
        /// 自动点击目标元素
        /// </summary>
        private void AutoClickItems()
        {
            for (int i = 0; i < clickList.Count; i++)
            {
                var element = elementPool.Find(x => x.Name == clickList[i] && x.ClickAble);
                if (element != null)
                {
                    element.RecordPlayer(this);
                    element.StepComplete();
                    clickedItems.Add(element);
                }
                else
                {
                    Debug.LogError("缺少：" + clickList[i]);
                }
            }
        }
    }
}