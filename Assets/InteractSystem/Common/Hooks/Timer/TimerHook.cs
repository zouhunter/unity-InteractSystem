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
using System;

namespace InteractSystem.Hooks
{
    [AddComponentMenu(MenuName.TimeHook)]
    public class TimerHook : ActionHook
    {
        [SerializeField]
        protected float autoTime = 2;

        protected override void CoreStartExecute()
        {
            base.CoreStartExecute();
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
