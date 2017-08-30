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
        public override void StartExecute()
        {
            dragAnimCtrl.SetStapActive(StapName);
            dragAnimCtrl.AutoInstallWhenNeed(StapName);
            base.StartExecute();
        }
        public override void UnDoCommand()
        {
            dragAnimCtrl.QuickUnInstall(StapName);
            dragAnimCtrl.UnDoAnim(StapName);
            base.UnDoCommand();
        }
        public override void EndExecute()
        {
            dragAnimCtrl.EndPlayAnim(StapName);
            base.EndExecute();
        }
    }

}