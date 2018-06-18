using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem
{

    public sealed class CompleteAbleItemFeature : ActionItemFeature
    {
        private List<UnityAction<CompleteAbleItemFeature>> onCompleteActions = new List<UnityAction<CompleteAbleItemFeature>>();

        public UnityAction<Graph.OperaterNode> onAutoExecute { get; set; }

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

        public void AutoExecute(Graph.OperaterNode operateNode)
        {
            if (onAutoExecute != null)
            {
                onAutoExecute.Invoke(operateNode);
            }
            else
            {
                Debug.Log("请编写自动步骤进行代码");
            }
        }
    }

}