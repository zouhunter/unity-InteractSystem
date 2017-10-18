using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public interface IActionObj
    {
        int QueueID { get; }
        bool Complete { get; }
        bool Started { get; }

        UnityAction<int> onEndExecute { get; set; }

        void OnUnDoExecute();
        void OnEndExecute();
        void OnStartExecute(bool isForceAuto);
    }
}