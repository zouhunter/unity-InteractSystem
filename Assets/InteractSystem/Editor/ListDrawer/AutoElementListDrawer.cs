using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

namespace InteractSystem.Drawer
{
    public class AutoElementListDrawer : PrefabElementListDrawer
    {
        protected override void OnAddItem(SerializedProperty prop, UnityEngine.Object obj)
        {
            base.OnAddItem(prop, obj);
            var coordinate = prop.FindPropertyRelative("coordinate");
            ActionEditorUtility.SaveCoordinatesInfo(coordinate, (obj as GameObject).transform);
        }
    }
}