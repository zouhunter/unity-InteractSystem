using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    /// <summary>
    /// 模拟安装坐标功能
    /// </summary>
    public class InstallPos : ActionObj
    {
        public string stapName;
        public bool autoInstall;

        public bool Installed { get { return obj != null; } }
        public InstallObj obj { get; private set; }

        private Renderer render;
        public IInstallCtrl installCtrl { private get; set; }
        void Start()
        {
            render = GetComponent<Renderer>();
        }

        public void Attach(InstallObj obj)
        {
            this.obj = obj;
            if (installCtrl.CurrStapComplete())
            {
                RemoteCtrl.EndExecuteCommand();
            }
        }

        public InstallObj Detach()
        {
            InstallObj old = obj;
            obj = null;
            return old;
        }

        void OnMouseEnter()
        {
            if (!Installed)
            {
                render.enabled = true;
            }
        }

        void OnMouseExit()
        {
            render.enabled = false;
        }
    }

}