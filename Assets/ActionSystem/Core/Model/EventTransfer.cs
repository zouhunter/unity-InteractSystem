using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class EventTransfer
    {
        public event Events.OperateErrorAction onUserError;//步骤操作错误
        public event Events.CommandExecuteAction onCommandExecute;
        public ActionGroup group;

        public EventTransfer(ActionGroup group)
        {
            this.group = group;
        }

        private void OnUserError(string stepName, string error)
        {
            if (onUserError != null)
                onUserError.Invoke(stepName, error);
        }

        /// <summary>
        /// 结束命令
        /// </summary>
        private void OnStepComplete(string stepName)
        {
            group.RemoteController.OnEndExecuteCommand(stepName);
        }

        private void OnCommandExectute(string stepName, int totalCount, int currentID)
        {
            if (onCommandExecute != null)
            {
                onCommandExecute.Invoke(stepName, totalCount, currentID);
            }
        }

    }
}