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
        public override IList<IActionCommand> CreateCommands()
        {
            var cmds = new List<IActionCommand>();
            for (int i = 0; i < repeat; i++)
            {
                cmds.Add(new ConnectCommand(StepName,objs, OnTriggerComplete, GetCtrl));
            }
            return cmds;
        }

        private ConnectCtrl GetCtrl()
        {
            var ctrl = new ConnectCtrl(this, objs,lineMaterial,lineWight,pointDistence);
            return ctrl;
        }

        private void OnTriggerComplete()
        {
            OnComplete();
        }
    }
}
