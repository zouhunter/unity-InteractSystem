using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem.Graph;

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
        protected event UnityAction onAutoPlayEnd;
        public bool IsPlaying { get; protected set; }
        [HideInInspector]
        public UnityEvent onPlayComplete;
        [SerializeField, Attributes.CustomField("节点支持")]
        protected int playableCount = 1;
        public override bool OperateAble
        {
             get { return playableCount > targets.Count && !IsPlaying; } 
        }
        protected CompleteAbleItemFeature completeAbleFeature = new CompleteAbleItemFeature();

        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            completeAbleFeature.Init(this, OnPlayAnim);
            onAutoPlayEnd = ()=> completeAbleFeature.OnComplete(firstLock);
            features.Add(completeAbleFeature);
            return features;
        }

        protected virtual void OnPlayAnim(UnityEngine.Object arg0)
        {
            IsPlaying = true;
        }

        protected void OnAnimComplete()
        {
            IsPlaying = false;
            if (onAutoPlayEnd != null)
                onAutoPlayEnd.Invoke();
        }

        protected override void OnSetActive(UnityEngine.Object target)
        {
            base.OnSetActive(target);
        }

        protected override void OnSetInActive(UnityEngine.Object target)
        {
            base.OnSetInActive(target);
            IsPlaying = false;
        }

        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            IsPlaying = false;
        }
        
    }

}
