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
        public override bool Registed
        {
            get
            {
                return registed;
            }
        }
        private InstallController intallController;
        private bool registed;

        // Use this for initialization
        void Awake()
        {
            var startParent = GetComponentInChildren<InstallStart>();
            var endParent = GetComponentInChildren<InstallTarget>();
            endParent.GetInstallDicAsync(OnAllInstallPosInit);

            intallController = new InstallController(startParent, endParent, OnInstallComplete);
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
        private void OnInstallComplete(string stepName)
        {
            if (OnStepEnd != null) OnStepEnd.Invoke(stepName);
        }
        //startParent.InsertScript<T>(on);

        private void OnInstallErr(string stepName, string err)
        {
            if (onUserErr != null) onUserErr(stepName, err);
        }

        private void OnAllInstallPosInit(Dictionary<string, List<InstallPos>> dic)
        {
            ActionCommand cmd;
            foreach (var item in dic)
            {
                cmd = new InstallCommand(item.Key, intallController, item.Value);
                if (OnRegistCommand != null) OnRegistCommand(cmd);
            }
            registed = true;
        }

    }

}