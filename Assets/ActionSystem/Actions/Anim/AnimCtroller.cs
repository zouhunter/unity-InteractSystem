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

namespace WorldActionSystem
{
    public class AnimCtroller : ActionCtroller
    {
        public AnimCtroller(ActionCommand trigger) : base(trigger)
        {
            //foreach (AnimObj anim in trigger.ActionObjs)
            //{
            //    anim.RegistAutoEndPlayEvent(OnEndPlayAnim);
            //}
        }

        //private void OnEndPlayAnim(int id)
        //{
        //    if (CurrentStepComplete())
        //    {
        //       trigger.Complete();
        //    }
        //    else
        //    {
        //        Debug.Log("wait");
        //    }
        //}
        //protected bool CurrentStepComplete()
        //{
        //    bool complete = true;
        //    foreach (var item in actionObjs)
        //    {
        //        complete &= item.Complete;
        //    }
        //    return complete;
        //}
    }
}