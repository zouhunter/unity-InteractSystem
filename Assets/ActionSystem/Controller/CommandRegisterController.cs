using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class CommandRegisterController
    {
        public StepComplete onStepComplete;
        public UserError onUserErr;
        public RegistCmds onRegisted;
        private List<IActionCommand> commandList = new List<IActionCommand>();
        private bool allInstallElementRegisted;
        private bool allActionRegisted;
        private bool allRegisted { get { return allInstallElementRegisted && allActionRegisted; } }
        private Dictionary<string,List<ActionTrigger>> actionDic;//触发器
        private ElementGroup elementGroup;//元素名、列表
        private string currentStep;

        private Dictionary<string, SequencesCommand> seqDic = new Dictionary<string, SequencesCommand>();

        public void RegistInstallElement(ElementGroup elements)
        {
            elementGroup = elements;
            allInstallElementRegisted = true;
            TryCreateCommandList();
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
        private void OnRegistTriggers(Dictionary<string,List<ActionTrigger>> dic)
        {
            actionDic = dic;
            if (actionDic != null)
            {
                foreach (var item in dic)
                {
                    var stepName = item.Key;
                    foreach (var trigger in item.Value)
                    {
                        trigger.InitTrigger(OnOneCommandComplete,onUserErr);
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
                if(onRegisted != null) onRegisted(commandList);
            }
        }

        private void OnOneCommandComplete(string stepName)
        {
            if(seqDic.ContainsKey(stepName))
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
                    if (item.Value.Count > 1)
                    {
                        item.Value.Sort();
                        var list = new List<IActionCommand>();
                        for (int i = 0; i < item.Value.Count; i++){
                            item.Value[i].ElementGroup = () => { return elementGroup; };
                            list.AddRange(item.Value[i].CreateCommands());
                        }
                        var cmd = new SequencesCommand(stepName,1, list);
                        seqDic.Add(stepName, cmd) ;
                        commandList.Add(cmd);
                    }
                    else
                    {
                        item.Value[0].ElementGroup = () => { return elementGroup; };
                        commandList.AddRange(item.Value[0].CreateCommands());
                    }
                   
                }
            }
        }
    }

}
