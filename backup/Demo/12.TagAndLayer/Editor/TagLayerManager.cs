using UnityEditor;
using UnityEngine;

    [InitializeOnLoad]
    public class TagLayerManager
    {
        public static string[] tags = new string[]
        {
        "momo"
        };

        public static string[] sortingLayers = new string[]
        {

        };

        public static string[] layers = new string[]
        {
       "Blend"
        };

        static TagLayerManager()
        {
            bool hasKey = PlayerPrefs.HasKey("WriteSettings");
            if (hasKey == false)
            {
                PlayerPrefs.SetInt("WriteSettings", 1);
                OnWrite();
            }
        }

        [MenuItem("Tools/TagTools/Write")]
        static void OnWrite()
        {
            foreach (var tag in TagLayerManager.tags)
            {
                AddTag(tag);
            }

            foreach (var sortingLayer in TagLayerManager.sortingLayers)
            {
                AddSortingLayer(sortingLayer);
            }

            foreach (var layer in TagLayerManager.layers)
            {
                AddLayer(layer);
            }
        }


        [MenuItem("Tools/TagTools/ClearTags")]
        static void ClearTags()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "tags")
                {
                    it.ClearArray();
                    tagManager.ApplyModifiedProperties();
                    return;
                }
            }
        }

        [MenuItem("Tools/TagTools/ClearSortingLayers")]
        static void ClearSortingLayers()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "m_SortingLayers")
                {
                    it.ClearArray();
                    tagManager.ApplyModifiedProperties();
                    return;
                }
            }
        }

        [MenuItem("Tools/TagTools/ClearLayers")]
        static void ClearLayers()
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
                        dataPoint.stringValue = string.Empty;
                    }
                    tagManager.ApplyModifiedProperties();
                    return;
                }
            }
        }

        [MenuItem("Tools/TagTools/ClearAll")]
        static void ClearAll()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                it.ClearArray();
                tagManager.ApplyModifiedProperties();
            }
        }

        static void ReadTag()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "tags")
                {
                    var count = it.arraySize;

                    for (int i = 0; i < count; i++)
                    {
                        var dataPoint = it.GetArrayElementAtIndex(i);
                        Debug.Log(dataPoint.stringValue);
                    }
                }
            }
        }

        static void ReadSortingLayer()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "m_SortingLayers")
                {
                    var count = it.arraySize;
                    for (int i = 0; i < count; i++)
                    {
                        var dataPoint = it.GetArrayElementAtIndex(i);
                        while (dataPoint.NextVisible(true))
                        {
                            if (dataPoint.name == "name")
                            {
                                //Debug.Log(dataPoint.stringValue);
                            }
                        }
                    }


                }
            }
        }

        static void ReadLayer()
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
                        Debug.Log(dataPoint.stringValue);
                    }
                }
            }
        }


        static void AddTag(string tag)
        {
            if (!isHasTag(tag))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name == "tags")
                    {
                        it.InsertArrayElementAtIndex(it.arraySize);
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(it.arraySize - 1);
                        dataPoint.stringValue = tag;
                        tagManager.ApplyModifiedProperties();
                        return;
                    }
                }
            }
        }

        static void AddSortingLayer(string sortingLayer)
        {
            if (!isHasSortingLayer(sortingLayer))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name == "m_SortingLayers")
                    {
                        Debug.Log("SortingLayers" + it.arraySize);
                        it.InsertArrayElementAtIndex(it.arraySize);
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(it.arraySize - 1);
                        while (dataPoint.NextVisible(true))
                        {
                            if (dataPoint.name == "name")
                            {
                                dataPoint.stringValue = sortingLayer;
                                tagManager.ApplyModifiedProperties();
                                return;
                            }
                        }
                    }
                }
            }
        }

        static void AddLayer(string layer)
        {
            if (!isHasLayer(layer))
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
                                dataPoint.stringValue = layer;
                                tagManager.ApplyModifiedProperties();
                                return;
                            }
                        }
                    }
                }
            }
        }

        static bool isHasTag(string tag)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.tags[i].Contains(tag))
                    return true;
            }
            return false;
        }

        static bool isHasSortingLayer(string sortingLayer)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "m_SortingLayers")
                {
                    for (int i = 0; i < it.arraySize; i++)
                    {
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                        while (dataPoint.NextVisible(true))
                        {
                            if (dataPoint.name == "name")
                            {
                                if (dataPoint.stringValue == sortingLayer) return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        static bool isHasLayer(string layer)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.layers[i].Contains(layer))
                    return true;
            }
            return false;
        }
    }
