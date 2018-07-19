using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem.Attributes
{
    public class CustomFieldAttribute : UnityEngine.PropertyAttribute
    {
        public string title;
        public CustomFieldAttribute(string title)
        {
            this.title = title;
        }
    }
}
