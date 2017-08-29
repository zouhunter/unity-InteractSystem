using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public abstract class InsertBehaiver : MonoBehaviour
    {
        private ActionHolder _target;
        protected ActionHolder target
        {
            get
            {
                if (_target == null)
                {
                    _target = GetComponent<ActionHolder>();
                }
                return _target;
            }
        }
    }

}
