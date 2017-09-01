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
        public ActionHolder[] ActionHolders { get { return actionHolders.ToArray(); } }

        private IRemoteController remoteController;
        private IActionStap[] steps;
        private List<ActionHolder> actionHolders = new List<ActionHolder>();
        private List<IActionCommand> commandDic = new List<IActionCommand>();

        void Awake()
        {
            Instance = this;

            foreach (Transform item in transform)
            {
                if (!item.gameObject.activeSelf) continue;
                ActionHolder holder = item.GetComponent<ActionHolder>();
                if (holder != null)
                {
                    holder.OnStepEnd = OnStepComplete;
                    holder.OnRegistCommand = OnRegistCommand;
                    holder.onUserErr = OnUserError;
                    actionHolders.Add(holder);
                }
            }
        }



        #region Public Functions
        /// <summary>
        /// 设置安装顺序并生成最终步骤
        /// </summary>
        public static IEnumerator LunchActionSystem(IActionStap[] steps)
        {
            Debug.Assert(steps != null);

            yield return new WaitUntil(() => Instance != null);

            if (Instance.remoteController != null)
            {
                yield break;
            }
            else
            {
                for (int i = 0; i < Instance.actionHolders.Count; i++)
                {
                    yield return new WaitUntil(() => Instance.actionHolders[i].Registed);
                }

                if (steps.Length != Instance.commandDic.Count)
                {
                    steps = ConfigSteps(Instance.commandDic, steps);
                }

                var actionCommandList = GetIActionCommandList(Instance.commandDic, steps);
                Instance.remoteController = new RemoteController(actionCommandList);
            }
            Instance.SwitchHighLight(Setting.highLightOpen);
            Instance.steps = steps;
        }

        /// <summary>
        /// 开启或关闭高亮提示
        /// </summary>
        /// <param name="isOn"></param>
        public void SwitchHighLight(bool isOn)
        {
            foreach (var item in actionHolders)
            {
                item.SetHighLight(isOn);
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
        /// 注册命令
        /// </summary>
        /// <param name="arg0"></param>
        private void OnRegistCommand(IActionCommand arg0)
        {
            commandDic.Add(arg0);
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
        private static IActionStap[] ConfigSteps(List<IActionCommand> commandDic, IActionStap[] steps)
        {
            if (string.Compare(commandDic.Count.ToString(), steps.Length.ToString()) != 0)
            {
                Debug.Log("count" + commandDic.Count + steps.Length.ToString());
            }
            List<IActionStap> activeStaps = new List<IActionStap>();
            for (int i = 0; i < steps.Length; i++)
            {
                var old = commandDic.Find(x => x.StepName == steps[i].StapName);
                if (old != null)
                {
                    activeStaps.Add(steps[i]);
                }
                else
                {
                    Debug.Log("[Ignored step:]" + steps[i].StapName);
                }
            }
            return activeStaps.ToArray();
        }
        /// <summary>
        /// 得到排序后的命令列表
        /// </summary>
        /// <returns></returns>
        private static List<IActionCommand> GetIActionCommandList(List<IActionCommand> commandDic, IActionStap[] steps)
        {
            var actionCommandList = new List<IActionCommand>();
            foreach (var item in steps)
            {
                var old = commandDic.Find(x => x.StepName == item.StapName);
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