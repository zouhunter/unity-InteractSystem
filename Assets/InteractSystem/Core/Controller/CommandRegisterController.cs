using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem
{
    public class CommandController
    {
        public ActionGroup group;
        public List<ActionCommand> CommandList { get; private set; }
        public CommandController(ActionGroup group)
        {
            this.group = group;
        }

        public void Lunch(List<ActionCommand> commandList)
        {
            foreach (var cmd in commandList)
            {
                cmd.SetContext(group.transform);
            }
        }
    }

}
