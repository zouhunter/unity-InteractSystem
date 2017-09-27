using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    [Serializable]
    public class ClickCommand : ActionCommand
    {
        [SerializeField]
        private Camera viewCamera;
        [SerializeField]
        private bool highLight;

        protected override IActionCtroller CreateCtrl()
        {
            if (viewCamera == null) viewCamera = Camera.main;
            
            return new WorldActionSystem.ClickContrller(this, viewCamera, highLight);
        }
    }

}