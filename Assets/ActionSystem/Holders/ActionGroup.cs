using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ActionGroup : MonoBehaviour
    {
        public string groupKey;
        public int totalCommand;
        public List<ActionPrefabItem> prefabList = new List<ActionPrefabItem>();

        #region Events
        public event UserError onUserError;//步骤操作错误
        public event CommandExecute onCommandExecute;
        #endregion

        #region Propertys
        internal IRemoteController RemoteController { get { return remoteController; } }
        internal IActionStap[] ActiveStaps { get { return steps; } }
        internal CommandController CommandCtrl
        {
            get
            {
                return commandCtrl;
            }
        }
        internal EventController EventCtrl
        {
            get
            {
                return eventCtrl;
            }
        }
        #endregion

        #region Private
        private IActionStap[] steps;
        private IRemoteController remoteController;
        private CommandController commandCtrl = new CommandController();
        private EventController eventCtrl = new EventController();
        private RegistCommandList onCommandRegisted { get; set; }
        #endregion

        #region UnityFunctions
        private void Start()
        {
            commandCtrl.InitCommand(totalCommand, OnCommandExectute, OnStepComplete, OnUserError,
               (x) =>
               {
                   if (onCommandRegisted != null)
                       onCommandRegisted.Invoke(x);
               });
            Utility.CreateRunTimeObjects(transform, prefabList);
            ActionSystem.Instence.RegistGroup(this);
        }
        private void OnDestroy()
        {
            ActionSystem.Instence.RemoveGroup(this);
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// 设置安装顺序并生成最终步骤
        /// </summary>
        public void LunchActionSystem<T>(T[] steps, UnityAction<T[]> onLunchOK) where T : IActionStap
        {
            Debug.Assert(steps != null);
            onCommandRegisted = (activeCommands) =>
            {
                this.steps = ConfigSteps<T>(activeCommands, steps);//重新计算步骤
                activeCommands = GetIActionCommandList(activeCommands, this.steps);
                remoteController = new RemoteController(activeCommands);
                onLunchOK.Invoke(Array.ConvertAll<IActionStap, T>(this.steps, x => (T)x));
            };

            if (commandCtrl.CommandRegisted)
            {
                onCommandRegisted.Invoke(commandCtrl.CommandList);
            }
        }
        #endregion

        #region private Funtions
        /// <summary>
        /// 结束命令
        /// </summary>
        private void OnStepComplete(string stepName)
        {
            if (remoteController.CurrCommand != null && remoteController.CurrCommand.StepName == stepName)
            {
                remoteController.OnEndExecuteCommand();
            }
            else
            {
                Debug.LogError("Not Step :" + stepName);
            }
        }

        private void OnCommandExectute(string stepName, int totalCount, int currentID)
        {
            if (onCommandExecute != null)
            {
                onCommandExecute.Invoke(stepName, totalCount, currentID);
            }
        }

        /// <summary>
        /// 错误触发
        /// </summary>
        /// <param name="stepName"></param>
        /// <param name="error"></param>
        private void OnUserError(string stepName, string error)
        {
            if (onUserError != null) onUserError.Invoke(stepName, error);
        }

        /// 重置步骤
        /// </summary>
        /// <param name="commandDic"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        private static IActionStap[] ConfigSteps<T>(List<IActionCommand> commandList, T[] steps) where T : IActionStap
        {
            List<IActionStap> activeStaps = new List<IActionStap>();
            List<string> ignored = new List<string>();
            for (int i = 0; i < steps.Length; i++)
            {
                var old = commandList.Find(x => x.StepName == steps[i].StapName);
                if (old != null)
                {
                    activeStaps.Add(steps[i]);
                }
                else
                {
                    ignored.Add(steps[i].StapName);
                }
            }
            Debug.Log("[Ignored steps:]" + String.Join("|", ignored.ToArray()));
            return activeStaps.ToArray();
        }

        /// <summary>
        /// 得到排序后的命令列表
        /// </summary>
        /// <returns></returns>
        private static List<IActionCommand> GetIActionCommandList(List<IActionCommand> commandList, IActionStap[] steps)
        {
            var actionCommandList = new List<IActionCommand>();
            foreach (var item in steps)
            {
                var old = commandList.Find(x => x.StepName == item.StapName);
                if (old != null)
                {
                    actionCommandList.Add(old);
                }
                else
                {
                    Debug.LogWarning(item + "已经存在");
                }
            }
            return actionCommandList;
        }

        #endregion

    }

}