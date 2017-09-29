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
        private static ActionSystem instance = default(ActionSystem);
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
        public ElementController ElementController { get { return elementController; } }
        public IActionStap[] ActiveStaps { get { return steps; } }
        private IRemoteController remoteController;
        private IActionStap[] steps;
        private CommandController commandCtrl = new WorldActionSystem.CommandController();
        private ElementController elementController = new WorldActionSystem.ElementController();//元素名、列表
        private List<IActionCommand> activeCommands;
        private RegistCmds onCommandRegist;

        public List<ActionPrefabItem> prefabList = new List<ActionPrefabItem>();
        
        #region Interface Fuctions

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            OnCreateCommandAndElements();
        }
        private void OnCreateCommandAndElements()
        {
            var cmds = new List<ActionCommand>();
            CreateActionObjects((cmd)=> {
                cmd.RegistAsOperate(OnUserError, () => { return elementController; });
                cmds.Add(cmd);}, (pick)=> {
                elementController.RegistElement(pick);
            });
           
            activeCommands = commandCtrl.RegistTriggers(cmds.ToArray(), OnStepComplete);
            if (onCommandRegist != null) onCommandRegist.Invoke(activeCommands);
        }
        #endregion

        #region Public Functions

        /// <summary>
        /// 设置安装顺序并生成最终步骤
        /// </summary>
        public static IEnumerator LunchActionSystem<T>(T[] steps, UnityAction<ActionSystem, T[]> onLunchOK) where T : IActionStap
        {
            Debug.Assert(steps != null);
            yield return new WaitUntil(() => Instance != null);

            Instance.onCommandRegist = (commandList) =>
            {
                Instance.steps = ConfigSteps<T>(Instance.activeCommands, steps);//重新计算步骤
                Instance.activeCommands = GetIActionCommandList(Instance.activeCommands, Instance.steps);
                Instance.remoteController = new RemoteController(Instance.activeCommands);
                onLunchOK.Invoke(Instance, Array.ConvertAll<IActionStap, T>(Instance.steps, x => (T)x));
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
        private void OnStepComplete(string stempName)
        {
            remoteController.OnEndExecuteCommand();
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

        /// <summary>
        /// 当完成命令对象注册
        /// </summary>
        /// <param name="cmdList"></param>
        private void OnCommandRegistComplete(List<IActionCommand> cmdList)
        {
            instance.activeCommands = cmdList;
            if (onCommandRegist != null) onCommandRegist.Invoke(cmdList);
        }

        internal  void CreateActionObjects(UnityAction<ActionCommand> onCreateCommand, UnityAction<PickUpAbleElement> onCreateElement)
        {
            foreach (var item in prefabList)
            {
                item.prefab.gameObject.SetActive(true);
                var created = GameObject.Instantiate(item.prefab);
                created.name = item.prefab.name;
                if (item.reparent && item.parent != null)
                {
                    transform.SetParent(item.parent,false);
                }
                else
                {
                    created.transform.SetParent(transform,false);
                }

                if(item.rematrix)
                {
                    TransUtil.LoadmatrixInfo(item.matrix, created.transform);
                }


                if (item.containsCommand)
                {
                    RetriveCommand(created.transform, onCreateCommand);
                }
                if (item.containsPickAble)
                {
                    RetivePickElement(created.transform, onCreateElement);
                }
            }
        }

        private void RetriveCommand(Transform trans,UnityAction<ActionCommand> onRetive)
        {
            if (!trans.gameObject.activeSelf) return;
            var com = trans.GetComponent<ActionCommand>();
            if(com)
            {
                onRetive(com);
                return;
            }
            else
            {

                foreach (Transform child in trans)
                {
                    RetriveCommand(child, onRetive);
                }
            }

        }

        private void RetivePickElement(Transform trans, UnityAction<PickUpAbleElement> onRetive)
        {
            if (!trans.gameObject.activeSelf) return;
            var com = trans.GetComponent<PickUpAbleElement>();
            if (com)
            {
                onRetive(com);
                return;
            }
            else
            {
                foreach (Transform child in trans)
                {
                    RetivePickElement(child, onRetive);
                }
            }

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