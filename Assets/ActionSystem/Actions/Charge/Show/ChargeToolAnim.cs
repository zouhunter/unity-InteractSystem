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
        [SerializeField]
        private float animTime = 2;


        private ScaleChargeCtrl scaleCtrl;
        private AnimChargeCtrl animCtrl;

        private float timer = 0;
        private bool asyncActive = false;
        private UnityAction onComplete;

        protected override void Awake()
        {
            base.Awake();
            scaleCtrl = new ScaleChargeCtrl(scaleTrans);
            animCtrl = new AnimChargeCtrl(animParent, anim);
        }
        private void Update()
        {
            scaleCtrl.Update();

            if (asyncActive && timer < animTime)
            {
                timer += Time.deltaTime;
                if (timer > animTime)
                {
                    CompleteAsync();
                }
            }
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

        private void StartAsync(UnityAction onComplete)
        {
            this.onComplete = onComplete;
            asyncActive = true;
            timer = 0;
        }
        private void CompleteAsync()
        {
            asyncActive = false;
            if (onComplete != null)
            {
                onComplete.Invoke();
                animCtrl.StopAnim();
            }
        }
    }

}