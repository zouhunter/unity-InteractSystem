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
        private DragStart startParent;
        private DragTarget endParent;
        private DragShow animParent;
        private DragAnimController intallController;
        private bool registed;
        // Use this for initialization
        void Awake()
        {
            startParent = GetComponentInChildren<DragStart>();
            endParent = GetComponentInChildren<DragTarget>();
            animParent = GetComponentInChildren<DragShow>();
            endParent.GetInstallDicAsync(OnAllInstallPosInit);

            intallController = new DragAnimController(startParent, endParent, animParent);
            intallController.InstallErr += OnInstallErr;
        }

        private void Update()
        {
            intallController.Reflesh();
        }

        public override void SetHighLight(bool on)
        {
            intallController.SwitchHighLight(on);
        }

        public override void InsertScript<T>(bool on)
        {
            startParent.InsertScript<T>(on);
        }

        private void OnInstallErr(string stepName, string err)
        {
            if (onUserErr != null) onUserErr(stepName, err);
        }

        private void OnAllInstallPosInit(Dictionary<string, List<DragPos>> dic)
        {
            ActionCommand cmd;
            foreach (var item in dic)
            {
                cmd = new DragAnimCommand(item.Key, intallController, item.Value);
                if (registFunc != null) registFunc(cmd);
            }
            registed = true;
        }

    }

}