using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using InteractSystem.Actions;
using UnityEditorInternal;
using System;

namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(DetachItem))]
    public class DetachItemDrawer : Editor
    {

        [InitializeOnLoadMethod]
        static void ImportLayer()
        {
            LayerTool.ImportLayers(DetachItem.layer);
        }
    }

}