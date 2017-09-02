using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class InstallCommand : IActionCommand
    {
        public string StepName { get; private set; }
        private Func<InstallCtrl> createFunc;
        private InstallCtrl _installCtrl;
        private InstallCtrl installCtrl
        {
            get
            {
                if (_installCtrl == null)
                {
                    _installCtrl = createFunc.Invoke();
                }
                return _installCtrl;
            }
        }

        public InstallCommand(string stepName,Func<InstallCtrl> createFunc)
        {
            this.StepName = stepName;
            Debug.Log(stepName);
            this.createFunc = createFunc;
        }
        public void StartExecute(bool forceAuto)
        {
            installCtrl.SetStapActive();
            installCtrl.AutoInstallWhenNeed(forceAuto);
        }
        public void EndExecute()
        {
            installCtrl.EndInstall();
        }
        public void UnDoExecute()
        {
            installCtrl.QuickUnInstall();
        }
    }

}