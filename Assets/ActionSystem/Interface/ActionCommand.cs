using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
namespace WorldActionSystem
{

    public abstract class ActionCommand
    {
        public event CommandExecute onExecuteAction;
        public event CommandExecute onUndoAction;
        public event CommandExecute onEndExecuteAction;

        public string StepName { get; set; }
        public ActionCommand(string stapName)
        {
            this.StepName = stapName;
        }
        public virtual void StartExecute(bool forceAuto)
        {
            if (onExecuteAction != null)
            {
                onExecuteAction(StepName);
            }
        }
        public virtual void EndExecute()
        {
            if (onEndExecuteAction != null)
            {
                onEndExecuteAction(StepName);
            }
        }

        public virtual void UnDoCommand()
        {
            if (onUndoAction != null)
            {
                onUndoAction(StepName);
            }
        }
    }


}