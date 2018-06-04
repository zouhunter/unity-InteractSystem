using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using WorldActionSystem.Graph;

namespace WorldActionSystem.Structure
{
    public class ActionStateMechine
    {
        public bool IsAuto { get; private set; }
        public ExecuteState currentState { get; private set; }
        public Dictionary<State, ExecuteState> statemap = new Dictionary<State, ExecuteState>();
        public Dictionary<ExecuteUnit, UnitStatus> statuDic = new Dictionary<ExecuteUnit, UnitStatus>();
        public ExecuteUnit root;
        protected ActionGroup Context { get { return Cmd.Context; } }
        public ActionCommand Cmd { get; private set; }

        public UnityAction<ControllerType> onCtrlStart { get; set; }
        public UnityAction<ControllerType> onCtrlStop { get; set; }
        private CameraController cameraCtrl
        {
            get
            {
                return CameraController.Instence;
            }
        }

        public static bool log = false;
        //树型结构
        private List<OperateNode> startedActions = new List<OperateNode>();
        public Stack<ExecuteUnit> activedUnits = new Stack<ExecuteUnit>();
        public Stack<ExecuteUnit> redoUnits = new Stack<ExecuteUnit>();
        private ExecuteUnit currentUnit;
        public UnityAction onComplete { get; set; }

        public ActionStateMechine(ActionCommand cmd)
        {
            this.Cmd = cmd;
            root = ExecuteUtil.AnalysisGraph(cmd);
            InitStates();
        }

        #region States
        public void InitStates()
        {
            statemap.Add(State.Start, new StartState());
            statemap.Add(State.End, new EndState());
            statemap.Add(State.Logic, new LogicState());
            statemap.Add(State.Operate, new OperateState());

            foreach (var item in statemap)
            {
                item.Value.stateMechine = this;
            }
        }

        public void SetState(State state)
        {
            currentState = statemap[state];
        }

        public void ExecuteGroup(List<ExecuteUnit> units)
        {
            foreach (var unit in units)
            {
                Execute(unit);
            }
        }

        public void Execute(ExecuteUnit unit)
        {
            SwitchState(unit);
            currentState.Execute(unit);
        }

        public void Complete(ExecuteUnit unit)
        {
            SwitchState(unit);
            currentState.Complete(unit);
        }

        public void UnDo(ExecuteUnit unit)
        {
            SwitchState(unit);
            currentState.UnDo(unit);
        }

        public void SwitchState(ExecuteUnit unit)
        {
            if (unit.node is Graph.StartNode)
            {
                SetState(State.Start);
            }
            else if (unit.node is Graph.EndNode)
            {
                SetState(State.End);
            }
            else if (unit.node is Graph.LogicNode)
            {
                SetState(State.Logic);
            }
            else if (unit.node is Graph.OperateNode)
            {
                SetState(State.Operate);
            }
        }
        #endregion

        /// <summary>
        /// 状态机执行完成
        /// </summary>
        public virtual void Complete()
        {
            if (onComplete != null)
                onComplete.Invoke();
        }

        public virtual void OnStartExecute(bool forceAuto)
        {
            this.IsAuto = forceAuto;
            Execute(root);
        }

        public virtual void OnEndExecute(bool all)
        {
            StopUpdateAction(false);
            if (all)
            {
                Complete(root);
            }
            else
            {
                if (redoUnits.Count > 0)
                {
                    currentUnit = redoUnits.Pop();
                    Complete(currentUnit);
                    Execute(currentUnit);
                }
                else
                {
                    if(startedActions.Count > 0)
                    {
                        startedActions[0].OnEndExecute(true);
                        OnStopAction(startedActions[0]);
                    }
                }
            }
        }
      

        public virtual void OnUnDoExecute(bool all)
        {
            StopUpdateAction(true);
            if (all)
            {
                UnDo(root);
            }
            else
            {
                if (activedUnits.Count > 0)
                {
                    var unit = activedUnits.Pop();
                    if (unit == currentUnit)
                    {
                        redoUnits.Push(unit);
                        UnDo(unit);
                        unit = activedUnits.Pop();
                    }

                    currentUnit = unit;
                    redoUnits.Push(currentUnit);
                    Debug.Log("UnDo:" + currentUnit.node);
                    UnDo(currentUnit);
                    Execute(currentUnit);
                }
            }
        }
        /// <summary>
        /// 结束开启的步骤
        /// </summary>
        public virtual void CompleteStarted()
        {
            foreach (var item in startedActions)
            {
                item.OnEndExecute(true);
            }
        }
        /// <summary>
        /// 添加新的触发器
        /// </summary>
        /// <param name="action"></param>
        public void OnStartAction(OperateNode action)
        {
            startedActions.Add(action);
            if (onCtrlStart != null)
                onCtrlStart.Invoke(action.CtrlType);
        }

        /// <summary>
        /// 移除触发器
        /// </summary>
        /// <param name="action"></param>
        public void OnStopAction(OperateNode action)
        {
            startedActions.Remove(action);
            if (onCtrlStop != null && startedActions.Find(x => x.CtrlType == action.CtrlType) == null)
            {
                onCtrlStop.Invoke(action.CtrlType);
            }
        }

        private string GetCameraID(OperateNode obj)
        {
            //忽略匹配相机
            if (Config.quickMoveElement)
            {
                return null;
            }
            //除要求使用特殊相机或是动画步骤,都用主摄像机
            else if (Config.useOperateCamera)
            {
                if (string.IsNullOrEmpty(obj.CameraID))
                {
                    return Cmd.CameraID;
                }
                else
                {
                    return obj.CameraID;
                }
            }
            //默认是主相机
            else
            {
                return CameraController.defultID;
            }
        }

        private void StopUpdateAction(bool force)
        {
            if (cameraCtrl != null)
            {
                cameraCtrl.StopStarted(force);
            }
        }
    }
}