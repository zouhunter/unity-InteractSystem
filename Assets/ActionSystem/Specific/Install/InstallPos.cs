using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace WorldActionSystem
{
    /// <summary>
    /// 模拟安装坐标功能
    /// </summary>
    public class InstallPos :MonoBehaviour, ActionObj
    {
        public string stapName;
        public bool autoInstall;

        public bool Installed { get { return obj != null; } }
        public InstallObj obj { get; private set; }

        public IInstallCtrl installCtrl { private get; set; }

        public IRemoteController RemoteCtrl
        {
            get
            {
                return ActionSystem.Instance.RemoteController;
            }
        }

        public void Attach(InstallObj obj)
        {
            this.obj = obj;
            obj.onInstallOkEvent = ()=>{
                if (installCtrl.CurrStapComplete())
                {
                    RemoteCtrl.EndExecuteCommand();
                }
            };
          
        }

        public InstallObj Detach()
        {
            InstallObj old = obj;
            obj = null;
            return old;
        }
    }

}