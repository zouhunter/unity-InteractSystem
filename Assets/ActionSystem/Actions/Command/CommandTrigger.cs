using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class CommandTrigger : ActionTrigger, IActionCommand
    {
        private List<int> queueID = new List<int>();
        public override IActionCommand CreateCommand()
        {
            return this;
        }

        public void EndExecute()
        {
            foreach (CommandObj item in actionObjs)
            {
                item.EndExecute();
            }
        }

        public void StartExecute(bool forceAuto)
        {
            Debug.Log("StartExecute");
            InitObjectQueue();
        }

        public void UnDoExecute()
        {
            foreach (CommandObj item in actionObjs)
            {
                item.UnDoExecute();
            }
        }

        void InitObjectQueue()
        {
            queueID.Clear();
            foreach (CommandObj item in actionObjs)
            {
                if (!queueID.Contains(item.queueID)){
                    queueID.Add(item.queueID);
                }
            }
            queueID.Sort();
            ExecuteAStep();
        }

        bool ExecuteAStep()
        {
            if (queueID.Count > 0)
            {
                var id = queueID[0];
                queueID.RemoveAt(0);
                var neetActive = Array.FindAll<ActionObj>(actionObjs, x => (x as CommandObj).queueID == id);
                if (neetActive.Length > 0)
                {
                    foreach (CommandObj item in neetActive)
                    {
                        item.StartExecute();
                        item.onEndExecute = OnCommandObjComplete;
                    }
                }
              
                return true;
            }
            return false;
        }

        void OnCommandObjComplete(int id)
        {
            var notComplete = Array.FindAll<ActionObj>(actionObjs, x => (x as CommandObj).queueID == id && !x.Complete);
            if (notComplete.Length == 0)
            {
                if (!ExecuteAStep())
                {
                    if (onStepComplete != null) onStepComplete.Invoke(StepName);
                }
            }
        }
    }

}