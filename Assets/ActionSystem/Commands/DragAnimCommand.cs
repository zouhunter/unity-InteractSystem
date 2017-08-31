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
        private DragAnimController dragAnimCtrl;

        public DragAnimCommand(string stapName, DragAnimController dragAnimCtrl) : base(stapName)
        {
            this.dragAnimCtrl = dragAnimCtrl;
        }
        public override void StartExecute(bool forceAuto)
        {
            dragAnimCtrl.SetStapActive(StepName);
            dragAnimCtrl.AutoInstallWhenNeed(StepName, forceAuto);
            base.StartExecute(forceAuto);
        }
        public override void UnDoCommand()
        {
            dragAnimCtrl.QuickUnInstall(StepName);
            dragAnimCtrl.UnDoAnim(StepName);
            base.UnDoCommand();
        }
        public override void EndExecute()
        {
            dragAnimCtrl.EndPlayAnim(StepName);
            base.EndExecute();
        }
    }

}