using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    [System.Serializable]
    public class InstallCommand : CoroutionCommand
    {
        public float distence;
        public bool hightLight;

        protected override ICoroutineCtrl CreateCtrl()
        {
            var installObjs = Array.ConvertAll<ActionObj, InstallObj>(trigger.ActionObjs, x => x as InstallObj);
            var coroutineCtrl = new InstallCtrl(trigger, distence, hightLight, installObjs);
            return coroutineCtrl;
        }
    }

}