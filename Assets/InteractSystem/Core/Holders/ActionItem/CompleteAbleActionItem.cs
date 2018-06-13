using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem
{
    public abstract class CompleteAbleActionItem : ActionItem
    {
        private List<UnityAction<CompleteAbleActionItem>> onCompleteActions = new List<UnityAction<CompleteAbleActionItem>>();

        public void RegistOnCompleteSafety(UnityAction<CompleteAbleActionItem> onClicked)
        {
            if (!onCompleteActions.Contains(onClicked))
            {
                onCompleteActions.Add(onClicked);
            }
        }
      
        public void OnComplete()
        {
            if (onCompleteActions.Count > 0)
            {
                var actions = onCompleteActions.ToArray();
                foreach (var onClicked in actions)
                {
                    onClicked.Invoke(this as CompleteAbleActionItem);
                }
            }
        }

        public void RemoveOnComplete(UnityAction<CompleteAbleActionItem> onClicked)
        {
            if (onCompleteActions.Contains(onClicked))
            {
                onCompleteActions.Remove(onClicked);
            }
        }
    }
}
