using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    [Serializable]
    public class ConnectCommand : ActionCommand 
    {
        public float lineWight = 0.1f;
        public Material lineMaterial;
        public float hitDistence = 10;
        public float pointDistence=0.1f;

        protected override IActionCtroller CreateCtrl()
        {
            var objs = Array.ConvertAll<ActionObj, ConnectObj>(ActionObjs, x => x as ConnectObj);
          
            var ctrl = new ConnectCtrl(this, objs, lineMaterial, lineWight, hitDistence, pointDistence);
            return ctrl;
        }
      
    }
}
