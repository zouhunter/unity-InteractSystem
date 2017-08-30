using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    /// <summary>
    /// 步骤结束事件
    /// </summary>
    /// <param name="stepName"></param>
    public delegate void StepComplete(string stepName);
}