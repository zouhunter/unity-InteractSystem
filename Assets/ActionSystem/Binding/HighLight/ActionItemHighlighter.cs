using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem.Binding
{
    public class ActionItemHighlighter : ActionItemBinding
    {
        [SerializeField]
        private Color highLightColor = Color.green;
        [SerializeField]
        protected GameObject viewObj;
        protected IHighLightItems highLighter;
        protected bool notice { get { return Config.highLightNotice; } }
        protected bool actived;

        protected override void Awake()
        {
            base.Awake();
            if (viewObj == null) viewObj = gameObject;
            highLighter = new ShaderHighLight();
        }
        protected void Update()
        {
            if (!notice) return;

            if (actionItem.Active)
            {
                highLighter.HighLightTarget(viewObj, highLightColor);
            }
        }
        protected override void OnActive()
        {
            base.OnActive();
        }
        protected override void OnInActive()
        {
            base.OnInActive();
            highLighter.UnHighLightTarget(viewObj);
        }
    }

}
