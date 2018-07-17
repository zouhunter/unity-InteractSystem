using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InteractSystem
{
    public class PathTweener
    {
        private MonoBehaviour holder;
        private Vector3[] positons;
        private float animTime;
        private UnityAction onComplete;
        private Transform target;
        private Coroutine coroutine;
        private UnityAction<int> onwayPointChanged { get; set; }
        public PathTweener(MonoBehaviour holder)
        {
            this.holder = holder;
        }

        internal void DOPath(Transform transform, Vector3[] vector3, int animTime, UnityAction onComplete)
        {
            this.positons = vector3;
            this.animTime = animTime;
            this.onComplete = onComplete;
            this.target = transform;
            coroutine = holder.StartCoroutine(MoveCore());
        }

        IEnumerator MoveCore()
        {
            float d_time = animTime / (positons.Length - 1);
            for (int i = 0; i < positons.Length - 1; i++)
            {
                if (onwayPointChanged != null) onwayPointChanged(i);
                var startPos = positons[i];
                var targetPos = positons[i + 1];
                for (float j = 0; j < d_time; j += Time.deltaTime)
                {
                    target.position = Vector3.Lerp(startPos, targetPos, j);
                    yield return null;
                }
            }
            if (onwayPointChanged != null) onwayPointChanged(positons.Length - 1);
            if (onComplete != null) onComplete();
        }

        internal void Kill()
        {
            if (coroutine != null)
            {
                holder.StopCoroutine(coroutine);
            }
        }

        internal void OnWaypointChange(UnityAction<int> p)
        {
            onwayPointChanged = p;
        }
    }
}