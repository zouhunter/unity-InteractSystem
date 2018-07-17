using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class DefultColliderAttribute : PropertyAttribute
    {
        public string title;
        public DefultColliderAttribute(string title = null)
        {
            this.title = title;
        }
    }
}

