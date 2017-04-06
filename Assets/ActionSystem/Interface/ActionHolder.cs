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
        /// <summary>
        /// 注册安装命令
        /// </summary>
        public UnityAction<ActionCommand> registFunc;
        
        /// <summary>
        /// 用户操作错误
        /// </summary>
        public UnityAction<string, string> onUserErr;

        /// <summary>
        /// 设置高亮显示
        /// </summary>
        /// <param name="on"></param>
        public abstract void SetHighLight(bool on);

        /// <summary>
        /// 添加功能脚本
        /// </summary>
        /// <param name="on"></param>
        public abstract void InsertScript<T>(bool on) where T:MonoBehaviour;
    }

}