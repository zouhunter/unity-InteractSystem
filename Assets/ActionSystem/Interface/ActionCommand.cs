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
        public event CommandExecute executeAction;
        public event CommandExecute undoAction;
        public event CommandExecute endExecuteAction;

        public string StapName { get; set; }
        public ActionCommand(string stapName)
        {
            this.StapName = stapName;
        }
        public virtual void StartExecute(bool forceAuto)
        {
            if (executeAction != null)
            {
                executeAction(StapName);
            }
        }
        public virtual void EndExecute()
        {
            if (endExecuteAction != null)
            {
                endExecuteAction(StapName);
            }
        }

        public virtual void UnDoCommand()
        {
            if (undoAction != null)
            {
                undoAction(StapName);
            }
        }
    }


}