using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
#if !NoFunction
using DG.Tweening;
#endif
using System;

public class InstallAnim : MonoBehaviour
{
    public Vector3 startPos = new Vector3(5, 0, 0);
    private Vector3 initPos;
#if !NoFunction
    public Tweener tween;
#endif
    internal void Init(UnityEvent onPlayEnd)
    {
#if !NoFunction

        initPos = transform.position;
        transform.position += startPos;
        tween = transform.DOMove(initPos, 1).OnComplete(() =>
        {
            onPlayEnd.Invoke();
            transform.position = initPos;
        }).SetAutoKill(false).Pause();
#endif
    }

    internal void Play(float duration)
    {
#if !NoFunction
        tween.ChangeValues(transform.position, initPos, duration).Play();
#endif
    }

    internal void EndPlay()
    {
#if !NoFunction
        tween.Complete();
#endif
    }

    internal void UnDoPlay()
    {
#if !NoFunction
        transform.position += startPos;
        tween.Rewind();
#endif
    }
}
