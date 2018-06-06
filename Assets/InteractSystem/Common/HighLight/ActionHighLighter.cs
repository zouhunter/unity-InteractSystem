using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;

namespace InteractSystem.Binding
{
    public class ActionHighLighter : OperaterBinding
    {
        [SerializeField]
        protected Color color = Color.green;
        [SerializeField]
        protected GameObject viewObj;
        protected Highlighter highLighter;
        protected bool noticeAuto { get { return Config.highLightNotice; } }
      
        public void HighLight()
        {
            if (highLighter)
            {
                highLighter.FlashingOn(Color.white, color);
            }
        }

        public void UnHighLight()
        {
            if (highLighter)
            {
                highLighter.FlashingOff();
                highLighter.Off();
            }
        }
    }
}