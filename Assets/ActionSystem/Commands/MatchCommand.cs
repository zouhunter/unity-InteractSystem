﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    [Serializable]
    public class MatchCommand : ActionCommand
    {
        public float distence;
        protected override IActionCtroller CreateCtrl()
        {
            var matchCtrl = new MatchCtrl(this,distence);
            return matchCtrl;
        }
    }

}