using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{

    public class DragAnimCommand : IActionCommand
    {
        private DragAnimController dragAnimCtrl;
        public string StepName { get; private set; }

        public DragAnimCommand(string stepName, DragAnimController dragAnimCtrl)
        {
            this.dragAnimCtrl = dragAnimCtrl;
            this.StepName = stepName;
        }
        public  void StartExecute(bool forceAuto)
        {
            dragAnimCtrl.SetStapActive(StepName);
            dragAnimCtrl.AutoInstallWhenNeed(StepName, forceAuto);
        }
        public  void UnDoExecute()
        {
            dragAnimCtrl.QuickUnInstall(StepName);
            dragAnimCtrl.UnDoAnim(StepName);
        }
        public  void EndExecute()
        {
            dragAnimCtrl.EndPlayAnim(StepName);
        }
    }

}