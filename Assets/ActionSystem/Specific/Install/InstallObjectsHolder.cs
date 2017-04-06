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
    /// 注册所有安装命令
    /// </summary>
    public class InstallObjectsHolder : ActionHolder
    {
        private IInstallStart startParent;
        private IInstallEnd endParent;
        private IInstallCtrl intallController;

        // Use this for initialization
        void Awake()
        {
            startParent = GetComponentInChildren<IInstallStart>();
            endParent = GetComponentInChildren<IInstallEnd>();
            endParent.GetInstallDicAsync(OnAllInstallPosInit);

            intallController = new InstallController(startParent, endParent);
            intallController.InstallErr += OnInstallErr;
        }

        private void Update()
        {
            intallController.Reflesh();
        }

        public override void SetHighLight(bool on)
        {
            intallController.SwitchHighLight(on);
        }

        public override void InsertScript<T>(bool on)
        {
            startParent.InsertScript<T>(on);
        }

        private void OnInstallErr(string err){
            onUserErr("安装错误", err);
        }
        
        private void OnAllInstallPosInit(Dictionary<string, List<InstallPos>> dic)
        {
            ActionCommand cmd;
            foreach (var item in dic)
            {
                cmd = new InstallCommand(item.Key, intallController, item.Value);
                if(registFunc != null) registFunc(cmd);
            }
        }

    }

}