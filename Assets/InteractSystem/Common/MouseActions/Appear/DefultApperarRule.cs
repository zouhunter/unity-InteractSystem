using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    public class DefultApperarRule : AutoAppearRule
    {
        public Coordinate coordinate;

        public override void OnCreate(ISupportElement element)
        {
            element.Body.transform.localPosition = coordinate.position;
            element.Body.transform.localEulerAngles = coordinate.eulerAngles;
            element.Body.transform.localScale = coordinate.localScale;
        }
    }
}