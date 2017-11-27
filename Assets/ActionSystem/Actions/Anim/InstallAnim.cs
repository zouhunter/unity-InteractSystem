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
        [SerializeField]
        private Transform targetTrans;
        private Vector3 targetPos;
        [SerializeField]
        private float time = 2f;
        private Vector3 initPos;
#if !NoFunction
        public Tweener tween;
#endif
        private void Awake()
        {
            initPos = transform.position;
            if(targetTrans != null){
                targetPos = targetTrans.transform.position;
            }
        }

        void Init(UnityAction onAutoPlayEnd)
        {
#if !NoFunction
            transform.position = initPos;
            tween = transform.DOMove(targetPos, time).OnComplete(() =>
            {
                onAutoPlayEnd.Invoke();
                transform.position = targetPos;
            }).SetAutoKill(false).Pause();
#endif
        }

        public void Play(float speed, UnityAction onAutoPlayEnd)
        {
            Init(onAutoPlayEnd);
#if !NoFunction
            tween.ChangeValues(transform.position, targetPos,1f/ speed).Play();
#endif
        }

        public void EndPlay()
        {
#if !NoFunction
            tween.Complete();
            transform.position = targetPos;
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
