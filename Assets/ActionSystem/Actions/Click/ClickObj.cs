using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class ClickObj : ActionObj
    {
        public Renderer render;
        protected override void Start()
        {
            base.Start();
            if(render ==null) {
                render = GetComponentInChildren<Renderer>();
            }
            gameObject.layer = Setting.clickItemLayer;
        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            if (auto)
            {
                Invoke("OnEndExecute", 0.5f);
            }
        }
    }

}