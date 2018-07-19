using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using NodeGraph;

namespace InteractSystem.Hooks
{
    public class CameraHook : ActionHook
    {
        [SerializeField]
        private string cameraNodeID;
        private CameraController cameraCtrl { get { return CameraController.Instence; } }

        protected override void OnBeforeEndExecute()
        {
            base.OnBeforeEndExecute();
            cameraCtrl.SetViewCameraQuick(cameraNodeID);
        }

        public override void OnStartExecute(bool auto)
        {
            base.OnStartExecute(auto);
            cameraCtrl.SetViewCameraAsync(() => {
                OnEndExecute(false);
            }, cameraNodeID);
        }
    }
}