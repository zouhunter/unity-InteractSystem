using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public static class TriggerStatistics
    {
        public static List<ActionTrigger> RetriveTriggsr(Transform transform)
        {
            List<ActionTrigger> triggerList = new List<WorldActionSystem.ActionTrigger>() ;
            RetriveActionTrigger(transform, (trigger) => { triggerList.Add(trigger); });
            return triggerList;
        }

        private static void RetriveActionTrigger(Transform activeTransform, UnityAction<ActionTrigger> onRetrive)
        {
            var trigger = activeTransform.GetComponent<ActionTrigger>();
            if (trigger != null)
            {
                onRetrive.Invoke(trigger);
            }
            else
            {
                foreach (Transform item in activeTransform)
                {

                    if (item.gameObject.activeSelf)
                    {
                        RetriveActionTrigger(item, onRetrive);
                    }
                }
            }

        }
    }
}