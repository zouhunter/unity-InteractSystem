using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class AnimationCommand : ActionCommand
    {
        protected override IActionCtroller CreateCtrl()
        {
            return new AnimCtroller(this);
        }
    }

}