using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class QueueIDCtrl : ICoroutineCtrl
    {
        private ActionCommand trigger { get; set; }
        protected List<int> queueID = new List<int>();
        private QueueIDObj[] actionObjs { get; set; }

        public QueueIDCtrl( ActionCommand trigger)
        {
            InitCommand(trigger);
        }
        public void InitCommand(ActionCommand trigger)
        {
            this.trigger = trigger;
            actionObjs = Array.ConvertAll<ActionObj, QueueIDObj>(trigger.ActionObjs, x => x as QueueIDObj);
        }

        public virtual void StartExecute(bool forceAuto)
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
        public virtual void EndExecute()
        {
            foreach (var item in actionObjs)
            {
                item.OnEndExecute();
            }
        }

        public virtual void UnDoExecute()
        {
            foreach (var item in actionObjs)
            {
                item.OnUnDoExecute();
            }
        }

        public virtual IEnumerator Update()
        {
            yield return null;
        }

        private void OnCommandObjComplete(int id)
        {
            var notComplete = Array.FindAll<ActionObj>(actionObjs, x => (x as QueueIDObj).QueueID == id && !x.Complete);
            if (notComplete.Length == 0)
            {
                if (!ExecuteAStep())
                {
                    trigger.Complete();
                }
            }
        }
        private bool ExecuteAStep()
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
                        item.OnStartExecute();
                        item.onEndExecute = OnCommandObjComplete;
                    }
                }

                return true;
            }
            return false;
        }
    }
}