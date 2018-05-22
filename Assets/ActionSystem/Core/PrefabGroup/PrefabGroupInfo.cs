using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    [System.Serializable]
    public class PrefabGroupInfo
    {
        public string prefabName;
        public string instenceName;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 size;
    }

}