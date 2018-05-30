using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace WorldActionSystem.Actions
{
    public class AnimGroup : AnimPlayer
    {
        private AnimPlayer[] childAnims;
        private bool actived;
        private int completedCount;
        public override float duration
        {
            get
            {
                return _duration;
            }

            set
            {
                _duration = value;
                SetChildAnimDuration();
            }
        }
  
        protected override void OnEnable()
        {
            base.OnEnable();
            childAnims = GetComponentsInChildren<AnimPlayer>(true).Where(x=>x != this).ToArray();
        }

        //public override void SetPosition(Vector3 pos)
        //{
        //    transform.position = pos;
        //}

        public override void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public override void StepActive()
        {
            actived = true;
            completedCount = 0;
            foreach (var item in childAnims)
            {
                item.onAutoPlayEnd = OnPlayEnd;
                item.StepActive();
            }
        }

     
        public override void StepComplete()
        {
            actived = false;
            foreach (var item in childAnims)
            {
                item.StepComplete();
            }
        }

        public override void StepUnDo()
        {
            actived = false;
            foreach (var item in childAnims)
            {
                item.StepUnDo();
            }
        }

        private void SetChildAnimDuration()
        {
            foreach (var item in childAnims)
            {
                item.duration = duration;
            }
        }

        private void OnPlayEnd()
        {
            if (!actived) return;

            completedCount++;
            if(completedCount >= childAnims.Length)
            {
               if(onAutoPlayEnd != null)
                    onAutoPlayEnd.Invoke();
            }
        }
    }
}