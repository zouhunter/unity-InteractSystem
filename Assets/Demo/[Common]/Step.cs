using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Step : IActionStap
{
    public string step;
    public string StapName
    {
        get
        {
            return step;
        }
    }

}