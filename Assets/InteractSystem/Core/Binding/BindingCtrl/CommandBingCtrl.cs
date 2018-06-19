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
            var allBindings = new List<CommandBinding>();
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

            if(Config.Instence.commandBindings != null && Config.Instence.commandBindings.Count > 0)
            {
                foreach (var item in Config.Instence.commandBindings)
                {
                    if (item != null && !allBindings.Contains(item))
                    {
                        allBindings.Add(item);
                    }
                }
            }

            this.bindings = CreateInstenceBindings(allBindings.ToArray());
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