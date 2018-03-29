using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    [AddComponentMenu(MenuName.AnimObj)]
    public class AnimObj : ActionObj
    {
        public float delyTime = 0f;
        public AnimPlayer animPlayer;
        [Range(0.1f, 10f)]
        public float speed = 1;
        [SerializeField]
        private bool playAtPostion;
        private Coroutine delyPlay;

        public override ControllerType CtrlType
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        public override void OnStartExecute(bool forceauto)
        {
            base.OnStartExecute(forceauto);
            FindAnimCore();
            Debug.Assert(animPlayer != null);
            delyPlay = StartCoroutine(DelyPlay());
        }

        private IEnumerator DelyPlay()
        {
            yield return new WaitForSeconds(delyTime);
            if (animPlayer != null)
            {
                animPlayer.duration = speed;
                animPlayer.onAutoPlayEnd = OnAnimPlayCallBack;
                animPlayer.StepActive();
            }
        }
        private void OnAnimPlayCallBack()
        {
            OnEndExecute(false);
        }

        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);
            if (delyPlay != null) StopCoroutine(delyPlay);
            if (animPlayer != null)
            {
                animPlayer.BindingTarget = this;
                animPlayer.StepComplete();
                animPlayer.SetVisible(endActive);
                animPlayer.gameObject.SetActive(endActive);
            }
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (delyPlay != null) StopCoroutine(delyPlay);
            if (animPlayer != null)
            {
                animPlayer.StepUnDo();
                animPlayer.BindingTarget = null;
                animPlayer = null;
            }
        }

        private void FindAnimCore()
        {
            animPlayer = GetComponentInChildren<AnimPlayer>(true);
            if(animPlayer == null)
            {
                var elements = elementCtrl.GetElements<AnimPlayer>(Name);
                if(elements != null && elements.Count > 0)
                {
                    animPlayer = elements.Find(x => x.Body != null && x.BindingTarget == null);//[0];
                }
            }

            if(animPlayer)
            {
                animPlayer.gameObject.SetActive(true);

                if (playAtPostion)
                {
                    animPlayer.transform.localPosition = transform.position;
                    animPlayer.transform.localRotation = transform.rotation;
                }
            }
        }

    }
}