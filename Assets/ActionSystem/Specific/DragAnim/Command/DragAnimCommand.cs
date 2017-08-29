using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{

    public class DragAnimCommand : ActionCommand
    {
        private DragAnimController intallController;
        private List<DragPos> value;

        public DragAnimCommand(string stapName, DragAnimController intallController, List<DragPos> value) : base(stapName)
        {
            this.intallController = intallController;
            this.value = value;
        }
        public override void StartExecute()
        {
            intallController.SetStapActive(StapName);
            intallController.AutoInstallWhenNeed(StapName);
            base.StartExecute();
        }
        public override void UnDoCommand()
        {
            intallController.QuickUnInstall(StapName);
            intallController.UnDoAnim(StapName);
            base.UnDoCommand();
        }
        public override void EndExecute()
        {
            intallController.EndPlayAnim(StapName);
            base.EndExecute();
        }
    }

}