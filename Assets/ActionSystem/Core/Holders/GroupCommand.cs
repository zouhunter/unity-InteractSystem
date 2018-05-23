//using System;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Collections;
//using System.Collections.Generic;

//namespace WorldActionSystem
//{
//    /// <summary>
//    /// 并行执行
//    /// </summary>
//    public class GroupCommand : ActionCommand
//    {
//        public string StepName { get; private set; }
//        public bool Started
//        {
//            get
//            {
//                return started;
//            }
//        }
//        public bool Completed
//        {
//            get
//            {
//                return completed;
//            }
//        }
//        public List<ActionCommand> CommandList { get { return commandList; } }
//        public ActionObjCtroller ActionObjCtrl
//        {
//            get
//            {
//                if (currentCmd == null) return null;
//                return currentCmd.ActionObjCtrl;
//            }
//        }
//        private ActionCommand currentCmd
//        {
//            get
//            {
//                if (currentCmds.Count == 0) return null; return currentCmds[0];
//            }
//        }
//        private List<ActionCommand> commandList;
//        private Queue<ActionCommand> cmdQueue = new Queue<ActionCommand>();
//        protected List<int> queueID = new List<int>();
//        private List<ActionCommand> currentCmds = new List<ActionCommand>();
//        private bool forceAuto;
//        private bool started;
//        private bool completed;
//        private bool log = false;

//        private UnityAction<ActionCommand> onEndExecute { get; set; }
//        private UnityAction<string, int, int> onActionObjStartExecute { get; set; }
//        private Events.OperateErrorAction onUserError { get; set; }

//        public GroupCommand(string stepName, List<ActionCommand> commandList)
//        {
//            StepName = stepName;
//            this.commandList = CreateCommandList(commandList);
//            InitCommands();
//        }

//        /// <summary>
//        /// 实现command的复制
//        /// </summary>
//        /// <param name="commands"></param>
//        /// <returns></returns>
//        private List<ActionCommand> CreateCommandList(List<ActionCommand> commands)
//        {
//            var list = new List<ActionCommand>();
//            foreach (var cmd in commands)
//            {
//                list.Add(cmd);
//                if (cmd.CopyCount > 0)
//                {
//                    for (int i = 0; i < cmd.CopyCount; i++)
//                    {
//                        list.Add(CreateCommandCopy(cmd));
//                    }
//                }
//            }
//            return list;
//        }
//        private ActionCommand CreateCommandCopy(ActionCommand cmd)
//        {
//            var newCmd = UnityEngine.Object.Instantiate(cmd);
//            newCmd.name = cmd.name;
//            return newCmd;
//        }

//        private void InitCommands()
//        {
//            foreach (var item in commandList)
//            {
//                var cmd = item;
//                cmd.RegistAsOperate(onUserError);
//                cmd.RegistComplete(OnCommandObjComplete);
//                cmd.RegistCommandChanged((x,count,currentID) =>{
//                    OnCommandStartExecute(x, commandList.Count, commandList.IndexOf(cmd));
//                });
//            }

//        }
//        public bool StartExecute(bool forceAuto)
//        {
//            if (!started)
//            {
//                started = true;
//                this.forceAuto = forceAuto;
//                ChargeQueueIDs();
//                ExecuteAStep(forceAuto);
//                return true;
//            }
//            else
//            {
//                Debug.Log("already started" + StepName);
//                return false;
//            }
//        }
//        public bool EndExecute()
//        {
//            if (!completed)
//            {
//                completed = true;
//                commandList.Sort();
//                for (int i = 0; i < commandList.Count; i++)
//                {
//                    var item = commandList[i];
//                    Debug.Log("Group:" + item);
//                    if (!item.Started)
//                    {
//                        item.StartExecute(forceAuto);
//                    }
//                    if (!item.Completed)
//                    {
//                        item.EndExecute();
//                    }
//                }
               
//                OnEndExecute();
//                return true;
//            }
//            else
//            {
//                Debug.Log("already complete" + StepName);
//                return false;
//            }
//        }
//        public void UnDoExecute()
//        {
//            started = false;
//            completed = false;
//            for (int i = commandList.Count - 1; i >= 0; i--)
//            {
//                var item = commandList[i];
//                if (item.Started)
//                {
//                    item.UnDoExecute();
//                }
//            }

//        }

//        internal void RegistComplete(UnityAction<ActionCommand> onOneCommandComplete)
//        {
//            this.onEndExecute = onOneCommandComplete;
//        }

//        internal void RegistCommandChanged(UnityAction<string, int, int> onActionObjStartExecute)
//        {
//            this.onActionObjStartExecute = onActionObjStartExecute;
//        }

//        internal void RegistAsOperate(Events. OperateErrorAction onUserError)
//        {
//            this.onUserError = onUserError;
//        }

//        protected bool ExecuteAStep(bool forceAuto)
//        {
//            if (queueID.Count > 0)
//            {
//                var id = queueID[0];
//                queueID.RemoveAt(0);
//                var neetActive = commandList.FindAll(x => (x as ActionCommand).QueueID == id);

//                if (forceAuto)
//                {
//                    cmdQueue.Clear();
//                    foreach (var item in neetActive)
//                    {
//                        cmdQueue.Enqueue(item);
//                    }
//                    QueueExectueCommands();
//                }
//                else
//                {
//                    foreach (ActionCommand item in neetActive)
//                    {
//                        TryStartOneCommand(item);
//                    }
//                }

//                return true;
//            }
//            return false;
//        }

//        private void OnCommandStartExecute(string stepName, int totalCount, int currentCommand)
//        {
//            if (onActionObjStartExecute != null)
//            {
//                onActionObjStartExecute.Invoke(stepName, totalCount, currentCommand);
//            }
//        }

//        private void OnCommandObjComplete(ActionCommand cmd)
//        {
//            if(log) Debug.Log("OnCommandObjComplete:" + cmd);
//            if (currentCmds.Contains(cmd))
//            {
//                currentCmds.Remove(cmd);
//            }

//            if (!Completed)
//            {
//                var notComplete = commandList.FindAll(x => (x as ActionCommand).QueueID == cmd.QueueID && !x.Completed);
//                if (notComplete.Count == 0)
//                {
//                    if (!ExecuteAStep(forceAuto) && !Completed)
//                    {
//                        OnEndExecute();
//                    }
//                }
//                else if (cmdQueue.Count > 0)//正在循环执行
//                {
//                    QueueExectueCommands();
//                }
//            }

//        }
//        protected void QueueExectueCommands()
//        {
//            if (cmdQueue.Count > 0)
//            {
//                var cmd = cmdQueue.Dequeue();
//                if (log)
//                    Debug.Log("QueueExectueCmd:" + cmd);
//                TryStartOneCommand(cmd);
//            }
//        }
//        private void TryStartOneCommand(ActionCommand cmd)
//        {
//            if (log) Debug.Log("Start Cmd:" + cmd);

//            if (!cmd.Started)
//            {
//                cmd.StartExecute(forceAuto);
//                if (!currentCmds.Contains(cmd))
//                {
//                    currentCmds.Add(cmd);
//                }
//            }
//            else
//            {
//                Debug.LogError(cmd + " allready started");
//            }

//        }

//        private void ChargeQueueIDs()
//        {
//            cmdQueue.Clear();
//            queueID.Clear();
//            foreach (ActionCommand item in commandList)
//            {
//                if (!queueID.Contains(item.QueueID))
//                {
//                    queueID.Add(item.QueueID);
//                }
//            }
//            queueID.Sort();
//        }

//        public void OnEndExecute()
//        {
//            if (this.onEndExecute != null && !Completed)
//            {
//                onEndExecute(this);
//            }
//        }
//    }

//}