using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Prefer
{
    public abstract class PreferValue<T>
    {
        protected bool updated = false;
        protected T _value;
        public T value
        {
            get
            {
                if (updated){
                    updated = false;
                    _value = GetPreferValue();
                    //Debug.Log("read:" + key);
                }
                return _value;
            }
            set
            {
                if(!Equals(_value,value)) {
                    _value = value;
                    SetPreferValue(value);
                    //Debug.Log("record:" + key);
                }
            }
        }
        public string key;
        public PreferValue(string key)
        {
            this.key = key;
            this.updated = true;
        }
        protected abstract void SetPreferValue(T value);
        protected abstract T GetPreferValue();
        protected abstract bool Equals(T a, T b);
    }

}