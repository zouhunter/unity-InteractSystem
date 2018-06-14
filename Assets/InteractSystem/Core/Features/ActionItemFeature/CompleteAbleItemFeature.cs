using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem
{

    public abstract class CompleteAbleItemFeature : ActionItemFeature
    {
        private List<UnityAction<CompleteAbleItemFeature>> onCompleteActions = new List<UnityAction<CompleteAbleItemFeature>>();

        public void RegistOnCompleteSafety(UnityAction<CompleteAbleItemFeature> onClicked)
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
                    onClicked.Invoke(this);
                }
            }
        }

        public void RemoveOnComplete(UnityAction<CompleteAbleItemFeature> onClicked)
        {
            if (onCompleteActions.Contains(onClicked))
            {
                onCompleteActions.Remove(onClicked);
            }
        }

        public abstract void AutoExecute();
    }

}