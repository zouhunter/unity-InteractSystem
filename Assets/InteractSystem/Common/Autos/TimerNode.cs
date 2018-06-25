using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using NodeGraph;
using System;

namespace InteractSystem.Auto
{
    [CustomNode("Auto/Timer", 1, "InteractSystem")]
    public class TimerNode : Graph.OperaterNode
    {
        [SerializeField]
        protected float autoTime = 2;
        private CoroutineController coroutineCtrl { get { return CoroutineController.Instence; } }

        protected override void OnStartExecuteInternal()
        {
            base.OnStartExecuteInternal();
            coroutineCtrl.DelyExecute(AutoComplete, autoTime);
        }

        private void AutoComplete()
        {
            OnEndExecute(false);
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            coroutineCtrl.Cansalce(AutoComplete);
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            coroutineCtrl.Cansalce(AutoComplete);
        }
    }
}
