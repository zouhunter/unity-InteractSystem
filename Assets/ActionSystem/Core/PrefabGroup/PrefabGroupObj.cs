using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class PrefabGroupObj : ScriptableObject
    {
        public string discription;
        public List<GameObject> prefabs = new List<GameObject>();
        public List<PrefabGroupInfo> prefabInfos = new List<PrefabGroupInfo>();
    }
}
