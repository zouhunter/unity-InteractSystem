using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem
{
    public abstract class AnimPlayer : ActionItem
    {
        [SerializeField]
        protected bool _reverse;
        protected float _duration = 1;
        public virtual float duration { get { return _duration; } set { _duration = value; } }
        public virtual bool reverse { get { return _reverse; } set { _reverse = value; } }
        public UnityAction onAutoPlayEnd { get; set; }
        public bool IsPlaying { get; protected set; }
        [HideInInspector]
        public UnityEvent onPlayComplete;
        [SerializeField]
        protected int playableCount = 1;
        public override bool OperateAble
        {
             get { return playableCount > targets.Count; } 
        }
        public override void SetActive(UnityEngine.Object target)
        {
            base.SetActive(target);
            IsPlaying = true;
        }

        public override void SetInActive(UnityEngine.Object target)
        {
            base.SetInActive(target);
            IsPlaying = false;
            onPlayComplete.Invoke();
        }

        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            IsPlaying = false;
        }

        public virtual bool CanPlay()
        {
            if (targets.Count < playableCount && !IsPlaying)
            {
                return true;
            }
            return false;
        }

    }

}
