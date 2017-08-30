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
        public IList<InstallPos> installers;
        public InstallController installCtrl;
        public InstallCommand(string stapName, InstallController installCtrl, IList<InstallPos> installers) : base(stapName)
        {
            this.installers = installers;
            this.installCtrl = installCtrl;
        }
        public override void StartExecute()
        {
            installCtrl.SetStapActive(StapName);
            installCtrl.AutoInstallWhenNeed(StapName);
            base.StartExecute();
        }
        public override void EndExecute()
        {
            installCtrl.EndInstall(StapName);
            base.EndExecute();
        }
        public override void UnDoCommand()
        {
            installCtrl.QuickUnInstall(StapName);
            base.UnDoCommand();
        }
    }
}