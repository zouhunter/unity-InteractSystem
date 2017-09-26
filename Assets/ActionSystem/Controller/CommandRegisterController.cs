using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class CommandRegisterController : IActionEvents
    {
        public StepComplete onStepComplete { get; set; }
        public UserError onUserErr { get; set; }
        public RegistCmds onRegisted;
        private List<IActionCommand> commandList = new List<IActionCommand>();
        private bool allActionRegisted;
        private bool allRegisted { get { return allActionRegisted; } }
        private Dictionary<string, List<ActionTrigger>> actionDic;//触发器
        private ElementController elementController;//元素名、列表

        private Dictionary<string, SequencesCommand> seqDic = new Dictionary<string, SequencesCommand>();

        public CommandRegisterController()
        {
            this.elementController = new WorldActionSystem.ElementController();
        }
        public void RegistElement(InstallItem installItem)
        {
            elementController.RegistElement(installItem);
        }
        public void RegistActionTriggers(ActionTriggers actionTriggers)
        {
            if (actionTriggers == null)
            {
                allActionRegisted = true;
            }
            else
            {
                actionTriggers.onAllElementInit = OnRegistTriggers;
            }
        }

        /// <summary>
        /// 如果动画被注册为命令，则替换
        /// </summary>
        /// <param name="dic"></param>
        private void OnRegistTriggers(Dictionary<string, List<ActionTrigger>> dic)
        {
            actionDic = dic;
            if (actionDic != null)
            {
                foreach (var item in dic)
                {
                    foreach (var trigger in item.Value)
                    {
                        trigger.onStepComplete = OnOneCommandComplete;
                        trigger.onUserErr = onUserErr;
                    }
                }
            }
            allActionRegisted = true;
            TryCreateCommandList();
        }
        /// <summary>
        /// 创建命令列表
        /// </summary>
        private void TryCreateCommandList()
        {
            if (allRegisted)
            {
                RegistTriggerCommand();
                if (onRegisted != null) onRegisted(commandList);
            }
        }

        private void OnOneCommandComplete(string stepName)
        {
            if (seqDic.ContainsKey(stepName))
            {
                var cmd = seqDic[stepName];
                if (!cmd.ContinueExecute())
                {
                    onStepComplete.Invoke(stepName);
                }
            }
            else
            {
                onStepComplete.Invoke(stepName);
            }
        }


        private void RegistTriggerCommand()
        {
            if (actionDic != null)
            {
                foreach (var item in actionDic)
                {
                    var stepName = item.Key;
                    if (item.Value.Count > 1)//多命令合并为一个命令
                    {
                        item.Value.Sort();
                        var list = new List<IActionCommand>();
                        for (int i = 0; i < item.Value.Count; i++)
                        {
                            item.Value[i].ElementController = () => { return elementController; };
                            list.AddRange(item.Value[i].CreateCommands());
                        }
                        var cmd = new SequencesCommand(stepName, list);
                        seqDic.Add(stepName, cmd);
                        commandList.Add(cmd);
                    }
                    else//单命令 选择性合并为一个命令
                    {
                        item.Value[0].ElementController = () => { return elementController; };
                        var cmds = item.Value[0].CreateCommands();
                        if (cmds.Count > 1)
                        {
                            var cmd = new SequencesCommand(stepName, cmds);
                            seqDic.Add(stepName, cmd);
                            commandList.Add(cmd);
                        }
                        else if (cmds.Count == 1)
                        {
                            commandList.AddRange(cmds);
                        }
                    }

                }
            }
        }
    }

}
