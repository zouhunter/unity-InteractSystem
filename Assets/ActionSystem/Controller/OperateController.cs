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
        public UnityAction<string> userError { get; set; }
        public UnityAction<IPlaceItem> onSelect { get; set; }
        private CameraController cameraCtrl
        {
            get
            {
                return ActionSystem.Instence.cameraCtrl;
            }
        }
        protected Camera viewCamera
        {
            get
            {
                return cameraCtrl == null ?
                    Camera.main : cameraCtrl.GetActiveCamera(Config.useOperateCamera);
            }
        }
        public abstract void Update();
        protected virtual void OnUserError(string errInfo)
        {
            if (userError != null) userError.Invoke(errInfo);
        }
        protected virtual void OnSelectItem(IPlaceItem item)
        {
            if (onSelect != null) onSelect.Invoke(item);
        }
    }
}