using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System;

namespace WorldActionSystem
{
    public class AnimCtroller : ActionCtroller
    {
        public AnimCtroller(ActionCommand trigger) : base(trigger)
        {
            
        }

        public override IEnumerator Update()
        {
            yield break;
        }
    }
}