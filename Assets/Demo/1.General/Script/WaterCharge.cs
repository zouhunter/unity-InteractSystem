using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;
using System;

public class WaterCharge : MonoBehaviour {
    private float currentValue;

    public void OnCharge(ChargeData arg0)
    {
        currentValue += arg0.value;
        transform.localScale = new Vector3(transform.localScale.x, currentValue, transform.localScale.z);
    }
}
