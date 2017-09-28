using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{

    public class SequencesCommand : IActionCommand
    {
        private int index;
        public string StepName { get; private set; }
        private IList<IActionCommand> commandList;
        private bool forceAuto;
        private bool started;
        private bool completed;

        public SequencesCommand(string stepName, IList<IActionCommand> commandList)
        {
            StepName = stepName;
            this.commandList = commandList;
        }

        public bool StartExecute(bool forceAuto)
        {
            if(!started)
            {
                started = true;
                this.forceAuto = forceAuto;
                commandList[index].StartExecute(forceAuto);
                return true;
            }
            else
            {
                Debug.Log("already started" + StepName);
                return false;
            }
        }


        public bool EndExecute()
        {
            if(!completed)
            {
                completed = true;
                foreach (var item in commandList)
                {
                    item.EndExecute();
                }
                OnEndExecute();
                return true;
            }
           else
            {
                Debug.Log("already complete" + StepName);
                return false;
            }
        }
      
        public void UnDoExecute()
        {
            started = false;
            completed = false;
            index = 0;
            foreach (var item in commandList)
            {
                item.UnDoExecute();
            }
        }

        internal bool ContinueExecute()
        {
            index++;
            if (index < commandList.Count)
            {
                Debug.Log("Execute:" + index + "->continue StartExecute");
                commandList[index].StartExecute(forceAuto);
                return true;
            }
            Debug.Log("Execute:" + index + "->EndExecute");
            return false;
        }

        public void OnEndExecute()
        {
            index = commandList.Count - 1;
        }
    }

}