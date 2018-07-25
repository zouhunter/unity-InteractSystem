using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem.Actions;
using System;
using System.Linq;

namespace InteractSystem
{
    [System.Serializable]
    public class GroupCollectNodeFeature : CollectNodeFeature
    {
        public GroupCollectNodeFeature(Type type) : base(type) { }
        protected override void CompleteElements(bool undo)
        {
            foreach (var element in itemList)
            {
                var objs = elementPool.FindAll(x => x.Name == element);
                if (objs == null) return;
                for (int i = 0; i < objs.Count; i++)
                {
                    var currentObj = objs[i];
                    if (undo)
                    {
                        UndoElement(currentObj);
                    }
                    else
                    {
                        SetInActiveElement(currentObj);
                    }
                }
            }

        }
    }
}
