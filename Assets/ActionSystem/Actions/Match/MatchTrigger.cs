using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class MatchTrigger : ActionTrigger
    {
        public float distence;
        public bool highLight;
        public List<MatchObj> matchObjs { get { return _matchObjs; } }
        private List<MatchObj> _matchObjs = new List<MatchObj>();
        private MatchCtrl matchCtrl;
        protected override void Awake()
        {
            base.Awake();
            _matchObjs.AddRange(Array.ConvertAll<ActionObj, MatchObj>(actionObjs, x => (MatchObj)x));
        }
        public override IList<IActionCommand> CreateCommands()
        {
            var cmds = new List<IActionCommand>();
            cmds.Add(new MatchCommand(StepName, CreateMatchCtrl));
            return cmds;
        }
        private MatchCtrl CreateMatchCtrl()
        {
            matchCtrl = new MatchCtrl(this, distence, highLight, ElementController(), matchObjs);
            matchCtrl.onMatchComplete = OnStepComplete;
            matchCtrl.onMatchError = base.OnUserError;
            return matchCtrl;
        }

        private void OnStepComplete()
        {
            matchCtrl = null;
            OnComplete();
        }
    }
}