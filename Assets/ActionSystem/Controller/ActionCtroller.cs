using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class ActionCtroller
    {
        public ActionObjCtroller activeObjCtrl { get; private set; }
        private List<IOperateController> controllerList = new List<IOperateController>();
        protected Coroutine coroutine;
        private ControllerType activeTypes = 0;
        public static bool log = false;
        public UnityAction<IActionObj> onActionStart;

        private CameraController cameraCtrl
        {
            get
            {
                return ActionSystem.Instence.cameraCtrl;
            }
        }
        private PickUpController pickupCtrl { get; set; }
        private MonoBehaviour holder;

        public ActionCtroller(MonoBehaviour holder,PickUpController pickupCtrl)
        {
            this.holder = holder;
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
                    if ((ctrl.CtrlType & activeTypes) != 0)
                    {
                        ctrl.Update();
                    }
                }
            }

        }
        private void RegisterControllers()
        {
            pickupCtrl.onPickup += (OnPickUpObj);
            Debug.Log("RegisterControllers");
            controllerList.Add(new LinkCtrl());
            controllerList.Add(new PlaceCtrl());
            controllerList.Add(new ClickCtrl());
            controllerList.Add(new RopeCtrl());
            controllerList.Add(new RotateCtrl());
            controllerList.Add(new ConnectCtrl());
            controllerList.Add(new DragCtrl());

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
            activeTypes |= ctrlType;
        }

        private void OnActionStop(ControllerType ctrlType)
        {
            activeTypes ^= ctrlType;
        }

        public virtual void OnEndExecute(ActionObjCtroller activeObjCtrl)
        {
            if (activeObjCtrl != null) activeObjCtrl.OnEndExecute();
        }

        public virtual void OnUnDoExecute(ActionObjCtroller activeObjCtrl)
        {
            if (activeObjCtrl != null) activeObjCtrl.OnUnDoExecute();
        }
    }
}