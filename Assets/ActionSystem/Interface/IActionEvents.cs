using UnityEngine;
using System.Collections;
using System;
namespace WorldActionSystem
{

    public interface IActionEvents
    {
        StepComplete onStepComplete { get; set; }
        UserError onUserErr { get; set; }
    }
}
