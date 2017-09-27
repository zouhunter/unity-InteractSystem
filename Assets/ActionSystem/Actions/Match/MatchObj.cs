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
        public IMatchItem obj { get; private set; }
        void Awake()
        {
            gameObject.layer = Setting.matchPosLayer;
        }
        public bool Attach(IMatchItem obj)
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

        public IMatchItem Detach()
        {
            IMatchItem old = obj;
            obj = default(IMatchItem);
            return old;
        }
    }
}