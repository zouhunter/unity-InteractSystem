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
        public IActionStap[] ActiveStaps { get { return staps; } }
        public ActionHolder[] ActionHolders { get { return actionHolders.ToArray(); } }

        private IRemoteController remoteController;
        private IActionStap[] staps;
        private List<ActionHolder> actionHolders = new List<ActionHolder>();
        private List<ActionCommand> commandDic = new List<ActionCommand>();

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
        public static IEnumerator LunchActionSystem(IActionStap[] staps)
        {
            Debug.Assert(staps != null);

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

                if (staps.Length != Instance.commandDic.Count)
                {
                    staps = ConfigSteps(Instance.commandDic, staps);
                }

                var actionCommandList = GetActionCommandList(Instance.commandDic, staps);
                Instance.remoteController = new RemoteController(actionCommandList);
            }
            Instance.SwitchHighLight(Setting.highLightOpen);
            Instance.staps = staps;
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
        private void OnRegistCommand(ActionCommand arg0)
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
        /// <param name="staps"></param>
        /// <returns></returns>
        private static IActionStap[] ConfigSteps(List<ActionCommand> commandDic, IActionStap[] staps)
        {
            if (string.Compare(commandDic.Count.ToString(), staps.Length.ToString()) != 0)
            {
                Debug.Log("count" + commandDic.Count + staps.Length.ToString());
            }
            List<IActionStap> activeStaps = new List<IActionStap>();
            for (int i = 0; i < staps.Length; i++)
            {
                var old = commandDic.Find(x => x.StepName == staps[i].StapName);
                if (old != null)
                {
                    activeStaps.Add(staps[i]);
                }
                else
                {
                    Debug.Log("[Ignored stap:]" + staps[i].StapName);
                }
            }
            return activeStaps.ToArray();
        }
        /// <summary>
        /// 得到排序后的命令列表
        /// </summary>
        /// <returns></returns>
        private static List<ActionCommand> GetActionCommandList(List<ActionCommand> commandDic, IActionStap[] staps)
        {
            var actionCommandList = new List<ActionCommand>();
            foreach (var item in staps)
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