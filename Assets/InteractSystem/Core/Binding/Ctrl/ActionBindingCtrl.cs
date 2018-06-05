using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Binding
{

    public class ActionBindingCtrl
    { 
        public ActionBinding[] bindings;

        public ActionBindingCtrl(ActionBinding[] bindings)
        {
            if (bindings != null && bindings.Length > 0)
            {
                this.bindings = CreateInstenceBindings(bindings);
            }
        }

        private static ActionBinding[] CreateInstenceBindings(ActionBinding[] bindings)
        {
            var worps = new ActionBinding[bindings.Length];
            for (int i = 0; i < bindings.Length; i++)
            {
                worps[i] = bindings[i];
            }
            return worps;
        }
        public void OnBeforeActionsStart(Graph.OperateNode node,bool auto)
        {
            if (bindings == null) return;
            foreach (var item in bindings)
            {
                item.OnStartExecuteInternal(node,auto);
            }
        }
        public void OnBeforeActionsUnDo(Graph.OperateNode node)
        {
            if (bindings == null) return;
            foreach (var item in bindings)
            {
                item.OnUnDoExecuteInternal(node);
            }
        }
        public void OnBeforeActionsPlayEnd(Graph.OperateNode node,bool force)
        {
            if (bindings == null) return;
            foreach (var item in bindings)
            {
                item.OnBeforeEnd(node, force);
            }
        }
    }

}