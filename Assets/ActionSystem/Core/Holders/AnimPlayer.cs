using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public abstract class AnimPlayer: ActionItem
    {
        [SerializeField]
        protected bool _reverse;
        [SerializeField]
        protected int playableCount = 1;
        protected float _duration = 1;
        public virtual float duration { get { return _duration; }set { _duration = value; } }
        public virtual bool reverse { get { return _reverse; } set { _reverse = value; } }
        public UnityAction onAutoPlayEnd { get; set; }
        public bool IsPlaying { get; protected set; }
        [HideInInspector]
        public UnityEvent onPlayComplete;

        public override void StepActive()
        {
            base.StepActive();
            IsPlaying = true;
        }
        public override void StepUnDo()
        {
            base.StepUnDo();
            IsPlaying = false;
        }
        public override void StepComplete()
        {
            base.StepComplete();
            IsPlaying = false;
            onPlayComplete.Invoke();
        }
     
        public virtual bool CanPlay()
        {
            if(targets.Count < playableCount && !IsPlaying)
            {
                return true;
            }
            return false;
        }
        
    }

}
