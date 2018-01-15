using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;
using System;
namespace WorldActionSystem
{
    public class ChargeToolAnim : ChargeToolBingding
    {
        [SerializeField]
        private Animation anim;
        [SerializeField]
        private Transform animParent;
        [SerializeField]
        private string loadAnimName;
        [SerializeField]
        private string chargeAnimName;
        [SerializeField]
        private Transform scaleTrans;
     
        private ScaleChargeCtrl scaleCtrl;
        private AnimChargeCtrl animCtrl;
        
        protected override void Awake()
        {
            base.Awake();
            scaleCtrl = new ScaleChargeCtrl(scaleTrans,target.capacity);
            animCtrl = new AnimChargeCtrl(animParent, anim);
        }
        protected override void Update()
        {
            base.Update();
            scaleCtrl.Update();
        }

        protected override void OnCharge(Vector3 center, ChargeData data, UnityAction onComplete)
        {
            if (onComplete != null)
            {
                scaleCtrl.SubAsync(data, animTime);
                animCtrl.PlayAnim(chargeAnimName, center, animTime);
                StartAsync(onComplete);
            }
            else
            {
                scaleCtrl.Sub(data);
            }
        }

        protected override void OnLoad(Vector3 center, ChargeData data, UnityAction onComplete)
        {
            if (onComplete != null)
            {
                scaleCtrl.AddAsync(data, animTime);
                animCtrl.PlayAnim(loadAnimName, center, animTime);
                StartAsync(onComplete);
            }
            else
            {
                scaleCtrl.Add(data);
            }
        }
        protected override void CompleteAsync()
        {
            base.CompleteAsync();
            if(animCtrl != null){
                animCtrl.StopAnim();
            }
        }
    }

}