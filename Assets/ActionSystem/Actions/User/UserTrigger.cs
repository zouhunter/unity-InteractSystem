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
        public override IList<IActionCommand> CreateCommands()
        {
            var cmds = new List<IActionCommand>();
            for (int i = 0; i < repeat; i++)
            {
                cmds.Add(new QueueIDCommand(StepName, repeat, Array.ConvertAll<ActionObj, QueueIDObj>(actionObjs, x => x as QueueIDObj), OnStepComplete));
            }
            return cmds;
        }
        private void OnStepComplete()
        {
            OnComplete();
        }
    }

}