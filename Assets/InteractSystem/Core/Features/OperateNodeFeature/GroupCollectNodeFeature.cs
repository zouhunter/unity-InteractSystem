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

        protected override void UnDoActivedElement()
        {
            ForEachElement((element)=> {
                UndoElement(element);
            });
        }

        protected override void InActivedElements()
        {
            ForEachElement((element) => {
                SetInActiveElement(element);
            });
        }

    }
}
