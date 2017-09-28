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
        [SerializeField]
        private int queueID;
        public int QueueID
        {
            get
            {
                return queueID;
            }
        }
        public Renderer render;
        protected override void Start()
        {
            base.Start();
            if(render ==null) {
                render = GetComponentInChildren<Renderer>();
            }
            gameObject.layer = Setting.clickItemLayer;
        }
    }

}