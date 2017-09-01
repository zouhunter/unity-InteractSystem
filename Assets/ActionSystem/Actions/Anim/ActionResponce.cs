using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public abstract class ActionResponce : MonoBehaviour
    {
        [SerializeField]
        private string _stepName;
        public string StepName { get { return _stepName; } }
        public StepComplete OnStepEnd;
        public UserError onUserErr;
        public abstract IActionCommand CreateCommand();
    }
}
