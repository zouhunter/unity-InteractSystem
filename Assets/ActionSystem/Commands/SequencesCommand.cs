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

        public SequencesCommand(string stepName, IList<IActionCommand> commandList)
        {
            StepName = stepName;
            this.commandList = commandList;
        }

        public void StartExecute(bool forceAuto)
        {
            this.forceAuto = forceAuto;
            commandList[index].StartExecute(forceAuto);
        }


        public void EndExecute()
        {
            index = commandList.Count - 1;
            foreach (var item in commandList)
            {
                item.EndExecute();
            }
        }

      
        public void UnDoExecute()
        {
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
                StartExecute(forceAuto);
                return true;
            }
            Debug.Log("Execute:" + index + "->EndExecute");
            return false;
        }
    }

}