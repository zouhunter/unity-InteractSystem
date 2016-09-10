using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
public enum StapType
{
    Install,
    Animation,
}
public interface IActionStap
{
    string StapName { get; } 
}
[System.Serializable]
public class StapInfo: IActionStap
{
    [SerializeField]
    private string stapName;
    public string StapName { get { return stapName; } }
    public StapType stapType;
    public string tipInfo;
}
