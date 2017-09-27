using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class ActionCtroller : IActionCtroller
    {
        protected ActionCommand trigger { get; set; }
        protected List<int> queueID = new List<int>();
        protected ActionObj[] actionObjs { get; set; }

        public ActionCtroller( ActionCommand trigger)
        {
            InitCommand(trigger);
        }
        public void InitCommand(ActionCommand trigger)
        {
            this.trigger = trigger;
            actionObjs = trigger.ActionObjs;
        }

        public virtual void StartExecute(bool forceAuto)
        {
            queueID.Clear();
            foreach (ActionObj item in actionObjs)
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
            Debug.Log("complete :" + id);
            var notComplete = Array.FindAll<ActionObj>(actionObjs, x => (x as ActionObj).QueueID == id && !x.Complete);
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
                var neetActive = Array.FindAll<ActionObj>(actionObjs, x => (x as ActionObj).QueueID == id);
                if (neetActive.Length > 0)
                {
                    foreach (ActionObj item in neetActive)
                    {
                        item.OnStartExecute();
                        item.onEndExecuteCurrent = OnCommandObjComplete;
                    }
                }

                return true;
            }
            return false;
        }
    }
}