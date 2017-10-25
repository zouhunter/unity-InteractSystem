using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
#if !NoFunction
using DG.Tweening;
#endif
namespace WorldActionSystem
{
    /// <summary>
    /// 模拟安装坐标功能
    /// </summary>
    public class InstallObj : ActionObj
    {
        public bool autoInstall;
        public bool Installed { get { return obj != null; } }
        public PickUpAbleElement obj { get; private set; }

        void Awake()
        {
            gameObject.layer = Setting.installPosLayer;
            ElementController.onInstall += OnInstallComplete;
            ElementController.onUnInstall += OnUnInstallComplete;
        }

        private void OnDestroy()
        {
            ElementController.onInstall -= OnInstallComplete;
            ElementController.onUnInstall -= OnUnInstallComplete;
        }

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            if (auto || autoInstall)//查找安装点并安装后结束
            {
                PickUpAbleElement obj = ElementController.GetUnInstalledObj(name);
                Attach(obj);
                obj.NormalInstall(gameObject);
            }
        }
        public override void OnEndExecute()
        {
            base.OnEndExecute();
            if (!Installed)
            {
                PickUpAbleElement obj = ElementController.GetUnInstalledObj(name);
                Attach(obj);
                obj.QuickInstall(gameObject);
            }
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (Installed)
            {
                var obj = Detach();
                obj.QuickUnInstall();
            }
        }
        private void OnInstallComplete(PickUpAbleElement obj)
        {
            if (obj == this.obj) {
                TryEndExecute();
            }
        }

        private void OnUnInstallComplete(PickUpAbleElement obj)
        {
            if(Installed && this.obj == obj)
            {
                this.obj = null;
                OnUnDoExecute();
                OnStartExecute(auto);
            }
        }
        public bool Attach(PickUpAbleElement obj)
        {
            if (this.obj != null)
            {
                return false;
            }
            else
            {
                this.obj = obj;
                return true;
            }
        }

        public PickUpAbleElement Detach()
        {
            PickUpAbleElement old = obj;
            obj = null;
            return old;
        }
    }

}