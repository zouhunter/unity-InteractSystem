using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem
{
    [System.Serializable]
    public class Coordinate
    {
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 localScale;

        private string _stringValue;
        public string StringValue
        {
            get
            {
                if (_stringValue == null)
                {
                    _stringValue = position.ToString() + eulerAngles.ToString() + localScale.ToString();
                }
                return _stringValue;
            }
        }
    }

}
