using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Enviroment
{
    public bool originalState;//初始状态
    public bool startState;//命令开始时的状态
    public bool completeState;//命令结束时的状态
    public Matrix4x4 matrix;//坐标参数
    public EnviromentObj enviromentObj;//共用的环境控制对象
    public bool ignore;

    public void SetOriginal()
    {

    }

    public void SetStart()
    {

    }

    public void SetComplete()
    {

    }
}

public class EnviromentObj : ScriptableObject,System.IComparable<EnviromentObj>
{
#if UNITY_EDITOR
    [HideInInspector]
    public int instanceID;
#endif
    /// <summary>
    /// 运行时的实例
    /// </summary>
    [HideInInspector]
    public GameObject instence;
    public GameObject prefab;

    public int CompareTo(EnviromentObj other)
    {
        if (prefab == null || other.prefab == null) return 0;
        return string.Compare(prefab.name, other.prefab.name);
    }
}