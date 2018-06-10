using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Binding
{

    public class CommandBingCtrl
    {
        public CommandBinding[] bindings;

        public CommandBingCtrl(CommandBinding[] bindings)
        {
            if(bindings != null && bindings.Length > 0)
            {
                this.bindings = CreateInstenceBindings(bindings);
            }
        }

        private static CommandBinding[] CreateInstenceBindings(CommandBinding[] bindings)
        {
            var worps = new CommandBinding[bindings.Length];
            for (int i = 0; i < bindings.Length; i++)
            {
                worps[i] = bindings[i];
            }
            return worps;
        }
        public void OnBeforeActionsStart(ActionCommand command) {
            if (bindings == null) return;
            foreach (var item in bindings) {
                item.OnBeforeActionsStart(command);
            }
        }
        public void OnBeforeActionsUnDo(ActionCommand command) {
            if (bindings == null) return;
            foreach (var item in bindings)
            {
                item.OnBeforeActionsUnDo(command);
            }
        }
        public void OnBeforeActionsPlayEnd(ActionCommand command) {
            if (bindings == null) return;
            foreach (var item in bindings)
            {
                item.OnBeforeActionsPlayEnd(command);
            }
        }
    }

}