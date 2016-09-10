using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;
public class StapConfig : MonoBehaviour {
    [Header("安装流程配制")]
    public List<StapInfo> staps;
    void OnEnable()
    {
        ActionSystem.Instance.SetActionStaps(staps.ToArray());
    }
}
