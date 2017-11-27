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
        private CommandExecute commandExecute;
        public List<IActionCommand> RegistTriggers(ActionCommand[] triggers, StepComplete onStepComplete, CommandExecute commandExecute)
        {
            this.onStepComplete = onStepComplete;
            this.commandExecute = commandExecute;
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
                    if (item.Value.Count > 1)//多命令
                    {
                        item.Value.Sort();
                        var list = new List<IActionCommand>();
                        var total = item.Value.Count;
                        for (int i = 0; i < item.Value.Count; i++)
                        {
                            int index = i;
                            int totalcmd = total;
                            item.Value[index].RegistComplete(OnOneCommandComplete);
                            item.Value[index].onBeforeActive.AddListener((x) =>
                            {
                                OnCommandStartExecute(stepName, totalcmd, index);
                            });
                            list.Add(item.Value[index]);
                        }
                        var cmd = new SequencesCommand(stepName, list);
                        seqDic.Add(stepName, cmd);
                        commandList.Add(cmd);
                    }
                    else//单命令
                    {
                        var cmd = item.Value[0];
                        cmd.RegistComplete(OnOneCommandComplete);
                        cmd.onBeforeActive.AddListener((x) =>
                        {
                            OnCommandStartExecute(stepName, 1, 1);
                        });
                        commandList.Add(cmd);
                    }

                }
            }
        }

        private void OnCommandStartExecute(string stepName, int totalCount, int currentID)
        {
            if(this.commandExecute !=null)
            {
                commandExecute.Invoke(stepName, totalCount, currentID);
            }
        }
    }

}
