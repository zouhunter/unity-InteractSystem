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
        ActionCtroller ActionCtrl { get; }
        string StepName { get; }
        bool Startd { get; }
        bool Completed { get; }
        bool StartExecute(bool forceAuto);
        bool EndExecute();
        void OnEndExecute();
        void UnDoExecute();
    }


}