using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using InteractSystem.Actions;
using UnityEditorInternal;
using System;

namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(RopeElement))]
    public class RopeElementDrawer : Editor
    {

        [InitializeOnLoadMethod]
        static void ImportLayer()
        {
            LayerTool.ImportLayers(RopeElement.layer);
        }
    }

}