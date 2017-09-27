using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    [Serializable]
    public class MatchCommand : CoroutionCommand
    {
        public float distence;
        public bool highLight;
        protected override ICoroutineCtrl CreateCtrl()
        {
            var matchCtrl = new MatchCtrl(trigger,distence, highLight);
            return matchCtrl;
        }
    }

}