using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(Actions.ChargeItem))]
    public class ChargeItemDrawer : ActionItemDrawer{

        [InitializeOnLoadMethod]
        static void ImportLayer() {
            LayerTool.ImportLayers(Actions.ChargeItem.layer, Actions.ChargeResource.layer);
        }
    }
}
