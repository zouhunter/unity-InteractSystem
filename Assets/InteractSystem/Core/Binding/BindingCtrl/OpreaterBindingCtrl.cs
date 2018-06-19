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
            var allBindings = new List<Binding.OperaterBinding>();
            if (bindings != null && bindings.Length > 0)
            {
                foreach (var item in bindings)
                {
                    if (item != null && !allBindings.Contains(item))
                    {
                        allBindings.Add(item);
                    }
                }
            }

            if (Config.Instence.operateBindings != null && Config.Instence.operateBindings.Count > 0)
            {
                foreach (var item in Config.Instence.operateBindings)
                {
                    if (item != null && !allBindings.Contains(item))
                    {
                        allBindings.Add(item);
                    }
                }
            }
            this.bindings = CreateInstenceBindings(allBindings.ToArray());
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