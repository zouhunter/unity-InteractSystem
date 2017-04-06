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
        public IRemoteController RemoteController { get; private set; }
        public event UnityAction<string,string> onStepErr;//步骤操作错误

        private List<ActionHolder> actionHolders = new List<ActionHolder>();
        private UnityAction<List<IActionStap>> onStapsActive;
        private IActionStap[] staps;
        private Dictionary<string, ActionCommand> commandDic = new Dictionary<string, ActionCommand>();
        private List<ActionCommand> actionCommandList = new List<ActionCommand>();

        void Awake(){
            Instance = this;
            foreach (Transform item in transform){
                ActionHolder holder = item.GetComponent<ActionHolder>();
                if (holder!= null)
                {
                    holder.registFunc = AddActionCommand;
                    holder.onUserErr = OnUserErr;
                    actionHolders.Add(holder);
                }
            }
        }

        #region Public Functions
        /// <summary>
        /// 获取控制器
        /// </summary>
        /// <returns></returns>
        public void GetRemoteController(UnityAction<IRemoteController> onCtrlCreate)
        {
            if (RemoteController != null)
            {
                onCtrlCreate(RemoteController);
            }
            else if (staps != null && staps.Length == commandDic.Count)
            {
                GetActionCommandList();
                RemoteController = new RemoteController(actionCommandList);
                onCtrlCreate(RemoteController);
            }
            else
            {
                StartCoroutine(WaitToCreateRemoteCtrl(onCtrlCreate));
            }
        }

        /// <summary>
        /// 设置安装顺序并生成最终步骤
        /// </summary>
        public void SetActionStaps(IActionStap[] staps, UnityAction<List<IActionStap>> onStapsActive)
        {
            this.staps = staps;
            this.onStapsActive = onStapsActive;
        }
        
        /// <summary>
        /// 设置高亮显示
        /// </summary>
        /// <param name="on"></param>
        public void SetHighLight(bool on)
        {
            for (int i = 0; i < actionHolders.Count; i++)
            {
                actionHolders[i].SetHighLight(on);
            }
        }

        /// <summary>
        /// 设置文字提示
        /// </summary>
        /// <param name="isOn"></param>
        public void InsertScript<T>(bool isOn) where T:MonoBehaviour
        {
            for (int i = 0; i < actionHolders.Count; i++)
            {
                actionHolders[i].InsertScript<T>(isOn);
            }
        }
        #endregion

        #region private Funtions
        IEnumerator WaitToCreateRemoteCtrl(UnityAction<IRemoteController> onCtrlCreate)
        {
            yield return WaitReConfig();
            GetActionCommandList();
            RemoteController = new RemoteController(actionCommandList);
            onCtrlCreate(RemoteController);
        }

        IEnumerator WaitReConfig()
        {
            yield return new WaitUntil(() => staps != null);

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
            if (onStapsActive != null)
            {
                onStapsActive(activeStaps);
            }
        }
        
        /// <summary>
        /// 添加命令
        /// </summary>
        private void AddActionCommand(ActionCommand cmd)
        {
            commandDic.Add(cmd.StapName, cmd);
        }
        
        /// <summary>
        /// 用户操作错误触发
        /// </summary>
        /// <param name="step"></param>
        /// <param name="err"></param>
        private void OnUserErr(string step,string err)
        {
            if (onStepErr != null)
            {
                onStepErr.Invoke(step,err);
            }
        }

        /// <summary>
        /// 得到排序后的命令列表
        /// </summary>
        /// <returns></returns>
        private List<ActionCommand> GetActionCommandList()
        {
            ActionCommand cmd;
            actionCommandList.Clear();
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