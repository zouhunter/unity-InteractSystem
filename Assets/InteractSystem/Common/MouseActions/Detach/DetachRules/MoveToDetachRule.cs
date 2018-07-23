using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    public class MoveToDetachRule : DetachRule
    {
        [SerializeField]
        private Vector3 targetPos;
        [SerializeField]
        private float time = 1;
        [SerializeField]
        private AnimationCurve curve;
        private Transform detachItem;
        public override void OnDetach(DetachItem target)
        {
            detachItem = target.transform;
            CoroutineController.Instence.StartCoroutine(DelayMoveTo());
        }

     
        public override void UnDoDetach()
        {
            CoroutineController.Instence.StopCoroutine(DelayMoveTo());
        }

        private IEnumerator DelayMoveTo()
        {
            var startPos = detachItem.transform.position;

            for (float i = 0; i < time; i+= Time.deltaTime)
            {
                yield return null;
                var frame = curve.Evaluate(i / time);
                detachItem.transform.position = Vector3.Lerp(startPos, targetPos, frame);
            }
        }

    }
}