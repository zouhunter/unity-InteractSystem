using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class CommandController
    {
        public bool CommandRegisted { get; private set; }
        public List<IActionCommand> CommandList { get { return _commandList; }}
        public Dictionary<string, List<ActionCommand>> CommandDic { get { return actionDic; } }
        private List<IActionCommand> _commandList = new List<IActionCommand>();
        private Dictionary<string, List<ActionCommand>> actionDic = new Dictionary<string, List<ActionCommand>>();//触发器
        private int totalCommand;
        private int currentCommand;
        private Events.StepCompleteAction onStepComplete;
        private Events.CommandExecuteAction commandExecute;
        private Events.RegistCommandListAction onAllCommandRegisted;
        private Events.OperateErrorAction onUserError;

        internal void InitCommand(int totalCommand, Events.CommandExecuteAction onCommandRegistComplete, Events. StepCompleteAction onStepComplete, Events.OperateErrorAction onUserError, Events.RegistCommandListAction onAllCommandRegisted)
        {
            this.totalCommand = totalCommand;
            this.onStepComplete = onStepComplete;
            this.onUserError = onUserError;
            this.commandExecute = onCommandRegistComplete;
            this.onAllCommandRegisted = onAllCommandRegisted;
            TryComplelteRegist();
        }

        public void RegistCommand(ActionCommand command)
        {
            currentCommand++;
            if (actionDic.ContainsKey(command.StepName))
            {
                actionDic[command.StepName].Add(command);
            }
            else
            {
                actionDic[command.StepName] = new List<ActionCommand>() { command };
            }
            TryComplelteRegist();
        }

        private void TryComplelteRegist()
        {
            if (totalCommand == currentCommand)
            {
                RegistTriggerCommand();
                if(onAllCommandRegisted != null){
                    onAllCommandRegisted.Invoke(_commandList);
                }
                CommandRegisted = true;
            }
        }

        private void OnOneCommandComplete(IActionCommand command)
        {
            if(onStepComplete != null)
            {
                onStepComplete(command.StepName);
            }
        }


        private void RegistTriggerCommand()
        {
            if (actionDic != null)
            {
                foreach (var item in actionDic)
                {
                    var stepName = item.Key;
                    if (item.Value.Count > 1 || item.Value[0].CopyCount > 0)//多命令
                    {
                        var cmd = new GroupCommand(stepName, item.Value);
                        cmd.RegistComplete(OnOneCommandComplete);
                        cmd.RegistAsOperate( OnUserError);
                        cmd.RegistCommandChanged(OnCommandStartExecute);
                        _commandList.Add(cmd);
                    }
                    else//单命令
                    {
                        var cmd = item.Value[0];
                        cmd.RegistComplete(OnOneCommandComplete);
                        cmd.RegistAsOperate(OnUserError);

                        cmd.onBeforeActive.AddListener((x) =>
                        {
                            OnCommandStartExecute(stepName, 1, 1);
                        });
                        _commandList.Add(cmd);
                    }

                }
            }
        }

        private void OnUserError(string stepName,string errInfo)
        {
            if(this.onUserError != null)
            {
                this.onUserError.Invoke(stepName, errInfo);
            }
        }


        private void OnCommandStartExecute(string stepName, int totalCount, int currentID)
        {
            if (this.commandExecute != null)
            {
                commandExecute.Invoke(stepName, totalCount, currentID);
            }
        }
    }

}
