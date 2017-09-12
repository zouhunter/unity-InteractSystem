using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
namespace WorldActionSystem
{

    public interface IActionCommand
    {
        string StepName { get; }
        int Count { get; }
        void StartExecute(bool forceAuto);
        void EndExecute();
        void UnDoExecute();
    }


}