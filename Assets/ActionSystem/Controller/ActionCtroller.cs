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
        #region 可选参数配制
        public float lineWight = 0.1f;
        public Material lineMaterial;
        #endregion
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
            var types = Enum.GetValues(typeof(ControllerType));
            foreach (var type in types)
            {
                RegisterController((ControllerType)type);
            }
            holder.StartCoroutine(Start());
        }

        private IEnumerator Start()
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
        private void RegisterController(ControllerType type)
        {
            OperateController currentCtrl = null;
            switch (type)
            {
                case ControllerType.Install:
                    currentCtrl = new InstallCtrl();
                    break;
                case ControllerType.Match:
                    currentCtrl = new MatchCtrl();
                    break;
                case ControllerType.Click:
                    currentCtrl = new ClickContrller();
                    break;
                case ControllerType.Rotate:
                    currentCtrl = new RotateAnimController();
                    break;
                case ControllerType.Connect:
                    var lineRender = holder.GetComponentInChildren<LineRenderer>();
                    if (lineRender == null){
                        lineRender = holder.gameObject.AddComponent<LineRenderer>();
                    }
                    currentCtrl = new ConnectCtrl(lineRender, lineMaterial, lineWight);
                    break;
                case ControllerType.Rope:
                    currentCtrl = new RopeController();
                    break;
                default:
                    break;
            }

            if (currentCtrl != null)
            {
                controllerList.Add(currentCtrl);
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