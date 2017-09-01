using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class DragAnimObjectHolder : ActionHolder
    {
        public override bool Registed
        {
            get
            {
                return registed;
            }
        }
        private DragAnimController dragAnimCtrl;
        private bool registed;
        // Use this for initialization
        void Awake()
        {
           var startParent = GetComponentInChildren<InstallElements>();
            var endParent = GetComponentInChildren<InstallTarget>();
            var animParent = GetComponentInChildren<AnimGroup>();

            dragAnimCtrl = new DragAnimController(startParent, endParent, animParent);
            dragAnimCtrl.InstallErr += OnInstallErr;
            animParent.onAllElementInit = OnAllInstallObjInit;
        }

        private void Update()
        {
            dragAnimCtrl.Reflesh();
        }

        public override void SetHighLight(bool on)
        {
            dragAnimCtrl.SwitchHighLight(on);
        }

        private void OnInstallErr(string stepName, string err)
        {
            if (onUserErr != null) onUserErr(stepName, err);
        }

        private void OnAllInstallObjInit(Dictionary<string, List<AnimObj>> dic)
        {
            foreach (var list in dic)
            {
                var cmd = new DragAnimCommand(list.Key, dragAnimCtrl);
                if (OnRegistCommand != null) OnRegistCommand(cmd);
                foreach (var obj in list.Value)
                {
                    obj.RegistEndPlayEvent( OnEndPlay);
                }
            }
            registed = true;
        }
        private void OnEndPlay(string StepName)
        {
            if (dragAnimCtrl.CurrStapComplete())
            {
                if (OnStepEnd != null)
                    OnStepEnd.Invoke(StepName);
            }
        }

       
    }

}