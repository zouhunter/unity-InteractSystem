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
        public UnityAction<AnimObj> onEndPlay;
        private Coroutine delyPlay;

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
        public override void StartExecute(bool forceauto = false)
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
            if (onEndPlay != null) onEndPlay.Invoke(this);
            onBeforePlayEnd.Invoke();
            gameObject.SetActive(endActive);
        }

        public override void EndExecute()
        {
            base.EndExecute();
            if (delyPlay != null) StopCoroutine(delyPlay);
            if (animPlayer != null) animPlayer.EndPlay();
            OnAutoEndPlay();
        }

        public override void UnDoExecute()
        {
            base.UnDoExecute();
            if (delyPlay != null) StopCoroutine(delyPlay);
            if (animPlayer != null) animPlayer.UnDoPlay();
        }
    }
}