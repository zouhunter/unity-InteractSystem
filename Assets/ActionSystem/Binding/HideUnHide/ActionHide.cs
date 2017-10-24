using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    /// <summary>
    /// 在一个触发器时间内触发关闭和打开
    /// 或 打开或关闭事件
    /// </summary>
    public class ActionHide : ActionObjBinding
    {
        public string key;
        public bool reverse;

        private string hideKey { get { return "HideObjects"; } }
        private string showKey { get { return "UnHideObjects"; } }

        private void OnDestroy()
        {
            if (actionObj)
            {
                actionObj.onBeforeStart.RemoveListener(OnBeforeActive);
                actionObj.onBeforeComplete.RemoveListener(OnBeforeComplete);
            }
        }
        protected override void OnBeforeActive()
        {
            SetElementState(reverse);
        }
        protected override void OnBeforeComplete()
        {
            SetElementState(!reverse);
        }

        private void SetElementState(bool open)
        {
            if (open)
            {
                EventController.NotifyObserver<string>(showKey, key);
            }
            else
            {
                EventController.NotifyObserver<string>(hideKey, key);
            }
        }

    }
}