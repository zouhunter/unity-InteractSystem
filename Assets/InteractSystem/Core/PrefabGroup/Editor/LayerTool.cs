using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;
namespace InteractSystem
{
    public class LayerTool
    {
        [InitializeOnLoadMethod]
        static void AutoAddLayers()
        {
            var fields = typeof(Layers).GetFields(System.Reflection.BindingFlags.GetField|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public);
            var layerNames = fields.Select(x => x.GetValue(null) as string).ToArray();
            ImportLayers(layerNames);
        }

        public static void ImportLayers(params string[] layers)
        {
            foreach (var layerName in layers)
            {
                if (Array.Find(InternalEditorUtility.layers, x => x == layerName) == null)
                {
                    SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                    SerializedProperty it = tagManager.GetIterator();
                    while (it.NextVisible(true))
                    {
                        if (it.name == "layers")
                        {
                            for (int i = 0; i < it.arraySize; i++)
                            {
                                if (i == 3 || i == 6 || i == 7) continue;
                                SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                                if (string.IsNullOrEmpty(dataPoint.stringValue))
                                {
                                    dataPoint.stringValue = layerName;
                                    tagManager.ApplyModifiedProperties();
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}