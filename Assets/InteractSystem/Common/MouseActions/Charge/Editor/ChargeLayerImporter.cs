using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace InteractSystem.Actions
{
    public class ChargeLayerImporter {

        [InitializeOnLoadMethod]
        static void ImportLayer() {
            LayerTool.ImportLayers(ChargeItem.layer,ChargeResource.layer);
        }
    }
}
