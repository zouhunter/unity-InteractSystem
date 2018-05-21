using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem.Attributes
{
    public class DefultStringAttribute : UnityEngine.PropertyAttribute
    {
        public string text;
        public DefultStringAttribute(string text)
        {
            this.text = text;
        }
    }

}
