using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
namespace WorldActionSystem
{
    public class ActionGroupObj : ScriptableObject
    {
        public string groupKey;
        public int totalCommand;
        [UnityEngine.Serialization.FormerlySerializedAs("prefabList")]
        public List<AutoPrefabItem> autoLoadElement = new List<AutoPrefabItem>();
        public List<RunTimePrefabItem> runTimeElements = new List<RunTimePrefabItem>();
    }
}
