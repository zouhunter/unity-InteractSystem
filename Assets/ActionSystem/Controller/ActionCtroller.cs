using System;
using UnityEngine;
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
        private ControllerType commandType { get { return trigger.commandType; } }
        private List<IActionCtroller> commandList = new List<IActionCtroller>();
        private Queue<ActionObj> actionQueue = new Queue<ActionObj>();
        private List<ActionHook> hookList = new List<ActionHook>();
        protected Coroutine coroutine;
        public ActionCtroller(ActionCommand trigger)
        {
            this.trigger = trigger;
            actionObjs = trigger.ActionObjs;
            ChargeQueueIDs();
            InitController();
        }
        private void InitController()
        {
            if ((commandType & ControllerType.Click) == ControllerType.Click)
            {
                var clickCtrl = new ClickContrller();
                clickCtrl.UserError = trigger.UserError;
                commandList.Add(clickCtrl);
            }
            if ((commandType & ControllerType.Connect) == ControllerType.Connect)
            {
                var lineRender = trigger.GetComponent<LineRenderer>();
                if (lineRender == null) lineRender = trigger.gameObject.AddComponent<LineRenderer>();
                var objs = Array.ConvertAll<IActionObj, ConnectObj>(Array.FindAll<IActionObj>(trigger.ActionObjs, x => x is ConnectObj), x => x as ConnectObj);
                var connectCtrl = new ConnectCtrl(lineRender, objs, trigger.lineMaterial, trigger.lineWight, trigger.hitDistence, trigger.pointDistence);
                connectCtrl.onError = trigger.UserError;
                commandList.Add(connectCtrl);
            }
            if ((commandType & ControllerType.Match) == ControllerType.Match)
            {
                var matchObjs = Array.ConvertAll<IActionObj, MatchObj>(Array.FindAll<IActionObj>(trigger.ActionObjs, x => x is MatchObj), x => x as MatchObj);
                var matchCtrl = new MatchCtrl(trigger.hitDistence, trigger.elementDistence, matchObjs);
                matchCtrl.UserError = trigger.UserError;
                commandList.Add(matchCtrl);
            }
            if ((commandType & ControllerType.Install) == ControllerType.Install)
            {
                var installObjs = Array.ConvertAll<IActionObj, InstallObj>(Array.FindAll<IActionObj>(trigger.ActionObjs, x => x is InstallObj), x => x as InstallObj);
                var installCtrl = new InstallCtrl(trigger.hitDistence, trigger.elementDistence, installObjs);
                installCtrl.UserError = trigger.UserError;
                commandList.Add(installCtrl);
            }
            if ((commandType & ControllerType.Rotate) == ControllerType.Rotate)
            {
                var rotAnimCtrl = new RotateAnimController(trigger.hitDistence);
                rotAnimCtrl.UserError = trigger.UserError;
                commandList.Add(rotAnimCtrl);
            }
        }

        public virtual void OnStartExecute(bool forceAuto)
        {
            this.isForceAuto = forceAuto;
            ExecuteAStep();
            if (coroutine == null)
            {
                coroutine = trigger.StartCoroutine(Update());
            }
            ForEachAction((ctrl) =>
            {
                ctrl.OnStartExecute(forceAuto);
            });
        }
        private void ChargeQueueIDs()
        {
            actionQueue.Clear();
            hookList.Clear();
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
            ForEachAction((ctrl) =>
            {
                ctrl.OnEndExecute();
            });

            foreach (var item in actionObjs)
            {
                if (!item.Complete)
                {
                    item.OnEndExecute();
                }
            }

            StopUpdateAction();
        }

        public virtual void OnUnDoExecute()
        {
            ChargeQueueIDs();
            foreach (var item in actionObjs)
            {
                if (item.Started)
                {
                    item.OnUnDoExecute();
                }
            }
            ForEachAction((ctrl) =>
            {
                ctrl.OnUnDoExecute();
            });
            StopUpdateAction();
        }

        public virtual IEnumerator Update()
        {
            while (true)
            {
                ForEachAction((ctrl) =>
                {
                    ctrl.Update();
                });
                yield return null;
            }
        }

        private void OnCommandObjComplete(int id)
        {
            var notComplete = Array.FindAll<IActionObj>(actionObjs, x => x.QueueID == id && !x.Complete);
            if (notComplete.Length == 0)
            {
                if (!ExecuteAStep())
                {
                    if (!trigger.Completed) trigger.Complete();
                }
            }
            else if(actionQueue.Count > 0)//正在循环执行
            {
                QueueExectueActions();
            }
        }

        protected bool ExecuteAStep()
        {
            if (queueID.Count > 0)
            {
                var id = queueID[0];
                queueID.RemoveAt(0);
                var neetActive = Array.FindAll<IActionObj>(actionObjs, x => x.QueueID == id && !x.Started);
                if (neetActive.Length == 0) return false;
                if (isForceAuto)
                {
                    hookList.Clear();
                    actionQueue.Clear();
                    foreach (var item in neetActive)
                    {
                        if(item is ActionObj)
                        {
                            actionQueue.Enqueue(item as ActionObj);
                        }
                        else /*if(item is ActionHook)*/
                        {
                            hookList.Add(item as ActionHook);
                        }
                    }
                    QueueExectueActions();
                }
                else
                {
                    CameraController.SetViewCamera(() =>
                    {
                        foreach (var item in neetActive)
                        {
                            var obj = item;
                            TryStartAction(obj);
                        }
                    }, GetCameraID(neetActive[neetActive.Length - 1]));
                    
                }
                return true;
            }
            return false;
        }

        protected void QueueExectueActions()
        {
            if(actionQueue.Count > 0)
            {
                var actionObj = actionQueue.Dequeue();
                CameraController.SetViewCamera(() =>
                {
                    TryStartAction(actionObj);
                },GetCameraID(actionObj));
            }
            ///最后执行hook
            if (actionQueue.Count == 0)
            {
                foreach (var item in hookList)
                {
                    TryStartAction(item);
                }
            }
        }
        private void TryStartAction(IActionObj obj)
        {
            if (!obj.Started)
            {
                obj.onEndExecute = OnCommandObjComplete;
                obj.OnStartExecute(isForceAuto);
            }
        }

        private string GetCameraID(IActionObj obj)
        {
            //不需要改变相机状态的钩子
            if (obj is ActionHook)
            {
                return null;
            }
            //忽略匹配相机
            else if (Setting.ignoreMatch && obj is MatchObj)
            {
                return null;
            }
            //除要求使用特殊相机或是动画步骤,都用主摄像机
            else if (Setting.useOperateCamera || commandType == 0)
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

        private void ForEachAction(UnityEngine.Events.UnityAction<IActionCtroller> OnRetive)
        {
            foreach (var item in commandList)
            {
                OnRetive(item);
            }
        }

        private void StopUpdateAction()
        {

            if (coroutine != null)
            {
                trigger.StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}