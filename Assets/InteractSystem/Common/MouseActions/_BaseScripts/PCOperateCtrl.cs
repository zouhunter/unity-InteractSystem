using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem.Actions
{

    public abstract class PCOperateCtrl<T, A> : OperateCtrl<T, A> where T : PCOperateCtrl<T, A>, new() where A : ActionItem
    {
        protected static T _instence;
        public static T Instence
        {
            get
            {
                if (_instence == null)
                {
                    _instence = new T();
                }
                return _instence;
            }
        }
        protected CameraController cameraCtrl
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

        public bool needUpdate { get { return Instence is IUpdateAble; } }
        protected CoroutineController coroutineCtrl { get { return CoroutineController.Instence; } }
        public override void RegistItem(A item)
        {
            base.RegistItem(item);
            if (needUpdate)
            {
                coroutineCtrl.RegistFrameAction((Instence as IUpdateAble).Update);
            }
        }
        public override void RemoveItem(A item)
        {
            base.RemoveItem(item);
            if (itemList.Count == 0 && needUpdate)
            {
                coroutineCtrl.RemoveFrameAction((Instence as IUpdateAble).Update);
            }
        }
    }
}