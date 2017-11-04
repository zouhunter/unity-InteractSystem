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

        public bool activeOnComplete;
        public bool activeOnStart;

        private string resetKey { get { return "HideResetObjects"; } }
        private string hideKey { get { return "HideObjects"; } }
        private string showKey { get { return "UnHideObjects"; } }


        protected override void OnBeforeActive(string step)
        {
            if (activeOnStart)
            {
                EventController.NotifyObserver(showKey, key);
            }
            else
            {
                EventController.NotifyObserver(hideKey, key);
            }
        }
        protected override void OnBeforePlayEnd(string step)
        {
            if (activeOnComplete)
            {
                EventController.NotifyObserver(showKey, key);
            }
            else
            {
                EventController.NotifyObserver(hideKey, key);
            }
        }
        protected override void OnBeforeUnDo(string step)
        {
            EventController.NotifyObserver(resetKey, key);
        }


    }
}
