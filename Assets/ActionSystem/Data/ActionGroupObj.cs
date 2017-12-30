using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
namespace WorldActionSystem
{
    public class ActionGroupObj : ScriptableObject
    {
        public List<ActionPrefabItem> prefabList = new List<ActionPrefabItem>();
    }

}
