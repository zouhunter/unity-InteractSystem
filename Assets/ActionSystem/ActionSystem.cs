using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ActionSystem : MonoBehaviour
    {
        public static ActionSystem Instance;

        public event UserError onUserError;//步骤操作错误

        public IRemoteController RemoteController { get { return remoteController; } }
        public IActionStap[] ActiveStaps { get { return steps; } }
        private IRemoteController remoteController;
        private IActionStap[] steps;
        private CommandRegisterController registController;
        private List<IActionCommand> activeCommand;
        void Awake()
        {
            Instance = this;
            registController = new WorldActionSystem.CommandRegisterController();
            registController.onUserErr = OnUserError;
            registController.onStepComplete = OnStepComplete;
            registController.RegistActionTriggers(GetComponentInChildren<ActionTriggers>());
            registController.RegistInstallElement(GetComponentInChildren<ElementGroup>());
            registController.onRegisted = (cmdList) => { activeCommand = cmdList; };
        }

        #region Public Functions
        /// <summary>
        /// 设置安装顺序并生成最终步骤
        /// </summary>
        public static IEnumerator LunchActionSystem(IActionStap[] steps)
        {
            Debug.Assert(steps != null);
            if (Instance == null)
            {
                yield return new WaitUntil(() => Instance != null);
            }
            else if (Instance.remoteController == null)
            {
                yield return new WaitUntil(() => Instance.activeCommand != null);
                steps = ConfigSteps(Instance.activeCommand, steps);//重新计算步骤
                Instance.activeCommand = GetIActionCommandList(Instance.activeCommand, steps);
                Instance.remoteController = new RemoteController(Instance.activeCommand);
                Instance.steps = steps;
            }
        }
        /// <summary>
        /// 打开或关闭绑定脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isOn"></param>
        public void InsertScript<S, T>(bool isOn) where T : MonoBehaviour where S : MonoBehaviour
        {
            var items = TransUtil.FindComponentsInChild<S>(transform);
            foreach (var item in items)
            {
                T titem = item.gameObject.GetComponent<T>();
                if (isOn && titem == null)
                {
                    item.gameObject.AddComponent<T>();
                }
                else if (!isOn && titem != null)
                {
                    Destroy(titem);
                }
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
        private static IActionStap[] ConfigSteps(List<IActionCommand> commandList, IActionStap[] steps)
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
            Debug.Log("[Ignored steps:]" + String.Join("|",ignored.ToArray()));
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