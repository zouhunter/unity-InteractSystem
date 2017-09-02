using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class UserTrigger : ActionTrigger
    {
        public override IActionCommand CreateCommand()
        {
            return new QueueIDCommand(StepName,Array.ConvertAll<ActionObj,QueueIDObj>(actionObjs,x=>x as QueueIDObj), OnStepComplete);
        }
        private void OnStepComplete(string stepName)
        {
            if (onStepComplete != null) onStepComplete(StepName);
        }
    }

}