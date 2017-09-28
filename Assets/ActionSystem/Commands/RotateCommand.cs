using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    [Serializable]
    public class RotateCommand : ActionCommand 
    {
        public float operateDistence;
        protected override IActionCtroller CreateCtrl()
        {
            var rotAnimCtrl = new WorldActionSystem.RotateAnimController(operateDistence,this);
            return rotAnimCtrl;
        }
    }

}
