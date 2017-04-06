using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MidlePosRot
{
    public List<Vector3> pos;
    public List<Rot> rot;

    [System.Serializable]
    public class Rot
    {
        public int id;
        public Vector3 rot;
    }
}
