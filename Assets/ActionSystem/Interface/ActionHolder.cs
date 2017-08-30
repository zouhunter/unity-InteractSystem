using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public abstract class ActionHolder : MonoBehaviour
    {
        public abstract bool Registed { get; }
        /// <summary>
        /// 注册安装命令
        /// </summary>
        public UnityAction<ActionCommand> OnRegistCommand;

        /// <summary>
        /// 步骤结束事件
        /// </summary>
        public StepComplete OnStepEnd;

        /// <summary>
        /// 用户操作错误
        /// </summary>
        public UserError onUserErr;

        /// <summary>
        /// 设置高亮显示
        /// </summary>
        /// <param name="on"></param>
        public abstract void SetHighLight(bool on);
    }

}