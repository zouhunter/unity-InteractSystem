using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public abstract class OperateController : IOperateController
    {
        public abstract ControllerType CtrlType { get; }
        public UnityAction<string> UserError { get; set; }
        public ActionSystem system { get; set; }
        protected Config config { get { return system.Config; } }
        private CameraController cameraCtrl { get { return system.CameraCtrl; } }
        protected Camera viewCamera
        {
            get
            {
                return cameraCtrl == null ?
                    Camera.main : cameraCtrl.GetActiveCamera(config.useOperateCamera);
            }
        }
        public abstract void Update();
        protected bool CanOperate(ActionObj target)
        {
            return target.system == system;
        }
    }
}