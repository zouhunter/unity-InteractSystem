using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using WorldActionSystem;
using System;

public class DragItem : PickUpAbleItem
{
    public override void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
