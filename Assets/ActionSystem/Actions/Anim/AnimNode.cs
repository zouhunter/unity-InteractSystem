using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using NodeGraph;

namespace WorldActionSystem.Graph
{
    [CustomNode("Auto/Anim", 0, "ActionSystem")]
    [AddComponentMenu(MenuName.AnimObj)]
    public class AnimNode : OperateNode
    {
        [SerializeField]
        private float delyTime = 0f;
        [SerializeField,Attributes.Range(0.1f, 10f)]
        private float speed = 1;
        [SerializeField]
        private bool reverse;
        [SerializeField]
        private string animName;
        private AnimPlayer animPlayer;
        private CoroutineController coroutineCtrl { get { return ActionSystem.Instence.CoroutineCtrl; } }
        private ElementController elementCtrl { get { return ElementController.Instence; } }
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
        public override void OnStartExecute(bool auto)
        {
            base.OnStartExecute(auto);
            coroutineCtrl.DelyExecute(DelyPlay, delyTime);
        }

        private void DelyPlay()
        {
            FindAnimCore(true);
            Debug.Assert(animPlayer != null, "no enough animplayer named:" + Name);
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

        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);
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
                var elements = elementCtrl.GetElements<AnimPlayer>(animName);
                if (elements != null && elements.Count > 0)
                {
                    animPlayer = elements.Find(x => x.Body != null && x.CanPlay());//[0];
                }
            }

            if (animPlayer)
            {
                if (record){
                    animPlayer.RecordPlayer(this);
                }
                animPlayer.gameObject.SetActive(true);
            }
        }

    }
}