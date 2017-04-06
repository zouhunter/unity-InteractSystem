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

        [Range(0.1f, 10f)]
        public float speed = 1;

        public UnityEvent onPlay;
        public UnityEvent unDo;
        public UnityEvent onPlayEnd;

        private AnimCore core;

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
            if (anim != null) core = AnimCore.Init(anim, speed, onPlayEnd);
            RegisterInsideEvents();
        }

        void Start()
        {
            gameObject.SetActive(startActive);
        }

        public void RegisterOutSideAnim(Animation anim)
        {
            if (this.anim == null && anim != null)
            {
                this.anim = anim;
                core = AnimCore.Init(anim, speed, onPlayEnd);
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
            if (core != null) core.Play();
        }

        public void EndPlay()
        {
            onPlayEnd.Invoke();
            if (core != null) core.EndPlay();
        }

        public void UnDoPlay()
        {
            unDo.Invoke();
            if (core != null) core.UnDoPlay();
        }
    }
}