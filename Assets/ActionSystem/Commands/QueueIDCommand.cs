using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class QueueIDCommand:CoroutionCommand
    {
        protected QueueIDObj[] actionObjs;
        public int Count { get; private set; }
        public override void InitCommand(string stepName, ActionCommand trigger)
        {
            base.InitCommand(stepName, trigger);
            this.actionObjs = Array.ConvertAll<ActionObj, QueueIDObj>(trigger.ActionObjs, x => x as QueueIDObj) ;// actionObjs;
        }
        protected override ICoroutineCtrl CreateCtrl()
        {
            return new QueueIDCtrl(trigger);
        }
    }
}