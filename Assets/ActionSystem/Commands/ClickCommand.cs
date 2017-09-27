using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    [Serializable]
    public class ClickCommand : CoroutionCommand
    {
        [SerializeField]
        private Camera viewCamera;
        [SerializeField]
        private bool highLight;

        protected override ICoroutineCtrl CreateCtrl()
        {
            ClickObj[] clickObjs = Array.ConvertAll<ActionObj, ClickObj>(trigger.ActionObjs, x => x as ClickObj);
            return new WorldActionSystem.ClickContrller(trigger, viewCamera ?? Camera.main, highLight, clickObjs);
        }
    }

}