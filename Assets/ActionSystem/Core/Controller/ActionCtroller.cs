using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace WorldActionSystem
{
    public class ActionCtroller
    {
        public ActionObjCtroller activeObjCtrl { get; private set; }
        private List<IOperateController> controllerList = new List<IOperateController>();
        protected Coroutine coroutine;
        private Dictionary<ControllerType, int> activeTypes = new Dictionary<ControllerType, int>();
        public static bool log = false;
        public UnityAction<Graph.OperateNode> onActionStart;

        private CameraController cameraCtrl
        {
            get
            {
                return ActionSystem.Instence.cameraCtrl;
            }
        }
        private PickUpController pickupCtrl { get; set; }

        public ActionCtroller(MonoBehaviour holder,PickUpController pickupCtrl)
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
                    if(activeTypes.ContainsKey(ctrl.CtrlType) && activeTypes[ctrl.CtrlType] > 0)
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
            if (activeObjCtrl != null)
                activeObjCtrl.trigger.UserError(error);
        }
        /// 激活首要对象
        /// </summary>
        /// <param name="obj"></param>
        internal void OnPickUpObj(PickUpAbleItem obj)
        {
            if (activeObjCtrl != null) activeObjCtrl.OnPickUpObj(obj);
        }

        public virtual void OnStartExecute(ActionObjCtroller activeObjCtrl, bool forceAuto)
        {
            this.activeObjCtrl = activeObjCtrl;
            this.activeObjCtrl.onCtrlStart = OnActionStart;
            this.activeObjCtrl.onCtrlStop = OnActionStop;
            this.activeObjCtrl.OnStartExecute(forceAuto);
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
            if(activeTypes.ContainsKey(ctrlType))
            {
                activeTypes[ctrlType]++;
                if (activeTypes[ctrlType] < 0)
                {
                    activeTypes[ctrlType] = 0;
                }
            }
           
        }

        public virtual void OnEndExecute(ActionObjCtroller activeObjCtrl)
        {
            if (activeObjCtrl != null)
                activeObjCtrl.OnEndExecute();
        }

        public virtual void OnUnDoExecute(ActionObjCtroller activeObjCtrl)
        {
            if (activeObjCtrl != null)
                activeObjCtrl.OnUnDoExecute();
        }
    }
}