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
    public class InstallCommand : ActionCommand
    {
        public InstallController installCtrl;
        public InstallCommand(string stapName, InstallController installCtrl) : base(stapName)
        {
            this.installCtrl = installCtrl;
        }
        public override void StartExecute(bool forceAuto)
        {
            installCtrl.SetStapActive(StepName);
            installCtrl.AutoInstallWhenNeed(StepName, forceAuto);
            base.StartExecute(forceAuto);
        }
        public override void EndExecute()
        {
            installCtrl.EndInstall(StepName);
            base.EndExecute();
        }
        public override void UnDoCommand()
        {
            installCtrl.QuickUnInstall(StepName);
            base.UnDoCommand();
        }
    }
}