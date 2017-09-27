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
        private List<IActionCommand> commandList = new List<IActionCommand>();
        private Dictionary<string, List<ActionCommand>> actionDic = new Dictionary<string, List<ActionCommand>>();//触发器
        private Dictionary<string, SequencesCommand> seqDic = new Dictionary<string, SequencesCommand>();
        private StepComplete onStepComplete;
        public List<IActionCommand> RegistTriggers(ActionCommand[] triggers, StepComplete onStepComplete)
        {
            this.onStepComplete = onStepComplete;
            foreach (var trigger in triggers)
            {
                var obj = trigger;
                if (actionDic.ContainsKey(obj.StepName))
                {
                    actionDic[obj.StepName].Add(obj);
                }
                else
                {
                    actionDic[obj.StepName] = new List<ActionCommand>() { obj };
                }
            }
            RegistTriggerCommand();
            return commandList;
        }

        private void OnOneCommandComplete(string stepName)
        {
            if (seqDic.ContainsKey(stepName))
            {
                var cmd = seqDic[stepName];
                if (!cmd.ContinueExecute())
                {
                    onStepComplete(stepName);
                }
            }
            else
            {
                onStepComplete(stepName);
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
                            item.Value[i].RegistComplete(OnOneCommandComplete);
                            list.Add(item.Value[i]);
                        }
                        var cmd = new SequencesCommand(stepName, list);
                        seqDic.Add(stepName, cmd);
                        commandList.Add(cmd);
                    }
                    else//单命令 选择性合并为一个命令
                    {
                        item.Value[0].RegistComplete(OnOneCommandComplete);
                        var cmd = item.Value[0];
                        commandList.Add(cmd);
                    }

                }
            }
        }
    }

}
