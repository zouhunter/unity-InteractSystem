using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem.Binding
{
    public class ActionItemHighlighter : ActionItemBinding
    {
        [SerializeField]
        private Color highLightColor = Color.green;
        protected IHighLightItems highLighter = new ShaderHighLight();
        protected bool notice { get { return Config.Instence.highLightNotice; } }
        protected bool actived;
        protected List<GameObject> viewObjects = new List<GameObject>();

        public override void Update()
        {
            base.Update();
            foreach (var item in viewObjects)
            {
                highLighter.HighLightTarget(item, highLightColor);
            }
        }

        public override void OnActive(ActionItem viewObj)
        {
            if (viewObj != null && !viewObjects.Contains(viewObj.gameObject))
            {
                viewObjects.Add(viewObj.gameObject);
            }
        }
        public override void OnInActive(ActionItem viewObj)
        {
            if (viewObj != null && viewObjects.Contains(viewObj.gameObject))
            {
                viewObjects.Remove(viewObj.gameObject);
                highLighter.UnHighLightTarget(viewObj.gameObject);
            }
            else
            {
               if(log) Debug.LogWarning("viewObj:" + viewObj + "can not inactive!");
            }
        }
    }

}
