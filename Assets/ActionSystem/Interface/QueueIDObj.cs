using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public abstract class QueueIDObj : ActionObj,ISortAble
    {
        [SerializeField]
        private int queueID;
        public int QueueID
        {
            get
            {
                return queueID;
            }
        }
        public UnityAction<int> onEndExecute;

        public override void EndExecute()
        {
            base.EndExecute();
            if (onEndExecute != null)
            {
                onEndExecute.Invoke(queueID);
            }
        }
    }

}