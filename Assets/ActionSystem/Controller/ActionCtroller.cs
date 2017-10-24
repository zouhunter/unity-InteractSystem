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
            var viewCamera = CameraController.GetViewCamera(trigger.CameraID); 
           
            Debug.Assert(viewCamera);
            if ((commandType & ControllerType.Click) == ControllerType.Click)
            {
                var clickCtrl = new ClickContrller(viewCamera);
                clickCtrl.UserError = trigger.UserError;
                commandList.Add(clickCtrl);
            }
            if ((commandType & ControllerType.Connect) == ControllerType.Connect)
            {
                var lineRender = trigger.GetComponent<LineRenderer>();
                if (lineRender == null) lineRender = trigger.gameObject.AddComponent<LineRenderer>();
                var objs = Array.ConvertAll<IActionObj, ConnectObj>(Array.FindAll<IActionObj>(trigger.ActionObjs, x => x is ConnectObj), x => x as ConnectObj);
                var connectCtrl = new ConnectCtrl(viewCamera, lineRender, objs, trigger.lineMaterial, trigger.lineWight, trigger.hitDistence, trigger.pointDistence);
                connectCtrl.onError = trigger.UserError;
                commandList.Add(connectCtrl);
            }
            if ((commandType & ControllerType.Match) == ControllerType.Match)
            {
                var matchObjs = Array.ConvertAll<IActionObj, MatchObj>(Array.FindAll<IActionObj>(trigger.ActionObjs, x => x is MatchObj), x => x as MatchObj);
                var matchCtrl = new MatchCtrl(viewCamera, trigger.hitDistence,trigger.elementDistence, matchObjs);
                matchCtrl.UserError = trigger.UserError;
                commandList.Add(matchCtrl);
            }
            if ((commandType & ControllerType.Install) == ControllerType.Install)
            {
                var installObjs = Array.ConvertAll<IActionObj, InstallObj>(Array.FindAll<IActionObj>(trigger.ActionObjs, x => x is InstallObj), x => x as InstallObj);
                var installCtrl = new InstallCtrl(viewCamera, trigger.hitDistence, trigger.elementDistence, installObjs);
                installCtrl.UserError = trigger.UserError;
                commandList.Add(installCtrl);
            }
            if ((commandType & ControllerType.Rotate) == ControllerType.Rotate)
            {
                var rotAnimCtrl = new RotateAnimController(viewCamera,trigger.hitDistence);
                rotAnimCtrl.UserError = trigger.UserError;
                commandList.Add(rotAnimCtrl);
            }
        }

        public virtual void OnStartExecute(bool forceAuto)
        {
            this.isForceAuto = forceAuto;
            ExecuteAStep(isForceAuto);
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
                if (!ExecuteAStep(isForceAuto))
                {
                    trigger.Complete();
                }
            }
        }

        protected bool ExecuteAStep(bool auto)
        {
            if (queueID.Count > 0)
            {
                var id = queueID[0];
                queueID.RemoveAt(0);
                var neetActive = Array.FindAll<IActionObj>(actionObjs, x => x.QueueID == id);
                if (neetActive.Length > 0)
                {
                    foreach (var item in neetActive)
                    {
                        item.onEndExecute = OnCommandObjComplete;
                        item.OnStartExecute(isForceAuto);
                    }
                }

                return true;
            }
            return false;
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