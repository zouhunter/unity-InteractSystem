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
        private DragAnimController intallController;
        private bool registed;
        // Use this for initialization
        void Awake()
        {
           var startParent = GetComponentInChildren<InstallStart>();
            var endParent = GetComponentInChildren<InstallTarget>();
            var animParent = GetComponentsInChildren<AnimObj>(true);

            intallController = new DragAnimController(startParent, endParent, animParent);
            intallController.InstallErr += OnInstallErr;

            OnAllInstallPosInit(animParent);
        }

        private void Update()
        {
            intallController.Reflesh();
        }

        public override void SetHighLight(bool on)
        {
            intallController.SwitchHighLight(on);
        }

        private void OnInstallErr(string stepName, string err)
        {
            if (onUserErr != null) onUserErr(stepName, err);
        }

        private void OnAllInstallPosInit(AnimObj[] objs)
        {
            ActionCommand cmd;
            foreach (var item in objs)
            {
                cmd = new DragAnimCommand(item.stapName, intallController);
                if (OnRegistCommand != null) OnRegistCommand(cmd);
            }
            registed = true;
        }

    }

}