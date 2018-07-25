using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem.Actions;
using System;

namespace InteractSystem
{

    public sealed class CompleteAbleItemFeature : ActionItemFeature
    {
        private List<UnityAction<CompleteAbleItemFeature>> onCompleteActions = new List<UnityAction<CompleteAbleItemFeature>>();
        public UnityAction<Graph.OperaterNode> onAutoExecute { get;private set; }

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
                var action = onCompleteActions[0];
                onCompleteActions.RemoveAt(0);
                action.Invoke(this);
            }
            else
            {
                Debug.LogWarning("have no on Complete action!");
            }
        }

        public void RemoveOnComplete(UnityAction<CompleteAbleItemFeature> onClicked)
        {
            if (onCompleteActions.Contains(onClicked))
            {
                onCompleteActions.Remove(onClicked);
            }
        }

        internal void Init(ActionItem actionItem,UnityAction<Graph.OperaterNode> onAutoExecute)
        {
            target = actionItem;
            this.onAutoExecute = onAutoExecute;
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