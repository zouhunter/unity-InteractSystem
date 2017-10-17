using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
#if !NoFunction
using DG.Tweening;
#endif
using System;
namespace WorldActionSystem
{

    public class InstallAnim : MonoBehaviour, AnimPlayer
    {
        public Vector3 startPos = new Vector3(5, 0, 0);
        private Vector3 initPos;
#if !NoFunction
        public Tweener tween;
#endif
        private void Awake()
        {
            initPos = transform.position;
        }

        void Init(UnityAction onAutoPlayEnd)
        {
#if !NoFunction
            transform.position = initPos + startPos;
            tween = transform.DOMove(initPos, 1).OnComplete(() =>
            {
                onAutoPlayEnd.Invoke();
                transform.position = initPos;
            }).SetAutoKill(false).Pause();
#endif
        }

        public void Play(float speed, UnityAction onAutoPlayEnd)
        {
            Init(onAutoPlayEnd);
#if !NoFunction
            tween.ChangeValues(transform.position, initPos,1f/ speed).Play();
#endif
        }

        public void EndPlay()
        {
#if !NoFunction
            tween.Complete();
            transform.position = initPos;
#endif
        }

        public void UnDoPlay()
        {
#if !NoFunction
            Debug.Log("UnDoPlay");
            tween.Rewind();
            transform.position = initPos;
#endif
        }
    }
}
