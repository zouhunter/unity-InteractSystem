using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    [System.Serializable]
    public class Coordinate
    {
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
        public Vector3 localScale;

        private string _stringValue;
        public string StringValue
        {
            get
            {
                if (_stringValue == null)
                {
                    _stringValue = localPosition.ToString() + localEulerAngles.ToString() + localScale.ToString();
                }
                return _stringValue;
            }
        }
    }

}
