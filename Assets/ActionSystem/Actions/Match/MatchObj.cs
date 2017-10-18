using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
#if !NoFunction
using DG.Tweening;
#endif
namespace WorldActionSystem
{
    public class MatchObj : ActionObj
    {
        public bool autoMatch;
        public bool Matched { get { return obj != null; } }
        public PickUpAbleElement obj { get; private set; }
        void Awake()
        {
            gameObject.layer = Setting.matchPosLayer;
        }
        public bool Attach(PickUpAbleElement obj)
        {
            if (this.obj != null){
                return false;
            }
            else
            {
                this.obj = obj;
                return true;
            }
        }

        public PickUpAbleElement Detach()
        {
            PickUpAbleElement old = obj;
            obj = default(PickUpAbleElement);
            return old;
        }
    }
}