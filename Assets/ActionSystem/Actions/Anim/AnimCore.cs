using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System;

namespace WorldActionSystem
{
    public class AnimCore : MonoBehaviour, AnimPlayer
    {
        public Animation anim;
        public string animName;
        private UnityAction onAutoPlayEnd;
        private AnimationClip clip;
        private AnimationState state;
        private float animTime;
        private AnimationEvent even;
        private Coroutine coroutine;
        private void Awake()
        {
            if(anim == null) anim = GetComponent<Animation>();
            if (string.IsNullOrEmpty(animName)) animName = anim.clip.name;
        }
        void Init(UnityAction onAutoPlayEnd)
        {
            anim.playAutomatically = false;
            anim.wrapMode = WrapMode.Once;
            this.onAutoPlayEnd = onAutoPlayEnd;
            RegisterEvent();
        }

        void RegisterEvent()
        {
            state = anim[animName];
            animTime = state.length;
            anim.cullingType = AnimationCullingType.BasedOnRenderers;
            anim.clip = clip = anim.GetClip(animName);
        }

        public void Play(float speed, UnityAction onAutoPlayEnd)
        {
            Init(onAutoPlayEnd);
            state.normalizedTime = 0f;
            state.speed = speed;
            anim.Play();
            if(coroutine == null) coroutine = StartCoroutine(DelyStop());
        }
        IEnumerator DelyStop()
        {
            float waitTime = animTime / state.speed;
            yield return new WaitForSeconds(waitTime);
            onAutoPlayEnd.Invoke();
        }
        /// <summary>
        /// 强制完成
        /// </summary>
        public void EndPlay()
        {
            state.normalizedTime = 1f;
            state.normalizedSpeed = 0;
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = null;
        }
        public void UnDoPlay()
        {
            state.normalizedTime = 0f;
            state.speed = 0;
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = null;
        }


    }
}