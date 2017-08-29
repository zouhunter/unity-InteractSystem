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

        public Animation anim;
        private AnimCore core;

        public InstallAnim installAnim;

        [Range(0.1f, 10f)]
        public float speed = 1;

        public UnityEvent onPlay;
        public UnityEvent unDo;
        public UnityEvent onPlayEnd;


        public IRemoteController RemoteCtrl
        {
            get
            {
                return ActionSystem.Instance.RemoteController;
            }
        }

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
                RemoteCtrl.EndExecuteCommand();
                gameObject.SetActive(endActive);
            });

            onPlay.AddListener(() =>
            {
                gameObject.SetActive(true);
            });

            unDo.AddListener(() =>
            {
                gameObject.SetActive(startActive);
            });
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        public void PlayAnim()
        {
            onPlay.Invoke();
            if (core != null) core.Play(speed);
            if (installAnim != null) installAnim.Play(1f/speed);
        }

        public void EndPlay()
        {
            onPlayEnd.Invoke();
            if (core != null) core.EndPlay();
            if (installAnim != null) installAnim.EndPlay();
        }

        public void UnDoPlay()
        {
            unDo.Invoke();
            if (core != null) core.UnDoPlay();
            if (installAnim != null) installAnim.UnDoPlay();
        }
    }
}