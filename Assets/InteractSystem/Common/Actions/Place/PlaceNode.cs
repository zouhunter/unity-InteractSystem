using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem.Common.Actions
{
    public abstract class PlaceNode : ClickAbleActionNode
    {
        /// <summary>
        /// 自动进行安装演示
        /// </summary>
        protected override void AutoCompleteItems()
        {
            TryAutoComplete(0);
        }

        protected void TryAutoComplete(int index)
        {
            if (index < itemList.Count)
            {
                var key = itemList[index];
                var item = elementPool.Find(x => x.Name == key && x.Active && x.OperateAble && x is PlaceItem) as PlaceItem;
                item.RegistOnCompleteSafety(OnAutoComplete);
                item.OnAutoInstall();
            }
        }

        private void OnAutoComplete(ClickAbleActionItem arg0)
        {
            arg0.RemoveOnComplete(OnAutoComplete);
            TryAutoComplete(currents.Count);
        }
    }
}