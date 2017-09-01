using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class AnimObj : ResponceObj, IOutSideRegister
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
                animPlayer.Init(OnAutoEndPlay);
                gameObject.SetActive(startActive);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public void RegistEndPlayEvent(UnityAction<string> onEndPlayAnim)
        {
            onBeforePlayEnd.AddListener(onEndPlayAnim);
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
            base.EndExecute();
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