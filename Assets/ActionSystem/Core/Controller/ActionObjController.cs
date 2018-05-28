using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem.Graph;

namespace WorldActionSystem
{

    public class ActionObjCtroller
    {
        protected ActionGroup Context { get { return Cmd.Context; } }
        public ActionCommand Cmd { get; private set; }
        protected bool isForceAuto { get; private set; }

        public UnityAction<ControllerType> onCtrlStart { get; set; }
        public UnityAction<ControllerType> onCtrlStop { get; set; }
        private CameraController cameraCtrl
        {
            get
            {
                return ActionSystem.Instence.cameraCtrl;
            }
        }

        //树型结构
        private ExecuteGroup executeGroup;
        private ExecuteUnit startUnit { get { return executeGroup.executeUnit; } }
        private List<ExecuteUnit> activeUnits;
        private List<OperateNode> startedActions = new List<OperateNode>();
        private Dictionary<ExecuteUnit, Stack<List<ExecuteUnit>>> waitUnits = new Dictionary<ExecuteUnit, Stack<List<ExecuteUnit>>>();

        public static bool log = false;

        public ActionObjCtroller(ActionCommand cmd)
        {
            this.Cmd = cmd;
            executeGroup = new ExecuteGroup(cmd.GraphObj);
        }

        public virtual void OnStartExecute(bool forceAuto)
        {
            this.isForceAuto = forceAuto;
            Execute(startUnit);
        }

        /// <summary>
        /// 执行一个单元
        /// </summary>
        /// <param name="unit"></param>
        private void Execute(ExecuteUnit unit)
        {
            if (unit.node is StartNode)
            {
                Debug.Assert(unit.childUnits.Count > 0);
                //打开下一级的步骤
                for (int i = unit.childUnits.Count - 1; i >= 0; i--){
                    if(waitUnits.ContainsKey(unit)){
                        waitUnits[unit].Push(unit.childUnits[i]);
                    }
                }
                ExecuteAnGroup(unit);
            }
            else if (unit.node is OperateNode)
            {
                var operateNode = unit.node as OperateNode;
                //operateNode.onEndExecute = ExecuteAnGroup;
                //打开下一级的步骤
                for (int i = unit.childUnits.Count - 1; i >= 0; i--){
                    //waitUnits.Push(unit.childUnits[i]);
                }
                operateNode.OnStartExecute(isForceAuto);
            }
            else if (unit.node is EndNode)
            {
                //判断是否结束
            }
            else if (unit.node is LogicNode)
            {
                Debug.Log("执行到逻辑节点");
                var logicNode = unit.node as LogicNode;
                switch (logicNode.logicType)
                {
                    case LogicType.And:
                        break;
                    case LogicType.Or:
                        break;
                    case LogicType.ExclusiveOr:
                        break;
                    default:
                        break;
                }
            }
            
        }

        internal void ExecuteAnGroup(ExecuteUnit unit)
        {
            if(waitUnits.ContainsKey(unit))
            {
                var stack = waitUnits[unit];
                if (stack.Count > 0)
                {
                    activeUnits = stack.Pop();
                    foreach (var childUnit in activeUnits)
                    {
                        Execute(childUnit);
                    }
                }
            }
            
        }

        /// <summary>
        /// 设置优先执行
        /// </summary>
        /// <param name="obj"></param>
        internal void OnPickUpObj(PickUpAbleItem obj)
        {
            var actionItems = obj.GetComponentsInChildren<ActionItem>();
            if (actionItems != null && actionItems.Length > 0)
            {
                //foreach (var item in actionItems)
                //{
                //    var prio = startedActions.Find(x => x.Name == item.Name);
                //    if (prio != null)
                //    {
                //        startedActions.Remove(prio);
                //        startedActions.Insert(0, prio);
                //    }
                //}
            }
        }


        public virtual void OnEndExecute()
        {
            StopUpdateAction(false);
        }

        public virtual void OnUnDoExecute()
        {
            StopUpdateAction(true);
        }

        /// <summary>
        /// 添加新的触发器
        /// </summary>
        /// <param name="action"></param>
        private void OnStartAction(OperateNode action)
        {
            startedActions.Add(action);
            if (onCtrlStart != null)
                onCtrlStart.Invoke(action.CtrlType);
        }

        /// <summary>
        /// 移除触发器
        /// </summary>
        /// <param name="action"></param>
        private void OnStopAction(OperateNode action)
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