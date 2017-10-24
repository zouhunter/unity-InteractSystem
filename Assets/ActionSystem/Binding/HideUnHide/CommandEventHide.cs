using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    /// <summary>
    /// 在一个ActionCommand时间内触发关闭和打开
    /// 或 打开或关闭事件
    /// </summary>
    public class CommandEventHide : ActionCommandBinding
    {
        public string key;
        public bool reverse;

        private string hideKey { get { return "HideObjects"; } }
        private string showKey { get { return "UnHideObjects"; } }


        protected override void OnBeforeActive(string step)
        {
            SetElementState(reverse);
        }
        protected override void OnBeforePlayEnd(string step)
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
