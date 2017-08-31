using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    /// <summary>
    /// 注册所有动画命令
    /// </summary>=
    public class AnimationObjectsHolder : ActionHolder
    {
        public override bool Registed{
            get{
                return registed;
            }
        }
        private bool registed;

        private AnimObj[] animObjects ;
        private Dictionary<string, List<AnimObj>> animDic = new Dictionary<string, List<AnimObj>>();
        private string CurrentStep;
        void Start()
        {
            animObjects = GetComponentsInChildren<AnimObj>(true);

            foreach (AnimObj anim in animObjects){
                anim.onEndPlay = OnEndPlayAnim;
                var obj = anim;
                if (animDic.ContainsKey(obj.stapName))
                {
                    animDic[obj.stapName].Add(obj);
                }
                else
                {
                    animDic[obj.stapName] = new List<AnimObj>() { obj };
                }
            }
            foreach (var item in animDic)
            {
                ActionCommand cmd = new AnimCommand(item.Key,item.Value.ToArray());
                cmd.onExecuteAction += (step) => { CurrentStep = step; };
                OnRegistCommand(cmd);
            }
            registed = true;
        }

        private void OnEndPlayAnim(AnimObj obj)
        {
            if (CurrentStepComplete())
            {
                if (OnStepEnd != null)
                    OnStepEnd.Invoke(obj.stapName);
            }
        }

        private bool CurrentStepComplete()
        {
            bool complete = true;
            foreach (var item in animDic[CurrentStep])
            {
                complete &= item.Complete;
            }
            return complete;
        }
        public override void SetHighLight(bool on)
        {
            //throw new NotImplementedException();
        }
    }

}