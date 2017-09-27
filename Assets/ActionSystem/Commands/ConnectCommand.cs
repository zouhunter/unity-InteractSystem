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

        protected override IActionCtroller CreateCtrl()
        {
            var objs = Array.ConvertAll<ActionObj, ConnectObj>(ActionObjs, x => x as ConnectObj);
            var lineRender = GetComponent<LineRenderer>();
            var ctrl = new ConnectCtrl(this,lineRender, objs, lineMaterial, lineWight, pointDistence, _viewCamera ?? Camera.main);
            return ctrl;
        }
      
    }
}
