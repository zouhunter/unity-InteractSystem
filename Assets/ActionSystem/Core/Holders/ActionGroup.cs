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
        protected List<OptionalCommandItem> actionCommands = new List<OptionalCommandItem>();
        [SerializeField]
        public List<RunTimePrefabItem> runTimeElements = new List<RunTimePrefabItem>();

        private List<ActionCommand> _activeCommands;
        #region Propertys
        public List<ActionCommand> activeCommands
        {
            get
            {
                if (_activeCommands == null)
                {
                    _activeCommands = actionCommands.Where(x => x.active).Select(x => Instantiate(x.prefab)).ToList();
                    foreach (var command in _activeCommands)
                    {
                        command.SetContext(this);
                        command.RegistAsOperate(EventTransfer.OnUserError);
                        command.RegistComplete(EventTransfer.OnStepComplete);
                        command.RegistCommandChanged(EventTransfer.OnCommandExectute);
                    }
                }
                return _activeCommands;
            }
        }
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
            ActionSystem.RegistGroup(this);
        }
        private void OnDestroy()
        {
            ElementController.Instence.RemoveRunTimeElements(runTimeElements);
            ActionSystem.RemoveGroup(this);
        }
        #endregion

        #region Public Functions

        /// <summary>
        /// 默认的按command名称进行排序
        /// </summary>
        public ICommandController LunchActionSystem()
        {
            var steps = activeCommands.ConvertAll<string>(x => x.StepName);
            steps.Sort();
            RemoteController = new LineCommandController(activeCommands);
            return RemoteController;
        }
        /// <summary>
        /// 设置安装顺序并生成最终步骤
        /// </summary>
        public ICommandController LunchActionSystem(string[] steps, out string[] stepsWorp)
        {
            //重新计算步骤
            var commands = WorpCommandList(activeCommands, steps);
            RemoteController = new LineCommandController(commands);
            stepsWorp = commands.ConvertAll<string>(x => x.StepName).ToArray();
            return RemoteController;
        }
        /// <summary>
        /// 传入command名称关联字典
        /// </summary>
        /// <param name="rule"></param>
        public ICommandController LunchActionSystem(Dictionary<string, string[]> rule)
        {
            RemoteController = new TreeCommandController(rule, activeCommands);
            return RemoteController;
        }
        #endregion

        #region private Funtions

        private static List<ActionCommand> WorpCommandList(List<ActionCommand> commandList, string[] steps)
        {
            List<ActionCommand> worpedCommands = new List<ActionCommand>();
            List<string> ignored = new List<string>();
            for (int i = 0; i < steps.Length; i++)
            {
                var command = commandList.Find(x => x.StepName == steps[i]);
                if (command != null)
                {
                    worpedCommands.Add(command);
                }
                else
                {
                    ignored.Add(steps[i]);
                }
            }
            Debug.Log("[Ignored steps:]" + String.Join("|", ignored.ToArray()));
            return worpedCommands;
        }
        #endregion

    }

}