using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem.Graph;

namespace WorldActionSystem.Structure
{

    public class ActionObjCtroller: ActionStateMechine
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

        public static bool log = false;
        //树型结构
        private ExecuteUnit root;
        private List<OperateNode> startedActions = new List<OperateNode>();
        public List<Structure.ExecuteUnit> activeUnits = new List<ExecuteUnit>();
        public Dictionary<ExecuteUnit, UnitStatus> statuDic = new Dictionary<ExecuteUnit, UnitStatus>();

        public ActionObjCtroller(ActionCommand cmd)
        {
            this.Cmd = cmd;
            root = ExecuteUtil.AnalysisGraph(cmd.GraphObj);
        }

        public virtual void OnStartExecute(bool forceAuto)
        {
            this.isForceAuto = forceAuto;
            Execute(root);
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