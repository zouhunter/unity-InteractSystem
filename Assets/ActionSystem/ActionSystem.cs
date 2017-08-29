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
        public static ActionSystem Instance;
        public event UserError onUserError;//步骤操作错误
        public IRemoteController RemoteController { get { return remoteController; } }
        public IActionStap[] ActiveStaps { get { return staps; } }
        public ActionHolder[] ActionHolders { get { return actionHolders.ToArray(); } }

        private IRemoteController remoteController;
        private IActionStap[] staps;
        private List<ActionHolder> actionHolders = new List<ActionHolder>();
        private Dictionary<string, ActionCommand> commandDic = new Dictionary<string, ActionCommand>();

        void Awake(){
            Instance = this;

            foreach (Transform item in transform)
            {
                ActionHolder holder = item.GetComponent<ActionHolder>();
                if (holder != null)
                {
                    holder.registFunc = (cmd) => {
                        commandDic.Add(cmd.StapName, cmd);
                    };
                    holder.onUserErr = (x, y) =>
                     {
                         if (onUserError != null)
                             onUserError(x, y);
                     };
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

            yield return new WaitUntil(()=>Instance != null);

            if (Instance.remoteController != null){
                yield break;
            }
            else
            {
                for (int i = 0; i < Instance.actionHolders.Count; i++){
                    yield return new WaitUntil(()=>Instance.actionHolders[i].Registed);
                }

                if (staps.Length != Instance.commandDic.Count){
                    staps = ConfigSteps(Instance.commandDic, staps);
                }

                var actionCommandList = GetActionCommandList(Instance.commandDic, staps);
                Instance.remoteController = new RemoteController(actionCommandList);
            }

            Instance.staps = staps;
        }

        #endregion

        #region private Funtions
        /// <summary>
        /// 重置步骤
        /// </summary>
        /// <param name="commandDic"></param>
        /// <param name="staps"></param>
        /// <returns></returns>
        private static IActionStap[] ConfigSteps(Dictionary<string, ActionCommand> commandDic, IActionStap[] staps)
        {
            if (string.Compare(commandDic.Count.ToString(), staps.Length.ToString()) != 0)
            {
                Debug.Log("count" + commandDic.Count + staps.Length.ToString());
            }
            List<IActionStap> activeStaps = new List<IActionStap>();
            for (int i = 0; i < staps.Length; i++)
            {
                if (commandDic.ContainsKey(staps[i].StapName))
                {
                    activeStaps.Add(staps[i]);
                }
                else
                {
                    Debug.Log("Ignore + stap" + staps[i].StapName);
                }
            }
            return activeStaps.ToArray();
        }
        /// <summary>
        /// 得到排序后的命令列表
        /// </summary>
        /// <returns></returns>
        private static List<ActionCommand> GetActionCommandList(Dictionary<string, ActionCommand> commandDic, IActionStap[] staps)
        {
            ActionCommand cmd;
            var actionCommandList = new List<ActionCommand>();
            foreach (var item in staps)
            {
                if (commandDic.TryGetValue(item.StapName, out cmd))
                {
                    actionCommandList.Add(cmd);
                }
            }
            return actionCommandList;
        }
        #endregion
    }

}