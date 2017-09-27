using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class CommandController : IActionEvents
    {
        public StepComplete onStepComplete { get; set; }
        public RegistCmds onRegistCommand{get;set;}
        public UserError onUserErr { get; set; }
        private List<IActionCommand> commandList = new List<IActionCommand>();
        private Dictionary<string, List<ActionTrigger>> actionDic = new Dictionary<string, List<ActionTrigger>>();//触发器
        private ElementController elementController;//元素名、列表
        private Dictionary<string, SequencesCommand> seqDic = new Dictionary<string, SequencesCommand>();
        private int triggerCount;//外部注册时,需要个数判断

        public CommandController()
        {
            this.elementController = new WorldActionSystem.ElementController();
        }
        public void RegistElement(PickUpAbleElement installItem)
        {
            elementController.RegistElement(installItem);
        }
        public void RegistTriggers(List<ActionTrigger> triggers)
        {
            foreach (var trigger in triggers)
            {
                var obj = trigger;
                trigger.onStepComplete = OnOneCommandComplete;
                trigger.onUserErr = onUserErr;

                if (actionDic.ContainsKey(obj.StepName))
                {
                    actionDic[obj.StepName].Add(obj);
                }
                else
                {
                    actionDic[obj.StepName] = new List<ActionTrigger>() { obj };
                }
            }
            RegistTriggerCommand();
            if (onRegistCommand != null) onRegistCommand.Invoke(commandList);
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
