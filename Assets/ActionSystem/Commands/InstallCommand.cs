using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
namespace WorldActionSystem
{
    [Serializable]
    public class InstallCommand : IActionCommand
    {
        public string StepName { get; private set; }
        public InstallController installCtrl;
        public InstallCommand(string stepName, InstallController installCtrl) 
        {
            this.StepName = stepName;
            this.installCtrl = installCtrl;
        }
        public  void StartExecute(bool forceAuto)
        {
            installCtrl.SetStapActive(StepName);
            installCtrl.AutoInstallWhenNeed(StepName, forceAuto);
        }
        public  void EndExecute()
        {
            installCtrl.EndInstall(StepName);
        }
        public  void UnDoExecute()
        {
            installCtrl.QuickUnInstall(StepName);
        }
    }
}