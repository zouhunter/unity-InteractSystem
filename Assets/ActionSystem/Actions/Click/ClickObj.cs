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
        public int queueID;

        protected override void Start()
        {
            base.Start();
            if(render ==null) {
                render = GetComponent<Renderer>();
            }
            gameObject.layer = Setting.clickItemLayer;
        }
    }

}