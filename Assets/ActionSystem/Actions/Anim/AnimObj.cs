using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class AnimObj : ActionObj, IOutSideRegister
    {
        public float delyTime = 0f;
        public AnimPlayer animPlayer;
        [Range(0.1f, 10f)]
        public float speed = 1;
        private Coroutine delyPlay;

        protected override void Start()
        {
            base.Start();
            animPlayer = GetComponentInChildren<AnimPlayer>();
            if (animPlayer != null){
                gameObject.SetActive(startActive);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
        
        public void RegisterOutSideAnim(AnimPlayer animPlayer)
        {
            if (animPlayer != null)
            {
                this.animPlayer = animPlayer;
                gameObject.SetActive(startActive);
            }
        }
        /// <summary>
        /// 播放动画
        /// </summary>
        public override void OnStartExecute(bool forceauto)
        {
            base.OnStartExecute(forceauto);
            delyPlay = StartCoroutine(DelyPlay());
        }

        private IEnumerator DelyPlay()
        {
            yield return new WaitForSeconds(delyTime);
            if (animPlayer != null) animPlayer.Play(speed, TryEndExecute);
        }

        public override void OnEndExecute()
        {
            base.OnEndExecute();
            if (delyPlay != null) StopCoroutine(delyPlay);
            if (animPlayer != null) animPlayer.EndPlay();
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (delyPlay != null) StopCoroutine(delyPlay);
            if (animPlayer != null) animPlayer.UnDoPlay();
        }
    }
}