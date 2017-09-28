using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    
    public abstract class ActionCtroller : IActionCtroller
    {
        protected ActionCommand trigger { get; set; }
        protected List<int> queueID = new List<int>();
        protected ActionObj[] actionObjs { get; set; }
        protected bool isForceAuto;
        public ActionCtroller( ActionCommand trigger)
        {
            this.trigger = trigger;
            actionObjs = trigger.ActionObjs;
            ChargeQueueIDs();
        }

        public virtual void OnStartExecute(bool forceAuto)
        {
            this.isForceAuto = forceAuto;
            if (!forceAuto) {
                ExecuteAStep();
            }
           
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
                item.OnEndExecute();
            }
        }

        public virtual void OnUnDoExecute()
        {
            ChargeQueueIDs();
            foreach (var item in actionObjs)
            {
                item.OnUnDoExecute();
            }
        }

        public abstract IEnumerator Update();

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
                        item.onEndExecute = OnCommandObjComplete;
                    }
                }

                return true;
            }
            return false;
        }
    }
}