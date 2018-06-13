using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem.Common.Actions;
using InteractSystem;
using System;

public class RandomAppear : AutoAppearRule
{
    public Vector3 center;
    public float range;
    public override void OnCreate(ISupportElement element)
    {
        var pos = center + UnityEngine.Random.insideUnitSphere * range;
        element.Body.transform.position = pos;
    }
}
