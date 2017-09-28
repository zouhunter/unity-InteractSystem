using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
namespace WorldActionSystem
{
    public class ActionSystemObj : ScriptableObject
    {
        public List<ActionPrefabItem> prefabList = new List<ActionPrefabItem>();
    }

}
