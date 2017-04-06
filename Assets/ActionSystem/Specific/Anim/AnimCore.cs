using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
namespace WorldActionSystem
{

    public class AnimCore : MonoBehaviour
    {
        private UnityEvent onPlayEnd;
        private Animation anim;
        private string animName;
        private AnimationClip clip;
        private AnimationState state;
        private float animTime;
        private AnimationEvent even;
        private float speed;

        public static AnimCore Init(Animation anim, float speed, UnityEvent onPlayEnd)
        {
            AnimCore core = null;
            if (anim != null)
            {
                core = anim.gameObject.AddComponent<AnimCore>();
                core.anim = anim;
                core.anim.playAutomatically = false;
                core.speed = speed;
                core.onPlayEnd = onPlayEnd;
                core.RegisterEvent();
            }
         
            return core;
        }


        void RegisterEvent()
        {
            animName = anim.clip.name;
            state = anim[animName];
            animTime = state.length;
            state.speed = speed;
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
        public void Play()
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