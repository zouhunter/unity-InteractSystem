using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class BtnObj : MonoBehaviour
    {
        public string stapName;
        public UnityEvent onClicked;
        public UnityEvent onUnClicked;
        public Renderer render;
        public int queueID;
        public bool clickAble { get; set; }
        public bool Clicked { get { return _clicked; } }
        private bool _clicked;
        private void Start()
        {
            if(render ==null) {
                render = GetComponent<Renderer>();
            }
            gameObject.layer = LayerMask.NameToLayer(Setting.clickItemLayer);
        }
        public void SetClicked()
        {
            _clicked = true;
            clickAble = false;
            onClicked.Invoke();
        }

        public void SetUnClicked()
        {
            _clicked = false;
            onUnClicked.Invoke();
        }
    }

}