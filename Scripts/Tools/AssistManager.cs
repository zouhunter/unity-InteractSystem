using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class AssistManager<T> :MonoBehaviour where T : MonoBehaviour
{
    private static T instance = default(T);
    private static object lockHelper = new object();
    public static bool mManualReset = false;

    protected AssistManager() { }
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockHelper)
                {
                    if (instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).ToString());
                        instance = go.AddComponent<T>();
                    }
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        instance = GetComponent<T>();
    }
    protected void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
};

