using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ActionSystem : AssistManager<ActionSystem>
    {
        IActionStap[] staps;
        Dictionary<string, ActionCommand> commandDic = new Dictionary<string, ActionCommand>();
        List<ActionCommand> actionCommandList = new List<ActionCommand>();
        public IRemoteController remoteController;

        /// <summary>
        /// 获取控制器
        /// </summary>
        /// <returns></returns>
        public void GetRemoteController(UnityAction<IRemoteController> onCtrlCreate)
        {
            if (remoteController != null)
            {
                onCtrlCreate(remoteController);
            }
            else if (staps != null && staps.Length == commandDic.Count)
            {
                GetActionCommandList();
                remoteController = new RemoteController(actionCommandList);
                onCtrlCreate(remoteController);
            }
            else
            {
                StartCoroutine(WaitToCreateRemoteCtrl(onCtrlCreate));
            }
        }
        IEnumerator WaitToCreateRemoteCtrl(UnityAction<IRemoteController> onCtrlCreate)
        {
            yield return new WaitUntil(() => staps != null);
            yield return new WaitUntil(() => commandDic.Count == staps.Length);
            GetActionCommandList();
            remoteController = new RemoteController(actionCommandList);
            onCtrlCreate(remoteController);
        }

        /// <summary>
        /// 设置安装顺序
        /// </summary>
        public void SetActionStaps(IActionStap[] staps)
        {
            this.staps = staps;
        }

        /// <summary>
        /// 添加命令
        /// </summary>
        public void AddActionCommand(ActionCommand cmd)
        {
            commandDic.Add(cmd.StapName, cmd);
        }

        /// <summary>
        /// 得到排序后的命令列表
        /// </summary>
        /// <returns></returns>
        List<ActionCommand> GetActionCommandList()
        {
            ActionCommand cmd;
            actionCommandList.Clear();
            foreach (var item in staps)
            {
                if (commandDic.TryGetValue(item.StapName, out cmd))
                {
                    actionCommandList.Add(cmd);
                }
            }
            return actionCommandList;
        }
    }

}