using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class InstallTrigger : ActionTrigger
    {
        public float distence;
        public bool highLight;
        public List<InstallObj> InstallObjs { get { return _installObjs; } }
        private List<InstallObj> _installObjs = new List<InstallObj>();
        private InstallCtrl installCtrl;
        protected override void Awake()
        {
            base.Awake();
            _installObjs.AddRange(Array.ConvertAll<ActionObj, InstallObj>(actionObjs, x => (InstallObj)x));
        }
        public override IList<IActionCommand> CreateCommands()
        {
            var cmds = new List<IActionCommand>();
            for (int i = 0; i < repeat; i++)
            {
                cmds.Add(new InstallCommand(StepName, CreateInstallCtrl));
            }
            return cmds;
        }
        private InstallCtrl CreateInstallCtrl()
        {
            installCtrl = new InstallCtrl(this,distence,highLight,InstallObjs);
            installCtrl.onComplete = OnStepComplete;
            installCtrl.onInstallError = base.OnUserError;
            installCtrl.elementGroup = ElementGroup();
            return installCtrl;
        }

        private void OnStepComplete()
        {
            installCtrl = null;
            OnComplete();
        }
    }

}
