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
        protected bool isForceAuto;
        public ActionCtroller(ActionCommand trigger)
        {
            this.trigger = trigger;
            actionObjs = trigger.ActionObjs;
            ChargeQueueIDs();
        }

        public virtual void OnStartExecute(bool forceAuto)
        {
            this.isForceAuto = forceAuto;
            ExecuteAStep(isForceAuto);
        }
        private void ChargeQueueIDs()
        {
            foreach (ActionObj item in actionObjs)
            {
                if (!queueID.Contains(item.QueueID))
                {
                    queueID.Add(item.QueueID);
                }
            }
            queueID.Sort();
        }
        public virtual void OnEndExecute()
        {
            foreach (var item in actionObjs)
            {
                if (!item.Complete)
                {
                    item.OnEndExecute();
                }
            }
        }

        public virtual void OnUnDoExecute()
        {
            ChargeQueueIDs();
            foreach (var item in actionObjs)
            {
                if (item.Started)
                {
                    item.OnUnDoExecute();
                }
            }
        }

        public virtual IEnumerator Update() { yield break; }

        private void OnCommandObjComplete(int id)
        {
            Debug.Log("complete :" + id);
            var notComplete = Array.FindAll<ActionObj>(actionObjs, x => (x as ActionObj).QueueID == id && !x.Complete);
            if (notComplete.Length == 0)
            {
                if (!ExecuteAStep(isForceAuto))
                {
                    trigger.Complete();
                }
            }
        }

        protected bool ExecuteAStep(bool auto)
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
                        item.onEndExecute = OnCommandObjComplete;
                        item.OnStartExecute(isForceAuto);
                    }
                }

                return true;
            }
            return false;
        }
    }
}