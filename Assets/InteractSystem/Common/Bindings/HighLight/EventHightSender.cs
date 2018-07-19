using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem;
using InteractSystem.Graph;

namespace InteractSystem.Binding
{
    public class EventHightSender : OperaterBinding
    {
        [SerializeField]
        private string key;
        private bool noticeAuto { get { return Config.Instence.highLightNotice; } }
        private string highLight { get { return "HighLightObjects"; } }
        private string unhighLight { get { return "UnHighLightObjects"; } }
        private EventController eventCtrl;
        
        public override void OnStartExecuteInternal(OperaterNode node, bool auto)
        {
            if (eventCtrl == null)
                eventCtrl = node.Command.Context.GetComponentInParent<ActionGroup>().EventCtrl;

            base.OnStartExecuteInternal(node, auto);
            if (noticeAuto){
                SetElementState(true);
            }
        }

        public override void OnBeforeEnd(OperaterNode node, bool force)
        {
            if (eventCtrl == null)
                eventCtrl = node.Command.Context.GetComponentInParent<ActionGroup>().EventCtrl;

            base.OnBeforeEnd(node, force);
            if (noticeAuto)
            {
                SetElementState(false);
            }
        }
        public override void OnUnDoExecuteInternal(OperaterNode node)
        {
            if (eventCtrl == null)
                eventCtrl = node.Command.Context.GetComponentInParent<ActionGroup>().EventCtrl;

            base.OnUnDoExecuteInternal(node);
            if (noticeAuto)
            {
                SetElementState(false);
            }
        }
        protected void SetElementState(bool open)
        {
            if (eventCtrl == null) return;
            if (!noticeAuto) return;
            if (open)
            {
                eventCtrl.NotifyObserver<string>(highLight, key);
            }
            else
            {
                eventCtrl.NotifyObserver<string>(unhighLight, key);
            }
        }

    }

}