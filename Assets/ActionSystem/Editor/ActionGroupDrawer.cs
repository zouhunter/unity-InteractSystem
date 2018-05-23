using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace WorldActionSystem
{

    [CustomEditor(typeof(ActionGroup)), CanEditMultipleObjects]
    public class ActionGroupDrawer : ActionGroupDrawerBase
    {
        protected override List<AutoPrefabItem> GetAutoPrefabs()
        {
            var group = target as ActionGroup;
            return group.autoLoadElement;
        }
        protected override List<RunTimePrefabItem> GetRunTimePrefabs()
        {
            var group = target as ActionGroup;
            return group.runTimeElements;
        }

    }
}