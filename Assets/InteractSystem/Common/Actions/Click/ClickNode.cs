using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
namespace InteractSystem.Common.Actions
{
    [CustomNode("Operate/Click", 12, "InteratSystem")]
    public class ClickNode : ClickAbleActionNode
    {
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Click;
            }
        }

        protected override void AutoCompleteItems()
        {
            for (int i = 0; i < itemList.Count; i++)
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
            OnEndExecute(false);
        }

    }
}