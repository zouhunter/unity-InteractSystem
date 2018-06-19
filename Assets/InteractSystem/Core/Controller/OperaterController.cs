using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem
{

    public abstract class OperateController : IOperateController
    {
        public static bool log = false;
        public abstract ControllerType CtrlType { get; }
        public UnityAction<string> userErr { get; set; }
        public bool Active { get; set; }
        private CameraController cameraCtrl
        {
            get
            {
                return CameraController.Instence;
            }
        }
        protected Camera viewCamera
        {
            get
            {
                return cameraCtrl.currentCamera;
            }
        }

        public abstract void Update();

        protected virtual void SetUserErr(string errInfo)
        {
            if (userErr != null)
            {
                this.userErr(errInfo);
            }
        }
    }
}