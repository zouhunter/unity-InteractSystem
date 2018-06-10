using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Binding
{

    public class OpreaterBindingCtrl
    { 
        public OperaterBinding[] bindings;

        public OpreaterBindingCtrl(OperaterBinding[] bindings)
        {
            if (bindings != null && bindings.Length > 0)
            {
                this.bindings = CreateInstenceBindings(bindings);
            }
        }

        private static OperaterBinding[] CreateInstenceBindings(OperaterBinding[] bindings)
        {
            var worps = new OperaterBinding[bindings.Length];
            for (int i = 0; i < bindings.Length; i++)
            {
                worps[i] = bindings[i];
            }
            return worps;
        }
        public void OnBeforeActionsStart(Graph.OperaterNode node,bool auto)
        {
            if (bindings == null) return;
            foreach (var item in bindings)
            {
                item.OnStartExecuteInternal(node,auto);
            }
        }
        public void OnBeforeActionsUnDo(Graph.OperaterNode node)
        {
            if (bindings == null) return;
            foreach (var item in bindings)
            {
                item.OnUnDoExecuteInternal(node);
            }
        }
        public void OnBeforeActionsPlayEnd(Graph.OperaterNode node,bool force)
        {
            if (bindings == null) return;
            foreach (var item in bindings)
            {
                item.OnBeforeEnd(node, force);
            }
        }
    }

}