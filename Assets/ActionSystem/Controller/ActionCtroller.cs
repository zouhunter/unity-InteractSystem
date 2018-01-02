using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class ActionCtroller
    {
        protected List<ActionObjCtroller> activeObjCtrls = new List<ActionObjCtroller>();
        private List<IOperateController> controllerList = new List<IOperateController>();
        protected Coroutine coroutine;
        private ControllerType activeTypes = 0;
        public static bool log = false;
        public UnityAction<IActionObj> onActionStart;
        protected MonoBehaviour holder;
        private CameraController cameraCtrl
        {
            get
            {
                return ActionSystem.Instence.cameraCtrl;
            }
        }

        public ActionCtroller(MonoBehaviour holder)
        {
            this.holder = holder;
            RegisterControllers();
            holder.StartCoroutine(Start());
        }

        private IEnumerator Start()
        {
            while (true)
            {
                foreach (var ctrl in controllerList)
                {
                    if ((ctrl.CtrlType & activeTypes) != 0)
                    {
                        ctrl.Update();
                    }
                }
                yield return null;
            }
        }
        private void RegisterControllers()
        {
            controllerList.Add(new InstallCtrl());
            controllerList.Add(new MatchCtrl());
            controllerList.Add(new ClickCtrl());
            controllerList.Add(new RotateCtrl());
            controllerList.Add(new ConnectCtrl());
            controllerList.Add(new RopeCtrl());
            controllerList.Add(new DragCtrl());

            foreach (var currentCtrl in controllerList)
            {
                currentCtrl.userError = OnUserError;
                currentCtrl.onSelect = OnPickUpObj;
            }
        }

        public void OnUserError(string error)
        {
            foreach (var item in activeObjCtrls)
            {
                item.trigger.UserError(error);
            }
        }
        /// 激活首要对象
        /// </summary>
        /// <param name="obj"></param>
        public void OnPickUpObj(IPlaceItem obj)
        {
            foreach (var item in activeObjCtrls)
            {
                item.OnPickUpObj(obj);
            }
        }

        public virtual void OnStartExecute(ActionObjCtroller actionCtrl, bool forceAuto)
        {
            if (!activeObjCtrls.Contains(actionCtrl))
            {
                activeObjCtrls.Add(actionCtrl);
            }
            actionCtrl.onCtrlStart = OnActionStart;
            actionCtrl.OnStartExecute(forceAuto);
        }

        private void OnActionStart(ControllerType ctrlType)
        {
            activeTypes |= ctrlType;
        }

        private void OnActionStop(ControllerType ctrlType)
        {
            activeTypes ^= ctrlType;
        }

        public virtual void OnEndExecute(ActionObjCtroller actionCtrl)
        {
            actionCtrl.OnEndExecute();
            if (activeObjCtrls.Contains(actionCtrl))
            {
                activeObjCtrls.Remove(actionCtrl);
            }
        }

        public virtual void OnUnDoExecute(ActionObjCtroller actionCtrl)
        {
            actionCtrl.OnUnDoExecute();
        }
    }
}