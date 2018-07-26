using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace InteractSystem.Hooks
{
    public class AnimGroup : AnimPlayer
    {
        private AnimPlayer[] childAnims;
        private bool actived;
        private int completedCount;
        public override float speed
        {
            get
            {
                return base.speed;
            }

            set
            {
                base.speed = value;
                foreach (var item in childAnims){
                    item.speed = speed;
                }
            }
        }
        public override bool opposite
        {
            get
            {
                return base.opposite;
            }

            set
            {
                base.opposite = value;
                foreach (var item in childAnims)
                {
                    item.opposite = opposite;
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            childAnims = GetComponentsInChildren<AnimPlayer>(true).Where(x=>x != this).ToArray();
        }


        public override void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        protected override void OnPlayAnim(UnityEngine.Object target)
        {
            base.OnPlayAnim(target);

            actived = true;
            completedCount = 0;
            foreach (var item in childAnims)
            {
                var feature = item.RetriveFeature<CompleteAbleItemFeature>();
                feature.RegistOnCompleteSafety(target, OnPlayEnd);
                item.SetActive(target);
                feature.AutoExecute(target);
            }
        }

     
        protected override void OnSetInActive(UnityEngine.Object target)
        {
            actived = false;
            foreach (var item in childAnims)
            {
                item.SetInActive(target);
            }
        }

        public override void UnDoChanges(UnityEngine.Object target)
        {
            actived = false;
            foreach (var item in childAnims)
            {
                item.UnDoChanges(target);
            }
        }
       

        private void OnPlayEnd(CompleteAbleItemFeature feature)
        {
            if (!actived) return;

            completedCount++;
            if(completedCount >= childAnims.Length)
            {
                OnAnimComplete();
            }
        }
    }
}