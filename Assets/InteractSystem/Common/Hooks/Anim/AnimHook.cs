using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using NodeGraph;

namespace InteractSystem.Hooks
{
    //[CustomNode("Auto/Anim", 0, "InteractSystem")]
    [AddComponentMenu(MenuName.AnimHook)]
    public class AnimHook : ActionHook
    {
        [SerializeField]
        private float delyTime = 0f;
        [SerializeField, Attributes.Range(0.1f, 10f)]
        private float speed = 1;
        [SerializeField]
        private bool reverse;
        [SerializeField, Attributes.DefultName]
        private string _animName;
        public string animName
        {
            get
            {
                if (string.IsNullOrEmpty(_animName))
                {
                    return name;
                }
                return _animName;
            }
        }
        private AnimPlayer animPlayer;
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
            animPlayer.duration = speed;
            animPlayer.reverse = reverse;
            animPlayer.onAutoPlayEnd = OnAnimPlayCallBack;
            animPlayer.SetVisible(true);
            animPlayer.StepActive();
        }
        private void OnAnimPlayCallBack()
        {
            OnEndExecute(false);
        }

        protected override void OnBeforeEndExecute()
        {
            base.OnBeforeEndExecute();
            coroutineCtrl.Cansalce(DelyPlay);
            if (animPlayer == null){
                FindAnimCore(true);
            }
            Debug.Assert(animPlayer != null, "缺少：" + animName);
            animPlayer.StepComplete();
        }


        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            coroutineCtrl.Cansalce(DelyPlay);
            animPlayer.StepUnDo();
            animPlayer.RemovePlayer(this);
            animPlayer = null;
        }

        private void FindAnimCore(bool record)
        {
            if (animPlayer == null)
            {
                var elements = elementCtrl.GetElements<AnimPlayer>(animName, true);
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