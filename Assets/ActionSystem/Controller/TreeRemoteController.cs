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

    public class TreeRemoteController : IRemoteController
    {
        public UnityAction onEndExecute;
        IActionCommand rootCommand;
        Dictionary<IActionCommand, List<IActionCommand>> commandDic;
        Dictionary<IActionCommand, IActionCommand> parentDic;
        Stack<IActionCommand> executedCommands = new Stack<IActionCommand>();//正向执行过了记录
        Stack<IActionCommand> backupCommands = new Stack<IActionCommand>();//回退时记录
        List<IActionCommand> activeCommands = new List<IActionCommand>();
        public IActionCommand CurrCommand { get; private set; }

        public TreeRemoteController(Dictionary<string, string[]> rule, List<IActionCommand> commandList)
        {
            this.commandDic = new Dictionary<IActionCommand, List<IActionCommand>>();
            this.parentDic = new Dictionary<IActionCommand, IActionCommand>();
            foreach (var item in rule)
            {
                var key = commandList.Find(x => x.StepName == item.Key);
                var values = new List<IActionCommand>();
                foreach (var child in item.Value)
                {
                    var c = commandList.Find(x => x.StepName == child);
                    parentDic.Add(c, key);
                    values.Add(c);
                }
                commandDic[key] = values;
            }
            rootCommand = SurchRootCommand(parentDic);
        }

        public TreeRemoteController(Dictionary<IActionCommand, List<IActionCommand>> commandDic)
        {
            this.commandDic = new Dictionary<IActionCommand, List<IActionCommand>>();
            this.parentDic = new Dictionary<IActionCommand, IActionCommand>();
            foreach (var item in commandDic)
            {
                commandDic[item.Key] = new List<IActionCommand>(item.Value);
                foreach (var child in item.Value)
                {
                    parentDic.Add(child, item.Key);
                }
            }
            rootCommand = SurchRootCommand(parentDic);
        }

        /// <summary>
        /// 开启一个命令,并返回正常执行与否
        /// </summary>
        public bool StartExecuteCommand(UnityAction onEndExecute, bool forceAuto)
        {
            Debug.Assert(rootCommand != null, "root is Empty");

            if (activeCommands.Count == 0)
            {
                this.onEndExecute = onEndExecute;

                if (CurrCommand == null)
                {
                    if (!rootCommand.Startd)
                    {
                        activeCommands.Add(rootCommand);
                        rootCommand.StartExecute(forceAuto);
                    }
                }
                else
                {
                    if (commandDic.ContainsKey(CurrCommand))
                    {
                        foreach (var cmd in commandDic[CurrCommand])
                        {
                            if (!cmd.Startd)
                            {
                                activeCommands.Add(cmd);
                                cmd.StartExecute(forceAuto);
                            }
                        }
                    }
                }

                return true;
            }
            else
            {
                this.onEndExecute = onEndExecute;
                return false;
            }
        }


        /// <summary>
        /// 结束已经开始的命令
        /// </summary>
        public bool EndExecuteCommand()
        {
            if (activeCommands.Count == 0)
            {
                if (backupCommands.Count > 0)
                {
                    CurrCommand = backupCommands.Pop();
                    CurrCommand.StartExecute(false);
                    CurrCommand.EndExecute();

                    if (onEndExecute != null)
                    {
                        onEndExecute();
                    }

                    return true;
                }
                return false;
            }
            else
            {
                if (backupCommands.Count > 0)
                {
                    backupCommands.Clear();
                }
                return false;
            }


        }

        /// <summary>
        /// 结束已经开始的命令
        /// </summary>
        public void OnEndExecuteCommand(string step)
        {
            if (activeCommands.Count > 0)
            {
                foreach (var item in activeCommands)
                {
                    if (step == item.StepName)
                    {
                        CurrCommand = item;
                        executedCommands.Push(item);
                    }
                    else
                    {
                        item.UnDoExecute();
                    }
                }
            }

            activeCommands.Clear();

            if (onEndExecute != null)
            {
                onEndExecute();
            }
        }

        /// <summary>
        /// 撤消操作，并返回能否继续撤销
        /// </summary>
        /// <returns></returns>
        public bool UnDoCommand()
        {
            if (activeCommands.Count > 0)
            {
                foreach (var cmd in activeCommands)
                {
                    cmd.UnDoExecute();
                }
            }

            if (executedCommands.Count > 0)
            {
                CurrCommand = executedCommands.Pop();
                CurrCommand.UnDoExecute();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 执行多个步骤
        /// </summary>
        /// <param name="step"></param>
        public bool ExecuteMutliCommand(int step)
        {
            return false;
        }

        /// <summary>
        /// 回到命令列表起始
        /// </summary>
        public void ToAllCommandStart()
        {
            while (executedCommands.Count >0)
            {
                UnDoCommand();
            }
        }

        /// <summary>
        /// 完成所有命令
        /// </summary>
        public void ToAllCommandEnd()
        {
            while (backupCommands.Count > 0)
            {
                EndExecuteCommand();
            }
        }

        /// <summary>
        /// 执行到指定的步骤
        /// </summary>
        /// <param name="stepName"></param>
        public bool ToTargetCommand(string stepName)
        {
            var commandList = new List<IActionCommand>();
            return false;
        }

        private static IActionCommand SurchRootCommand(Dictionary<IActionCommand, IActionCommand> parentDic)
        {
            var parent = parentDic.Keys.First();
            while (parentDic.ContainsKey(parent))
            {
                var newParent = parentDic[parent];
                if (newParent != parent)
                {
                    parent = newParent;
                }
                else
                {
                    break;
                }
            }
            return parent;
        }
    }

}