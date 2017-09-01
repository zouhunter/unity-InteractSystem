using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System;

namespace WorldActionSystem
{
    [RequireComponent(typeof(Animation))]
    public class AnimCore : MonoBehaviour, AnimPlayer
    {
        private UnityAction onPlayEnd;
        private Animation anim;
        private string animName;
        private AnimationClip clip;
        private AnimationState state;
        private float animTime;
        private AnimationEvent even;
        private void Awake()
        {
            anim = GetComponent<Animation>();
        }
        public void Init(UnityAction onPlayEnd)
        {
            anim.playAutomatically = false;
            anim.wrapMode = WrapMode.Once;
            this.onPlayEnd = onPlayEnd;
            RegisterEvent();
        }

        void RegisterEvent()
        {
            animName = anim.clip.name;
            state = anim[animName];
            animTime = state.length;
            anim.cullingType = AnimationCullingType.BasedOnRenderers;

            clip = anim.GetClip(animName);
            even = new AnimationEvent();
            even.time = animTime;
            even.functionName = "OnPlayToEnd";
            clip.AddEvent(even);
        }

        /// <summary>
        /// 完成事件
        /// </summary>
        void OnPlayToEnd()
        {
            onPlayEnd.Invoke();
        }
        public void Play(float speed)
        {
            state.normalizedTime = 0f;
            state.speed = speed;
            anim.Play();
        }
        /// <summary>
        /// 强制完成
        /// </summary>
        public void EndPlay()
        {
            state.normalizedTime = 1f;
            state.normalizedSpeed = 0;
        }
        public void UnDoPlay()
        {
            state.normalizedTime = 0f;
            state.speed = 0;
        }

     
    }
}