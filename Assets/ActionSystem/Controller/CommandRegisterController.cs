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
        private bool allAnimRegisted;
        private bool allInstallElementRegisted;
        private bool allActionRegisted;
        private bool allRegisted { get { return allAnimRegisted && allInstallElementRegisted && allActionRegisted; } }
        private List<ActionResponce> responceList;//步骤名、列表
        private List<ActionTrigger> actionList;//触发器
        private Dictionary<string, List<InstallItem>> itemDic;//元素名、列表
        private string currentStep;

        public void RegistAnimGroup(ActionResponces responce)
        {
            if (responce == null)
            {
                allAnimRegisted = true;
            }
            else
            {
                responce.onAllElementInit = OnRegistAnimElements;
            }
        }
        public void RegistInstallElement(ElementGroup elements)
        {
            if (elements == null)
            {
                allInstallElementRegisted = true;
            }
            else
            {
                elements.onAllElementInit = OnRegistInstallElement;
            }
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
        /// 如果没有其他触发器注册动画，则注册
        /// </summary>
        /// <param name="list"></param>
        private void OnRegistAnimElements(List<ActionResponce> list)
        {
            responceList = list;
            allAnimRegisted = true;
            TryCreateCommandList();
        }

        private void OnRegistInstallElement(Dictionary<string, List<InstallItem>> dic)
        {
            itemDic = dic;
            allInstallElementRegisted = true;
            TryCreateCommandList();
        }
        /// <summary>
        /// 如果动画被注册为命令，则替换
        /// </summary>
        /// <param name="list"></param>
        private void OnRegistTriggers(List<ActionTrigger> list)
        {
            actionList = list;
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
                RegistAutoAnimCommand();
                RegistTriggerCommand();
                if(onRegisted != null) onRegisted(commandList);
            }
        }

        /// <summary>
        /// 自动执行动画的步骤
        /// </summary>
        /// <returns></returns>
        private void RegistAutoAnimCommand()
        {
            if (responceList != null)
            {
                foreach (var item in responceList)
                {
                    if (actionList == null || !actionList.Find(x => x.StepName == item.StepName))
                    {
                        commandList.Add(item.CreateCommand());
                    }
                }
            }
           
        }

        private void RegistTriggerCommand()
        {
            if (actionList != null)
            {
                foreach (var item in actionList)
                {
                    item.Responce = () => { return GetResponce(item.StepName); };
                    item.InstallItems = () => { return GetInstallItems(item.name); };
                    commandList.Add(item.CreateCommand());
                }
            }
        }

        private ActionResponce GetResponce(string stepName)
        {
            ActionResponce value = null;
            if(responceList != null)
            {
                value = responceList.Find(x=>x.StepName == stepName);
            }
            return value;
        }

        private List<InstallItem> GetInstallItems(string elementName)
        {
            List<InstallItem> value = null;
            if (itemDic != null)
            {
                itemDic.TryGetValue(elementName, out value);
            }
            return value;
        }
    }
}
