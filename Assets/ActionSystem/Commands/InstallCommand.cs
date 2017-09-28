using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    [System.Serializable]
    public class InstallCommand : ActionCommand
    {
        public float distence = 10;
        protected override IActionCtroller CreateCtrl()
        {
            var installObjs = Array.ConvertAll<ActionObj, InstallObj>(ActionObjs, x => x as InstallObj);
            var coroutineCtrl = new InstallCtrl(this, distence, installObjs);
            return coroutineCtrl;
        }

    }

}