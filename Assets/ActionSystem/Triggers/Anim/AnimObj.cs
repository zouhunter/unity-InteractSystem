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
        private Coroutine delyPlay;
        void Awake()
        {
            if (anim == null) anim = GetComponentInChildren<Animation>();
            if (anim != null) core = AnimCore.Init(anim, OnAutoEndPlay);
            if (installAnim == null) installAnim = GetComponentInChildren<InstallAnim>();
            if (installAnim != null) installAnim.Init(OnAutoEndPlay);
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
                core = AnimCore.Init(anim, OnAutoEndPlay);
                gameObject.SetActive(startActive);
            }
        }

        public void RegisterOutSideAnim(InstallAnim installAnim)
        {
            if (installAnim != null) {
                this.installAnim = installAnim;
                installAnim.Init(OnAutoEndPlay);
                gameObject.SetActive(startActive);
            }
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        public void PlayAnim()
        {
            onPlay.Invoke();
            _complete = false;
            gameObject.SetActive(true);
            delyPlay = StartCoroutine(DelyPlay());
        }

        private IEnumerator DelyPlay()
        {
            yield return new WaitForSeconds(delyTime);
            if (core != null) core.Play(speed);
            if (installAnim != null) installAnim.Play(1f / speed);
        }
        private void OnAutoEndPlay()
        {
            _complete = true;
            if (onEndPlay != null) onEndPlay.Invoke(this);
            onPlayEnd.Invoke();
            gameObject.SetActive(endActive);
        }

        public void EndPlay()
        {
            if (core != null) core.EndPlay();
            if (installAnim != null) installAnim.EndPlay();
            if (delyPlay != null) StopCoroutine(delyPlay);
            OnAutoEndPlay();
        }

        public void UnDoPlay()
        {
            _complete = false;
            unDo.Invoke();
            gameObject.SetActive(startActive);
            if (core != null) core.UnDoPlay();
            if (installAnim != null) installAnim.UnDoPlay();
            if (delyPlay != null) StopCoroutine(delyPlay);
        }
    }
}