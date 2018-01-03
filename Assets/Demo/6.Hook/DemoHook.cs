using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;
using System;

public class DemoHook : ActionHook
{
    public ParticleSystem particle;

    protected override bool autoComplete
    {
        get
        {
            return false;
        }
    }

    public override void OnStartExecute(bool auto)
    {
        base.OnStartExecute(auto);
        particle.gameObject.SetActive(true);
    }
    private void Awake()
    {
        particle.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Started && !Complete)
        {
            if (Input.GetMouseButtonDown(0))
            {
                particle.Emit(100);
            }
        }
    }
    private void OnGUI()
    {
        if (Started && !Complete)
        {
            if (GUILayout.Button("QuitHook"))
            {
                OnEndExecute(false);
            }
        }

    }
    public override void OnEndExecute(bool force)
    {
        base.OnEndExecute(force);
        particle.gameObject.SetActive(false);
    }
}
