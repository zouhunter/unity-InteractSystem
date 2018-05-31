using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using NodeGraph;

namespace WorldActionSystem.Hooks
{
    //[CustomNode("Auto/Anim", 0, "ActionSystem")]
    [AddComponentMenu(MenuName.AnimObj)]
    public class AnimHook : ActionHook
    {
        [SerializeField]
        private float delyTime = 0f;
        [SerializeField, Attributes.Range(0.1f, 10f)]
        private float speed = 1;
        [SerializeField]
        private bool reverse;
        [SerializeField]
        private string animName;
        private AnimPlayer animPlayer;
        private CoroutineController coroutineCtrl { get { return ActionSystem.Instence.CoroutineCtrl; } }
        private ElementController elementCtrl { get { return ElementController.Instence; } }

        /// <summary>
        /// 播放动画
        /// </summary>
        public override void OnStartExecute(bool auto)
        {
            base.OnStartExecute(auto);
            coroutineCtrl.DelyExecute(DelyPlay, delyTime);
        }

        private void DelyPlay()
        {
            FindAnimCore(true);
            Debug.Assert(animPlayer != null, "no enough animplayer named:" + this);
            if (animPlayer != null)
            {
                animPlayer.duration = speed;
                animPlayer.reverse = reverse;
                animPlayer.onAutoPlayEnd = OnAnimPlayCallBack;
                animPlayer.SetVisible(true);
                animPlayer.StepActive();
            }
        }
        private void OnAnimPlayCallBack()
        {
            OnEndExecute(false);
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            if (animPlayer != null)
            {
                animPlayer.StepComplete();
            }
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (animPlayer != null)
            {
                animPlayer.StepUnDo();
                animPlayer.RemovePlayer(this);
            }
        }

        private void FindAnimCore(bool record)
        {
            if (animPlayer == null)
            {
                var elements = elementCtrl.GetElements<AnimPlayer>(animName,true);
                if (elements != null && elements.Count > 0)
                {
                    animPlayer = elements.Find(x => x.Body != null && x.CanPlay());//[0];
                }
            }

            if (animPlayer)
            {
                if (record)
                {
                    animPlayer.RecordPlayer(this);
                }
                animPlayer.gameObject.SetActive(true);
            }
        }

    }
}