using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace InteractSystem
{
    public class ActionCtroller
    {
        public ActionCommand activeCommand { get; private set; }
        private List<IOperateController> controllerList = new List<IOperateController>();
        protected Coroutine coroutine;
        private Dictionary<ControllerType, int> activeTypes = new Dictionary<ControllerType, int>();
        public static bool log = false;
        public UnityAction<Graph.OperateNode> onActionStart;
        private CameraController cameraCtrl
        {
            get
            {
                return CameraController.Instence;
            }
        }
        private PickUpController pickupCtrl { get; set; }
        private static ActionCtroller _instence;
        public static ActionCtroller Instence
        {
            get
            {
                if (_instence == null)
                {
                    _instence = new ActionCtroller(ActionSystem.Instence, PickUpController.Instence);
                }
                return _instence;
            }
        }

        public ActionCtroller(MonoBehaviour holder, PickUpController pickupCtrl)
        {
            //this.holder = holder;
            this.pickupCtrl = pickupCtrl;
            RegisterControllers();
            coroutine = holder.StartCoroutine(Update());
        }

        private IEnumerator Update()
        {
            var wait = new WaitForFixedUpdate();
            while (true)
            {
                yield return wait;//要保证在PickUpCtrl之前执行才不会有问题，否则拿起来就被放下了！！！
                foreach (var ctrl in controllerList)
                {
                    if (activeTypes.ContainsKey(ctrl.CtrlType) && activeTypes[ctrl.CtrlType] > 0)
                    {
                        ctrl.Update();
                    }
                }
            }

        }
        private void RegisterControllers()
        {
            pickupCtrl.onPickup += (OnPickUpObj);
            //Debug.Log("RegisterControllers");
            var types = this.GetType().Assembly.GetTypes();
            foreach (var t in types)
            {
                //是否是類
                if (t.IsClass)
                {
                    //是否是當前類的派生類
                    if (t.IsSubclassOf(typeof(OperateController)))
                    {
                        var ctrl = System.Activator.CreateInstance(t) as IOperateController;
                        controllerList.Add(ctrl);
                    }
                }
            }

            foreach (var ctrl in controllerList)
            {
                ctrl.userErr = OnUserError;
            }
        }

        public void OnUserError(string error)
        {
            if (activeCommand != null)
                activeCommand.UserError(error);
        }
        /// 激活首要对象
        /// </summary>
        /// <param name="obj"></param>
        internal void OnPickUpObj(PickUpAbleItem obj)
        {
            var actionItems = obj.GetComponentsInChildren<ActionItem>();
            if (actionItems != null && actionItems.Length > 0)
            {
                ElementController.Instence.SetPriority(actionItems);
            }
        }
        public virtual void SetContext(ActionCommand activeCommand)
        {
            this.activeCommand = activeCommand;
            this.activeCommand.objectCtrl.onCtrlStart = OnActionStart;
            this.activeCommand.objectCtrl.onCtrlStop = OnActionStop;
        }
        public virtual void OnStartExecute(bool forceAuto)
        {
            if (activeCommand != null)
            {
                activeCommand.objectCtrl.OnStartExecute(forceAuto);
            }
        }

        private void OnActionStart(ControllerType ctrlType)
        {
            if (!activeTypes.ContainsKey(ctrlType))
            {
                activeTypes.Add(ctrlType, 0);
            }
            activeTypes[ctrlType]++;
        }

        private void OnActionStop(ControllerType ctrlType)
        {
            if (activeTypes.ContainsKey(ctrlType))
            {
                activeTypes[ctrlType]++;
                if (activeTypes[ctrlType] < 0)
                {
                    activeTypes[ctrlType] = 0;
                }
            }

        }

        public virtual void OnEndExecute()
        {
            if (activeCommand != null)
            {
                activeCommand.objectCtrl.OnEndExecute(true);
            }
        }

        public virtual void OnUnDoExecute()
        {
            if (activeCommand != null)
            {
                activeCommand.objectCtrl.OnUnDoExecute(true);
            }
        }

        public virtual void OnEndExecuteStarted()
        {
            if (activeCommand != null)
            {
                activeCommand.objectCtrl.OnEndExecute(false);
            }
        }

        public virtual void OnUnDoExecuteOne()
        {
            if (activeCommand != null)
            {
                activeCommand.objectCtrl.OnUnDoExecute(false);
            }
        }
    }
}