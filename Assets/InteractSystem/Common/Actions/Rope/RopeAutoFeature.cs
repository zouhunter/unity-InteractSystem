using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
    public class RopeAutoFeature : CompleteAbleItemFeature
    {
        CoroutineController coroutineCtrl { get { return CoroutineController.Instence; } }

        public override void AutoExecute()
        {
            coroutineCtrl.StartCoroutine((target as RopeItem).AutoConnectRopeNodes(OnComplete));
        }
    }
}