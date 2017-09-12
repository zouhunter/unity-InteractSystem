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
        private UnityAction<string> onAutoPlayEnd;
        protected override void Start()
        {
            base.Start();
            animPlayer = GetComponentInChildren<AnimPlayer>();
            if (animPlayer != null){
                animPlayer.Init(OnAutoEndPlay);
                gameObject.SetActive(startActive);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public void RegistAutoEndPlayEvent(UnityAction<string> onAutoEndPlay)
        {
            onAutoPlayEnd = onAutoEndPlay;
        }

        public void RegisterOutSideAnim(AnimPlayer animPlayer)
        {
            if (animPlayer != null)
            {
                this.animPlayer = animPlayer;
                animPlayer.Init(OnAutoEndPlay);
                gameObject.SetActive(startActive);
            }
        }
        /// <summary>
        /// 播放动画
        /// </summary>
        public override void StartExecute()
        {
            base.StartExecute();
            delyPlay = StartCoroutine(DelyPlay());
        }

        private IEnumerator DelyPlay()
        {
            yield return new WaitForSeconds(delyTime);
            if (animPlayer != null) animPlayer.Play(speed);
        }
        private void OnAutoEndPlay()
        {
            _complete = true;
            gameObject.SetActive(endActive);
            if (onAutoPlayEnd != null) onAutoPlayEnd.Invoke(StepName);
        }

        public override void EndExecute()
        {
            base.EndExecute();
            if (delyPlay != null) StopCoroutine(delyPlay);
            if (animPlayer != null) animPlayer.EndPlay();
        }

        public override void UnDoExecute()
        {
            base.UnDoExecute();
            if (delyPlay != null) StopCoroutine(delyPlay);
            if (animPlayer != null) animPlayer.UnDoPlay();
        }
    }
}