using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class QueueIDCommand:CoroutionCommand
    {
        public int Count { get; private set; }
        protected override ICoroutineCtrl CreateCtrl()
        {
            return new QueueIDCtrl(this);
        }
    }
}