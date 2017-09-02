using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ConnectTrigger : ActionTrigger
    {
        public float lineWight = 0.1f;
        public Material lineMaterial;
        public float pointDistence;
        private ConnectObj[] objs;
        protected override void Awake()
        {
            base.Awake();
            objs = Array.ConvertAll<ActionObj, ConnectObj>(actionObjs, (x) => x as ConnectObj);
        }
        public override IActionCommand CreateCommand()
        {
            var cmd = new ConnectCommand(StepName, objs, OnTriggerComplete, GetCtrl);
            return cmd;
        }

        private ConnectCtrl GetCtrl()
        {
            var ctrl = new ConnectCtrl(this, objs,lineMaterial,lineWight,pointDistence);
            return ctrl;
        }

        private void OnTriggerComplete(string stepName)
        {
            if (onStepComplete != null) onStepComplete.Invoke(StepName);
        }
    }
}
