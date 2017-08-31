using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class AnimGroup : MonoBehaviour
    {
        public UnityAction<Dictionary<string, List<AnimObj>>> onAllElementInit;
        private Dictionary<string, List<AnimObj>> animDic = new Dictionary<string, List<AnimObj>>();
        private void Start()
        {
            var animObjects = GetComponentsInChildren<AnimObj>(true);

            foreach (AnimObj anim in animObjects)
            {
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
            if (onAllElementInit != null) onAllElementInit.Invoke(animDic);
        }

       
        public List<AnimObj> GetCurrAnims(string currStepName)
        {
            return animDic[currStepName];
        }

        public void PlayAnim(string currStepName)
        {
            var anims = animDic[currStepName];
            if (anims != null)
            {
                foreach (var anim in anims)
                {
                    anim.PlayAnim();
                }
            }
        }
        internal void SetAnimEnd(string stepName)
        {
            var anims = animDic[stepName];

            if (anims != null)
            {
                foreach (var anim in anims)
                {
                    anim.EndPlay();
                }
            }
        }

        internal void SetAnimUnPlayed(string stepName)
        {
            var anims = animDic[stepName];

            if (anims != null)
            {
                foreach (var anim in anims)
                {
                    anim.UnDoPlay();
                }
            }
        }

    }

}