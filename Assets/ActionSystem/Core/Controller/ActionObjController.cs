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
        private Dictionary<object, List<ExecuteUnit>> waitUnits = new Dictionary<object, List<ExecuteUnit>>();
        private Stack<ExecuteUnit> startedUnitsStake = new Stack<ExecuteUnit>();
        private Stack<ExecuteUnit> backupUnitsStake = new Stack<ExecuteUnit>();
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
                for (int i = 0; i < unit.childUnits.Count - 1; i++)
                {
                    var key = unit.childUnits[i];
                    var stack = unit.childUnits[i + 1];
                    waitUnits[key] = stack;
                }
                ExecuteList(unit.childUnits[0]);
            }
            else if (unit.node is OperateNode)
            {
                var operateNode = unit.node as OperateNode;

                operateNode.onEndExecute = () =>
                {
                    var key = unit.parentUnits[0].GetPositon(unit);
                    if (waitUnits.ContainsKey(key))
                    {
                        ExecuteList(waitUnits[key]);
                    }
                };

                for (int i = 0; i < unit.childUnits.Count - 1; i++)
                {
                    var key = unit.childUnits[i];
                    var value = unit.childUnits[i + 1];
                    waitUnits[key] = value;
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

        private void ExecuteList(List<ExecuteUnit> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Execute(list[i]);
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