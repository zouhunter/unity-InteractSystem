﻿using System;
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
        private void Awake(){
            _installObjs.AddRange(GetComponentsInChildren<InstallObj>(true));
        }
        public override IActionCommand CreateCommand()
        {
            return new InstallCommand1(StepName, CreateInstallCtrl);
        }
        private InstallCtrl CreateInstallCtrl()
        {
            installCtrl = new InstallCtrl(this);
            installCtrl.onStepComplete = OnStepComplete;
            installCtrl.InstallErr = OnUserError;
            return installCtrl;
        }

        private void OnStepComplete(string stepName)
        {
            if (onStepComplete != null) onStepComplete.Invoke(stepName);
        }
        private void OnUserError(string x,string y)
        {
            if (onUserErr != null) onUserErr(x,y);
        }
    }

}
