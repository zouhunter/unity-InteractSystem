using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class MatchCommand : IActionCommand
    {
        public string StepName { get; private set; }
        private Func<MatchCtrl> createFunc;
        private MatchCtrl _matchCtrl;
        private MatchCtrl matchCtrl
        {
            get
            {
                if (_matchCtrl == null)
                {
                    _matchCtrl = createFunc.Invoke();
                }
                return _matchCtrl;
            }
        }
        public MatchCommand(string stepName, Func<MatchCtrl> createFunc)
        {
            this.StepName = stepName;
            this.createFunc = createFunc;
        }
        public void StartExecute(bool forceAuto)
        {
            matchCtrl.StartMatch(forceAuto);
        }
        public void EndExecute()
        {
            matchCtrl.CompleteMatch();
            _matchCtrl = null;
        }
        public void UnDoExecute()
        {
            matchCtrl.UnDoMatch();
            _matchCtrl = null;
        }
    }

}