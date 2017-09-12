using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class ConnectCommand : QueueIDCommand
    {
        private ConnectCtrl ctrl;
        private Func<ConnectCtrl> onCreateCtrl;
        public ConnectCommand(string stepName,ConnectObj[] objs,UnityAction onStepComplete, Func<ConnectCtrl> onCreateCtrl):base(stepName, objs, onStepComplete)
        {
            this.onCreateCtrl = onCreateCtrl;
        }

        public override void StartExecute(bool forceAuto)
        {
            base.StartExecute(forceAuto);
            if (ctrl == null) ctrl = onCreateCtrl();
            ctrl.StartConnecter();
        }

        public override void EndExecute()
        {
            base.EndExecute();
            if (ctrl != null) 
            ctrl.StopConnecter();
        }

        public override void UnDoExecute()
        {
            base.UnDoExecute();
            if (ctrl != null)
                ctrl.UnDoConnectItems();
        }
    }
}
