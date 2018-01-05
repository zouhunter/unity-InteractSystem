using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem.Binding
{
    public class TransformSyncRegister : ActionObjEventRegister
    {
        public Transform body;
        private bool active;
        private const string SyncStartKey = "TransformSyncStart";
        private const string SyncStopKey = "TransformSyncStop";
        private Transform target;
        private void Awake()
        {
            eventCtrl.AddDelegate<TransformSyncBody>(SyncStartKey, StartRotate);
            eventCtrl.AddDelegate<string>(SyncStopKey, StopRotate);
        }
        private void Update()
        {
            if(active && body && target)
            {
                body.transform.SetPositionAndRotation(target.position, target.rotation);
                body.transform.localScale = target.localScale;
            }
        }

        private void StopRotate(string arg0)
        {
            if(key == arg0)
            {
                active = false;
            }
        }

        private void StartRotate(TransformSyncBody arg0)
        {
            if(arg0.key == key)
            {
                active = true;
                target = arg0.target;
            }
        }
    }
}