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
        private Dictionary<UnityEngine.Object, UnityAction<CompleteAbleItemFeature>> onCompleteActions = new Dictionary<UnityEngine.Object, UnityAction<CompleteAbleItemFeature>>();
        public UnityAction<Graph.OperaterNode> onAutoExecute { get; private set; }

        public void RegistOnCompleteSafety(UnityEngine.Object context, UnityAction<CompleteAbleItemFeature> onClicked)
        {
            if (context != null && onClicked != null)
            {
                onCompleteActions[context] = onClicked;
            }
        }

        public void OnComplete(UnityEngine.Object context)
        {
            if (context != null && onCompleteActions.ContainsKey(context))
            {
                var action = onCompleteActions[context];
                onCompleteActions.Remove(context);
                action.Invoke(this);
            }
            else
            {
                Debug.LogWarning("have no on Complete action!");
            }
        }

        public void RemoveOnComplete(UnityEngine.Object context)
        {
            if (onCompleteActions.ContainsKey(context))
            {
                onCompleteActions.Remove(context);
            }
        }

        internal void Init(ActionItem actionItem, UnityAction<Graph.OperaterNode> onAutoExecute)
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