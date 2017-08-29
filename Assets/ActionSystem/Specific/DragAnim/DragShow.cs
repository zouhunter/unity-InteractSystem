using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class DragShow : MonoBehaviour
    {
        public List<AnimObj> anims = new List<AnimObj>();
        // Use this for initialization
        void Start()
        {
            foreach (Transform item in transform)
            {
                AnimObj obj = item.GetComponent<AnimObj>();
                anims.Add(obj);
            }
        }
        internal void PlayAnim(string stapName)
        {
            var anim = anims.Find(x=>x.stapName == stapName);
            if (anim != null)
            {
                anim.PlayAnim();
            }
        }

        internal void UnDoAnim(string stapName)
        {
            var anim = anims.Find(x => x.stapName == stapName);
            if (anim != null)
            {
                anim.UnDoPlay();
            }
        }
        internal void EndPlayAnim(string stapName)
        {
            var anim = anims.Find(x => x.stapName == stapName);
            if (anim != null)
            {
                anim.EndPlay();
            }
        }
    }
}