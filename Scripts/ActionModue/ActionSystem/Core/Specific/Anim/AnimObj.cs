using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
		
/// <summary>
/// 动画对象，播放动画，停止动画
/// </summary>
public class AnimObj : ActionObj
{
    public string stapName;
    public string animName;
    public bool endActive;

    private Animation anim;
    private AnimationClip clip;
    private AnimationState state;
    private float animTime;
    private AnimationEvent even;

    void Start()
    {
        anim = GetComponent<Animation>();
        state = anim[animName];
        animTime = state.length;
        clip = anim.GetClip(animName);
        even = new AnimationEvent();
        even.time = animTime;
        even.functionName = "OnPlayToEnd";
        clip.AddEvent(even);
    }
    /// <summary>
    /// 播放动画
    /// </summary>
    public void PlayAnim()
    {
        state.normalizedTime = 0f;
        state.normalizedSpeed = 1;

        anim.Play();
    }

    /// <summary>
    /// 强制完成
    /// </summary>
    public void EndPlay()
    {
        state.normalizedTime = 1f;
        state.normalizedSpeed = 0;
    }
    public void OnPlayToEnd()
    {
        RemoteCtrl.EndExecuteCommand();
    }
}

	}