using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{

    public class ActionCtroller
    {
        protected ActionCommand trigger { get; set; }
        protected List<int> queueID = new List<int>();
        protected IActionObj[] actionObjs { get; set; }
        protected bool isForceAuto;
        private ControllerType commandType = 0;
        private ControllerType activeTypes = 0;
        private Queue<IActionObj> actionQueue = new Queue<IActionObj>();
        private List<IActionObj> startedActions = new List<IActionObj>();
        private List<IOperateController> controllerList = new List<IOperateController>();
        protected Coroutine coroutine;
        public static bool log = false;
        public UnityAction<IActionObj> onActionStart;
        private ActionSystem _system;
        private ActionSystem system { get { trigger.transform.SurchSystem(ref _system); return _system; } }
        protected Config config { get { return system.Config; } }
        protected CameraController cameraCtrl { get { return system.CameraCtrl; } }
        public ActionCtroller(ActionCommand trigger)
        {
            this.trigger = trigger;
            actionObjs = trigger.ActionObjs;
            ChargeQueueIDs();
            if (!config.ignoreController)
            {
                InitController();
            }
        }

        private void InitController()
        {
            foreach (var item in actionObjs)
            {
                if ((commandType & item.CtrlType) != item.CtrlType)
                {
                    commandType |= item.CtrlType;
                    RegisterControllerByType(item.CtrlType);
                }
            }
        }

        private void RegisterControllerByType(ControllerType type)
        {
            OperateController currentCtrl = null;
            switch (type)
            {
                case ControllerType.Install:
                    var installCtrl = new InstallCtrl();
                    installCtrl.onSelect = OnPickUpObj;
                    currentCtrl = installCtrl;
                    break;
                case ControllerType.Match:
                    var matchCtrl = new MatchCtrl();
                    matchCtrl.onSelect = OnPickUpObj;
                    currentCtrl = matchCtrl;
                    break;
                case ControllerType.Click:
                    currentCtrl = new ClickContrller();
                    break;
                case ControllerType.Rotate:
                    currentCtrl = new RotateAnimController();
                    break;
                case ControllerType.Connect:
                    var lineRender = trigger.GetComponent<LineRenderer>();
                    if (lineRender == null){
                        lineRender = trigger.gameObject.AddComponent<LineRenderer>();
                    }
                    currentCtrl = new ConnectCtrl(lineRender, trigger.lineMaterial, trigger.lineWight);
                    break;
                case ControllerType.Rope:
                    var ropCtrl = new RopeController();
                    ropCtrl.onSelect = OnPickUpObj;
                    currentCtrl = ropCtrl;
                    break;
                default:
                    break;
            }

            if(currentCtrl != null)
            {
                currentCtrl.system = system;
                currentCtrl.UserError = trigger.UserError;
                controllerList.Add(currentCtrl);
            }
        }
        /// <summary>
        /// 激活首要对象
        /// </summary>
        /// <param name="obj"></param>
        public void OnPickUpObj(IPlaceItem obj)
        {
            var prio = startedActions.Find(x => x.Name == obj.Name);
            if (prio != null)
            {
                startedActions.Remove(prio);
                startedActions.Insert(0, prio);
            }
        }
        public virtual void OnStartExecute(bool forceAuto)
        {
            this.isForceAuto = forceAuto;
            if (coroutine == null && trigger.gameObject.activeInHierarchy)
            {
                coroutine = trigger.StartCoroutine(Update());
            }
            ExecuteAStep();
        }
        private void ChargeQueueIDs()
        {
            actionQueue.Clear();
            queueID.Clear();
            foreach (var item in actionObjs)
            {
                if (!queueID.Contains(item.QueueID))
                {
                    queueID.Add(item.QueueID);
                }
            }
            queueID.Sort();
        }
        public virtual void OnEndExecute()
        {
            StopUpdateAction(false);
            CompleteQueues();
            Array.Sort(actionObjs);
            foreach (var item in actionObjs)
            {
                if (!item.Started)
                {
                    item.OnStartExecute(isForceAuto);
                }
                if (!item.Complete)
                {
                    item.OnEndExecute(true);
                }
            }

        }

        public virtual void OnUnDoExecute()
        {
            StopUpdateAction(true);
            UnDoQueues();
            ChargeQueueIDs();
            Array.Sort(actionObjs);
            Array.Reverse(actionObjs);
            foreach (var item in actionObjs)
            {
                if (item.Started)
                {
                    item.OnUnDoExecute();
                }
            }
        }

        public virtual IEnumerator Update()
        {
            while (true)
            {
                foreach (var ctrl in controllerList)
                {
                    if ((ctrl.CtrlType & activeTypes) == ctrl.CtrlType)
                    {
                        ctrl.Update();
                    }
                }
                yield return null;
            }
        }

        private void OnCommandObjComplete(IActionObj obj)
        {
            OnStopAction(obj);
            var notComplete = Array.FindAll<IActionObj>(actionObjs, x => x.QueueID == obj.QueueID && !x.Complete);
            if (notComplete.Length == 0)
            {
                if (!ExecuteAStep())
                {
                    if (!trigger.Completed)
                        trigger.Complete();
                }
            }
            else if (actionQueue.Count > 0)//正在循环执行
            {
                QueueExectueActions();
            }
        }

        public void CompleteOneStarted()
        {
            if (startedActions.Count > 0)
            {
                var action = startedActions[0];
                OnStopAction(action);
                action.OnEndExecute(true);
            }
            else
            {
                if (log) Debug.Log("startedActions.Count == 0");
            }
        }
        private void CompleteQueues()
        {
            while (actionQueue.Count > 0)
            {
                var action = actionQueue.Dequeue();
                if (!action.Complete)
                {
                    action.OnEndExecute(true);
                }
            }
        }
        private void UnDoQueues()
        {
            while (actionQueue.Count > 0)
            {
                var action = actionQueue.Dequeue();
                if (action.Started)
                {
                    action.OnUnDoExecute();
                }
            }
        }
        protected bool ExecuteAStep()
        {
            if (queueID.Count > 0)
            {
                var id = queueID[0];
                queueID.RemoveAt(0);
                var neetActive = Array.FindAll<IActionObj>(actionObjs, x => x.QueueID == id && !x.Started);
                if (isForceAuto)
                {
                    actionQueue.Clear();
                    foreach (var item in neetActive)
                    {
                        if (item.QueueInAuto)
                        {
                            actionQueue.Enqueue(item as ActionObj);
                        }
                        else
                        {
                            TryStartAction(item);
                        }
                    }
                    QueueExectueActions();
                }
                else
                {
                    foreach (var item in neetActive)
                    {
                        var obj = item;
                        TryStartAction(obj);
                    }
                }
                return true;
            }
            return false;
        }

        protected void QueueExectueActions()
        {
            if (actionQueue.Count > 0)
            {
                var actionObj = actionQueue.Dequeue();
                if (log) Debug.Log("QueueExectueActions" + actionObj);
                TryStartAction(actionObj);
            }
        }
        private void TryStartAction(IActionObj obj)
        {
            if (log) Debug.Log("Start A Step:" + obj);
            if (!obj.Started)
            {
                if (onActionStart != null) onActionStart.Invoke(obj);

                if (cameraCtrl != null)
                {
                    cameraCtrl.SetViewCamera(() =>
                   {
                       StartAction(obj);
                   }, GetCameraID(obj));
                }
                else
                {
                    StartAction(obj);
                }
            }
            else
            {
                Debug.LogError(obj + " allready started");
            }

        }

        private void StartAction(IActionObj obj)
        {
            if (!obj.Started)
            {
                obj.onEndExecute = () => OnCommandObjComplete(obj);
                obj.OnStartExecute(isForceAuto);
                OnStartAction(obj);
            }
        }

        /// <summary>
        /// 添加新的触发器
        /// </summary>
        /// <param name="action"></param>
        private void OnStartAction(IActionObj action)
        {
            startedActions.Add(action);
            activeTypes |= action.CtrlType;
        }

        /// <summary>
        /// 移除触发器
        /// </summary>
        /// <param name="action"></param>
        private void OnStopAction(IActionObj action)
        {
            startedActions.Remove(action);
            activeTypes = 0;
            foreach (var item in startedActions)
            {
                activeTypes |= item.CtrlType;
            }
        }


        private string GetCameraID(IActionObj obj)
        {
            //忽略匹配相机
            if (config.quickMoveElement && obj is MatchObj && !(obj as MatchObj).ignorePass)
            {
                return null;
            }
            else if (config.quickMoveElement && obj is InstallObj && !(obj as InstallObj).ignorePass)
            {
                return null;
            }
            //除要求使用特殊相机或是动画步骤,都用主摄像机
            else if (config.useOperateCamera || obj is AnimObj)
            {
                if (string.IsNullOrEmpty(obj.CameraID))
                {
                    return trigger.CameraID;
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
            if (coroutine != null)
            {
                trigger.StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}