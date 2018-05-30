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

namespace WorldActionSystem.Hooks
{
    [AddComponentMenu(MenuName.AutoObj)]
    public sealed class TimerHook : ActionHook
    {
        public float waitTime = 0;
        protected override bool autoComplete
        {
            get
            {
                return true;
            }
        }
        private void OnEnable()
        {
            autoTime = waitTime;
        }
    }
}
