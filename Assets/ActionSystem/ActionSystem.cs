using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ActionSystem:MonoBehaviour
    {
        private static ActionSystem instance = default(ActionSystem);
        private static object lockHelper = new object();
        public static bool mManualReset = false;
        protected ActionSystem() { }
        public static ActionSystem Instance
        {
            get
            {
                return instance;
            }

        }
        public event UserError onUserError;//步骤操作错误
        public IRemoteController RemoteController { get { return remoteController; } }
        public CommandController CommandCtrl { get { return commandCtrl; } }
        public IActionStap[] ActiveStaps { get { return steps; } }
        private IRemoteController remoteController;
        private IActionStap[] steps;
        private CommandController commandCtrl;
        private List<IActionCommand> activeCommands;
        private RegistCmds onCommandRegist;
        #region Public Functions

        private void Awake()
        {
            instance = this;
            instance.commandCtrl = new WorldActionSystem.CommandController();
            instance.commandCtrl.onUserErr = instance.OnUserError;
            instance.commandCtrl.onStepComplete = instance.OnStepComplete;
            instance.commandCtrl.onRegistCommand = instance.OnCommandRegistComplete;
        }
        private void Start()
        {
            var triggerList = TriggerStatistics.RetriveTriggsr(transform);
            CommandCtrl.RegistTriggers(triggerList);
        }
        /// <summary>
        /// 设置安装顺序并生成最终步骤
        /// </summary>
        public static IEnumerator LunchActionSystem<T>(T[] steps,UnityAction<ActionSystem, T[]> onLunchOK) where T: IActionStap
        {
            Debug.Assert(steps != null);
            yield return new WaitUntil(() => Instance != null);
            Instance.onCommandRegist = (commandList) =>
            {
                Instance.steps = ConfigSteps<T>(Instance.activeCommands, steps);//重新计算步骤
                Instance.activeCommands = GetIActionCommandList(Instance.activeCommands, Instance.steps);
                Instance.remoteController = new RemoteController(Instance.activeCommands);
                onLunchOK.Invoke(Instance,Array.ConvertAll<IActionStap,T>(Instance.steps,x=>(T)x));
            };

            if (Instance.activeCommands != null)
            {
                Instance.onCommandRegist.Invoke(Instance.activeCommands);
            }
        }


        #endregion

        #region private Funtions
        /// <summary>
        /// 结束命令
        /// </summary>
        /// <param name="stepName"></param>
        private void OnStepComplete(string stepName)
        {
            remoteController.EndExecuteCommand();
        }

        private void OnCommandRegistComplete(List<IActionCommand> cmdList)
        {
            instance.activeCommands = cmdList;
            if (onCommandRegist != null) onCommandRegist.Invoke(cmdList);
        }
        /// <summary>
        /// 用户操作不对
        /// </summary>
        private void OnUserError(string stepName, string errInfo)
        {
            if (onUserError != null)
                onUserError(stepName, errInfo);
        }
        /// <summary>
        /// 重置步骤
        /// </summary>
        /// <param name="commandDic"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        private static IActionStap[] ConfigSteps<T>(List<IActionCommand> commandList, T[] steps) where T:IActionStap
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