using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InteractSystem
{
    [AddComponentMenu(MenuName.ActionGroup)]
    public class ActionGroup : MonoBehaviour
    {
        [SerializeField]//步骤控制
        protected OptionalCommandItem[] actionCommands;
        [SerializeField]//用户创建元素
        protected RunTimePrefabItem[] runTimeElements;
        [SerializeField]//自动创建元素
        protected AutoPrefabItem[] autoElements;
        [SerializeField]//环境元素
        protected Enviroment.EnviromentItem[] enviroments;
      
        #region Propertys
        public List<ActionCommand> activeCommands { get; private set; }
        public ICommandController RemoteController { get; private set; }
        public EventController EventCtrl { get; private set; }
        public EventTransfer EventTransfer { get; private set; }
        public Enviroment.EnviromentCtrl enviromentCtrl { get; private set; }
        public AutoElementCtrl autoElementCtrl { get; private set; }
        #endregion

        #region UnityFunctions
        public ActionGroup()
        {
            EventTransfer = new EventTransfer(this);
        }

        private void OnEnable()
        {
            InitAutoElementCtrl();
            InitEnviromentCtrl();
            InitActionCommands();
            ActionSystem.RegistGroup(this);
            ElementController.Instence.RegistRunTimeElements(runTimeElements);
        }

        private void InitAutoElementCtrl()
        {
            autoElementCtrl = new AutoElementCtrl(transform,autoElements);
            autoElementCtrl.Create();
        }

        private void InitEnviromentCtrl()
        {
            enviromentCtrl = new Enviroment.EnviromentCtrl(enviroments);
            enviromentCtrl.SetContext(this);
        }

        private void OnDestroy()
        {
            ActionSystem.RemoveGroup(this);
            ElementController.Instence.RemoveRunTimeElements(runTimeElements);
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
        private void InitActionCommands()
        {
            activeCommands = actionCommands.Where(x => !x.ignore).Select(x => x.command).ToList();
            foreach (var command in activeCommands)
            {
                command.SetContext(this);
                command.RegistAsOperate(EventTransfer.OnUserError);
                command.RegistComplete(EventTransfer.OnStepComplete);
                command.RegistCommandChanged(EventTransfer.OnCommandExectute);
            }
        }

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