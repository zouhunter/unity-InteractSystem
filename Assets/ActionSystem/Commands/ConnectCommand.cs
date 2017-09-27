using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    [Serializable]
    public class ConnectCommand : QueueIDCommand
    {
        public float lineWight = 0.1f;
        public Material lineMaterial;
        public float pointDistence;
        public Camera _viewCamera;
        protected override ICoroutineCtrl CreateCtrl()
        {
            var objs = Array.ConvertAll<ActionObj, ConnectObj>(trigger.ActionObjs, x => x as ConnectObj);
            var lineRender = trigger.GetComponent<LineRenderer>();
            var ctrl = new ConnectCtrl(trigger, lineRender, objs, lineMaterial, lineWight, pointDistence, _viewCamera ?? Camera.main);
            return ctrl;
        }
      
    }
}
