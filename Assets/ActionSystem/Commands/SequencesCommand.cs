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
        private List<IActionCommand> commandList;
        private bool forceAuto;
        public int Count { get; private set; }

        public SequencesCommand(string stepName,int count, List<IActionCommand> commandList)
        {
            StepName = stepName;
            this.commandList = commandList;
            this.Count = count;
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
            Debug.Log(index);
            if (index < commandList.Count)
            {
                StartExecute(forceAuto);
                return true;
            }
            return false;
        }
    }

}