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
        public float autoCompleteTime = 2;
        private Coroutine waitCoroutine;
        public GameObject ViewObj { get { return viewObj; } }
        protected override void Start()
        {
            base.Start();
            gameObject.layer = Setting.clickItemLayer;
        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            if (auto){
                if (waitCoroutine == null)
                {
                    StartCoroutine(WaitClose());
                }
            }
        }
        IEnumerator WaitClose()
        {
            yield return new WaitForSeconds(autoCompleteTime);
            TryEndExecute();
        }
        public override void OnEndExecute()
        {
            base.OnEndExecute();
            if (auto && waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
                waitCoroutine = null;
            }
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (auto && waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
                waitCoroutine = null;
            }
        }
    }

}