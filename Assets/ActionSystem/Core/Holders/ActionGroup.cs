using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WorldActionSystem
{
    [AddComponentMenu(MenuName.ActionGroup)]
    public class ActionGroup : ScriptableObject
    {
        [SerializeField]
        private List<OptionalCommandItem> actionCommands = new List<OptionalCommandItem>();
        [SerializeField]
        public List<RunTimePrefabItem> runTimeElements = new List<RunTimePrefabItem>();

        #region Propertys
        public List<ActionCommand> ActiveCommands { get; private set; }
        public ICommandController RemoteController { get; private set; }
        public EventController EventCtrl { get; private set; }
        public EventTransfer EventTransfer { get; private set; }
        #endregion

        #region UnityFunctions
        public ActionGroup()
        {
            EventTransfer = new EventTransfer(this);
        }

        private void OnEnable()
        {
            ElementController.Instence.RegistRunTimeElements(runTimeElements);
            ActionSystem.Instence.RegistGroup(this);
        }
        private void OnDestroy()
        {
            ElementController.Instence.RemoveRunTimeElements(runTimeElements);
            ActionSystem.Instence.RemoveGroup(this);
        }
        #endregion

        #region Public Functions

        /// <summary>
        /// 默认的按command名称进行排序
        /// </summary>
        public ICommandController LunchActionSystem()
        {
            InitAcitveCommands();
            var steps = ActiveCommands.ConvertAll<string>(x => x.StepName);
            steps.Sort();
            ActiveCommands = GetIActionCommandList(ActiveCommands, steps.ToArray());
            RemoteController = new LineCommandController(ActiveCommands);
            return RemoteController;
        }
        /// <summary>
        /// 设置安装顺序并生成最终步骤
        /// </summary>
        public ICommandController LunchActionSystem(string[] steps)
        {
            InitAcitveCommands();
            var stepsWorp = ConfigSteps(ActiveCommands, steps);//重新计算步骤
            ActiveCommands = GetIActionCommandList(ActiveCommands, stepsWorp);
            RemoteController = new LineCommandController(ActiveCommands);
            return RemoteController;
        }
        /// <summary>
        /// 传入command名称关联字典
        /// </summary>
        /// <param name="rule"></param>
        public ICommandController LunchActionSystem(Dictionary<string, string[]> rule)
        {
            InitAcitveCommands();
            var steps = ActiveCommands.ConvertAll<string>(x => x.StepName);
            steps.Sort();
            ActiveCommands = GetIActionCommandList(ActiveCommands, steps.ToArray());
            RemoteController = new TreeCommandController(rule, ActiveCommands);
            return RemoteController;
        }
        /// <summary>
        /// 设置安装顺序并生成最终步骤
        /// </summary>
        public ICommandController LunchActionSystem<T>(T[] steps) where T : IActionStap
        {
            InitAcitveCommands();
            var stepsWorp = ConfigSteps<T>(ActiveCommands, steps);//重新计算步骤
            ActiveCommands = GetIActionCommandList(ActiveCommands, Array.ConvertAll<IActionStap, string>(stepsWorp, x => x.StapName));
            RemoteController = new LineCommandController(ActiveCommands);
            return RemoteController;
        }
        #endregion

        #region private Funtions
        private void InitAcitveCommands()
        {
            if(ActiveCommands == null){
                ActiveCommands = actionCommands.Where(x => x.active).Select(x => x.prefab).ToList();
            }
        }
        private static string[] ConfigSteps(List<ActionCommand> commandList, string[] steps)
        {
            List<string> activeStaps = new List<string>();
            List<string> ignored = new List<string>();
            for (int i = 0; i < steps.Length; i++)
            {
                var old = commandList.Find(x => x.StepName == steps[i]);
                if (old != null)
                {
                    activeStaps.Add(steps[i]);
                }
                else
                {
                    ignored.Add(steps[i]);
                }
            }
            Debug.Log("[Ignored steps:]" + String.Join("|", ignored.ToArray()));
            return activeStaps.ToArray();
        }
        private static IActionStap[] ConfigSteps<T>(List<ActionCommand> commandList, T[] steps) where T : IActionStap
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
            if (ignored.Count > 0) Debug.LogWarning("[Ignored steps:]" + String.Join("|", ignored.ToArray()));
            return activeStaps.ToArray();
        }
        private static List<ActionCommand> GetIActionCommandList(List<ActionCommand> commandList, string[] steps)
        {
            var actionCommandList = new List<ActionCommand>();
            foreach (var item in steps)
            {
                var old = commandList.Find(x => x.StepName == item);
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