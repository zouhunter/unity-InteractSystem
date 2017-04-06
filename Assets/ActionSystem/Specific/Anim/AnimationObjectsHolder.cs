using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    /// <summary>
    /// 注册所有动画命令
    /// </summary>
    public class AnimationObjectsHolder : ActionHolder
    {
        private List<AnimObj> animObjects = new List<AnimObj>();

        void Start()
        {
            foreach (Transform item in transform)            {
                AnimObj anim = item.GetComponent<AnimObj>();
                animObjects.Add(anim);
                ActionCommand cmd = new AnimCommand(anim.stapName, anim, true);
                registFunc(cmd);
            }
        }

        public override void SetHighLight(bool on)
        {
            //throw new NotImplementedException();
        }

        public override void InsertScript<T>(bool on)
        {
            //throw new NotImplementedException();
        }
    }

}