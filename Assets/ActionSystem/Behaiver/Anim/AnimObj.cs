using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class AnimObj : MonoBehaviour, IOutSideRegister
    {
        public string stapName;
        public bool endActive;
        public bool startActive;
        public float delyTime = 0f;
        public Animation anim;
        private AnimCore core;
        private bool _complete;
        public bool Complete { get { return _complete; } }
        public InstallAnim installAnim;

        [Range(0.1f, 10f)]
        public float speed = 1;

        [SerializeField] private UnityEvent onPlay;
        [SerializeField] private UnityEvent unDo;
        [SerializeField] private UnityEvent onPlayEnd;

        public UnityAction<AnimObj> onEndPlay;

        void Awake()
        {
            if (anim == null) anim = GetComponentInChildren<Animation>();
            if (anim != null) core = AnimCore.Init(anim, onPlayEnd);
            if (installAnim == null) installAnim = GetComponentInChildren<InstallAnim>();
            if (installAnim != null) installAnim.Init(onPlayEnd);
            RegisterInsideEvents();
        }

        void Start()
        {
            if(anim != null|| installAnim != null)
            gameObject.SetActive(startActive);
        }

        public void RegisterOutSideAnim(Animation anim)
        {
            if (anim != null)
            {
                this.anim = anim;
                core = AnimCore.Init(anim, onPlayEnd);
                gameObject.SetActive(startActive);
            }
        }

        public void RegisterOutSideAnim(InstallAnim installAnim)
        {
            if (installAnim != null) {
                this.installAnim = installAnim;
                installAnim.Init(onPlayEnd);
                gameObject.SetActive(startActive);
            }
        }

        void RegisterInsideEvents()
        {
            onPlayEnd.AddListener(() =>
            {
                _complete = true;
                if (onEndPlay != null) onEndPlay.Invoke(this);
                gameObject.SetActive(endActive);
            });

            onPlay.AddListener(() =>
            {
                _complete = false;
                gameObject.SetActive(true);
            });

            unDo.AddListener(() =>
            {
                _complete = false;
                gameObject.SetActive(startActive);
            });
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        public void PlayAnim()
        {
            onPlay.Invoke();
            StartCoroutine(DelyPlay());
        }

        private IEnumerator DelyPlay()
        {
            yield return new WaitForSeconds(delyTime);
            if (core != null) core.Play(speed);
            if (installAnim != null) installAnim.Play(1f / speed);
        }

        public void EndPlay()
        {
            onPlayEnd.Invoke();
            if (core != null) core.EndPlay();
            if (installAnim != null) installAnim.EndPlay();
        }

        public void UnDoPlay()
        {
            _complete = false;
            unDo.Invoke();
            if (core != null) core.UnDoPlay();
            if (installAnim != null) installAnim.UnDoPlay();
        }
    }
}