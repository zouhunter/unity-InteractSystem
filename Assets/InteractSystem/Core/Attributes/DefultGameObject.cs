using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class DefultGameObjectAttribute : PropertyAttribute
    {
        public string title;
        public DefultGameObjectAttribute(string title = null)
        {
            this.title = title;
        }
    }
}

