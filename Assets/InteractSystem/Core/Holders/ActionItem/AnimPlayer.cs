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
        [SerializeField,Attributes.CustomField("反向")]
        protected bool _reverse;
        [SerializeField, Attributes.CustomField("速度比")]
        protected float _duration = 1;
        public virtual float speed { get; set; }
        public virtual bool opposite { get; set; }
        public virtual float duration { get { return speed * _duration; } }
        public virtual bool reverse { get { return opposite ? !_reverse:_reverse; } }
        public UnityAction onAutoPlayEnd { get; set; }
        public bool IsPlaying { get; protected set; }
        [HideInInspector]
        public UnityEvent onPlayComplete;
        [SerializeField, Attributes.CustomField("可播放数")]
        protected int playableCount = 1;
        public override bool OperateAble
        {
             get { return playableCount > targets.Count; } 
        }
        protected override void OnSetActive(UnityEngine.Object target)
        {
            base.OnSetActive(target);
            IsPlaying = true;
        }

        protected override void OnSetInActive(UnityEngine.Object target)
        {
            base.OnSetInActive(target);
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
