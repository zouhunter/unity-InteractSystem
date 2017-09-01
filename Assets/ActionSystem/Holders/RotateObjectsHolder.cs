//using System;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Collections;
//using System.Collections.Generic;
//namespace WorldActionSystem
//{
//    public class RotateObjectsHolder : ActionHolder
//    {
//        private RotateTrigger rotParent;
//        private AnimGroup animParent;
//        private bool _registed;
//        private string currStepName;
//        public override bool Registed
//        {
//            get
//            {
//                return _registed;
//            }
//        }

//        private void Awake()
//        {
//            animParent = GetComponentInChildren<AnimGroup>();
//            rotParent = GetComponentInChildren<RotateTrigger>();
//            rotParent.onAllRotateOK = PlayAnim;
//            animParent.onAllElementInit = OnAllAnimInit;
//        }

//        public override void SetHighLight(bool on)
//        {
//            rotParent.SetHighLightState(on);
//        }

//        private void OnAllAnimInit(Dictionary<string, List<AnimObj>> dic)
//        {
//            foreach (var list in dic)
//            {
//                var cmd = new RotateCommand(list.Key, rotParent, animParent);
//                cmd.onBeforeExecute = ActiveStep;
//                if (OnRegistCommand != null) OnRegistCommand(cmd);
//                foreach (var obj in list.Value){
//                    obj.RegistEndPlayEvent( OnEndPlay);
//                }
//            }
//            _registed = true;
//        }
//        private void PlayAnim()
//        {
//            animParent.PlayAnim(currStepName);
//        }

//        private void OnEndPlay(string StepName)
//        {
//            if (CurrStapComplete())
//            {
//                if (OnStepEnd != null)
//                    OnStepEnd.Invoke(StepName);
//            }
//        }
//        private void ActiveStep(string StepName)
//        {
//            currStepName = StepName;
//        }

//        public bool CurrStapComplete()
//        {
//            bool complete = true;
//            var list = animParent.GetCurrAnims(currStepName);
//            foreach (var item in list)
//            {
//                complete &= item.Complete;
//            }
//            return complete;
//        }

//    }
//}