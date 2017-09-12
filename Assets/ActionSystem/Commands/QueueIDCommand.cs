using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class QueueIDCommand:IActionCommand
    {
        public string StepName { get; private set; }
        protected QueueIDObj[] actionObjs;
        protected List<int> queueID = new List<int>();
        protected UnityAction onComplete;
        public int Count { get; private set; }
        public QueueIDCommand(string stepName, QueueIDObj[] actionObjs, UnityAction onStepComplete)
        {
            this.StepName = stepName;
            this.actionObjs = actionObjs;
            this.onComplete = onStepComplete;
        }

        public virtual void EndExecute()
        {
            foreach (QueueIDObj item in actionObjs)
            {
                item.EndExecute();
            }
        }

        public virtual void StartExecute(bool forceAuto)
        {
            Debug.Log("StartExecute");
            InitObjectQueue();
        }

        public virtual void UnDoExecute()
        {
            foreach (QueueIDObj item in actionObjs)
            {
                item.UnDoExecute();
            }
        }

        void InitObjectQueue()
        {
            queueID.Clear();
            foreach (QueueIDObj item in actionObjs)
            {
                if (!queueID.Contains(item.QueueID))
                {
                    queueID.Add(item.QueueID);
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
                var neetActive = Array.FindAll<ActionObj>(actionObjs, x => (x as QueueIDObj).QueueID == id);
                if (neetActive.Length > 0)
                {
                    foreach (QueueIDObj item in neetActive)
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
            var notComplete = Array.FindAll<ActionObj>(actionObjs, x => (x as QueueIDObj).QueueID == id && !x.Complete);
            if (notComplete.Length == 0)
            {
                if (!ExecuteAStep())
                {
                    if (onComplete != null) onComplete.Invoke();
                }
            }
        }
    }
}