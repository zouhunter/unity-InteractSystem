using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityEditorInternal;
namespace WorldActionSystem
{
    public class ActionEditorUtility 
    {
        public static void LoadmatrixInfo(SerializedProperty matrixProp, Transform transform)
        {
            var materix = Matrix4x4.identity;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    materix[i, j] = matrixProp.FindPropertyRelative("e" + i + "" + j).floatValue;
                }
            }
            transform.localPosition = materix.GetColumn(0);
            transform.localEulerAngles = materix.GetColumn(1);
            transform.localScale = materix.GetColumn(2);
        }
        public static void SaveMatrixInfo(SerializedProperty matrixProp, Transform transform)
        {
            var materix = Matrix4x4.identity;
            materix.SetColumn(0, transform.localPosition);
            materix.SetColumn(1, transform.localEulerAngles);
            materix.SetColumn(2, transform.localScale);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrixProp.FindPropertyRelative("e" + i + "" + j).floatValue = materix[i, j];
                }
            }
        }
        public static void ApplyPrefab(GameObject gitem)
        {
            var prefab = PrefabUtility.GetPrefabParent(gitem);
            if (prefab != null)
            {
                var root = PrefabUtility.FindPrefabRoot((GameObject)prefab);
                if (root != null)
                {
                    if (root.name == gitem.name)
                    {
                        //PrefabUtility.RecordPrefabInstancePropertyModifications(gitem);
                        var modify = PrefabUtility.GetPropertyModifications(gitem);
                        Debug.Log("GetPropertyModifications:" + modify.Length);
                        foreach (var item in modify)
                        {
                            Debug.Log(item.propertyPath);
                        }
                        modify = PrefabUtility.GetPropertyModifications(root);
                        Debug.Log("GetPropertyModifications:" + modify.Length);
                        foreach (var item in modify)
                        {
                            Debug.Log(item.propertyPath);
                        }
                        PrefabUtility.SetPropertyModifications(root, modify);
                        return;
                        PrefabUtility.ReplacePrefab(gitem as GameObject, root, ReplacePrefabOptions.ConnectToPrefab);
                    }
                }
            }
        }

        internal static void LoadPrefab(SerializedProperty prefabProp,SerializedProperty instanceIDProp, SerializedProperty reparentProp,SerializedProperty parentProp,SerializedProperty rematrixProp,SerializedProperty matrixProp)
        {
            if (prefabProp.objectReferenceValue == null)
            {
                return;
            }

            if (instanceIDProp.intValue != 0)
            {
                var gitem = EditorUtility.InstanceIDToObject(instanceIDProp.intValue);

                if (gitem != null)
                {
                    return;
                }
            }

            GameObject gopfb = prefabProp.objectReferenceValue as GameObject;
            if (gopfb != null)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(gopfb) as GameObject;

                instanceIDProp.intValue = go.GetInstanceID();

                var actionSystem = GameObject.FindObjectOfType<ActionSystem>();

                if (reparentProp.boolValue)
                {
                    var parent = parentProp.objectReferenceValue;
                    if (parent != null)
                    {
                        go.transform.SetParent(parent as Transform, false);
                    }
                    else
                    {
                        Debug.LogWarning("坐标对象为空" + go.name);
                    }
                }
                else
                {
                    var parent = actionSystem == null ? null : actionSystem.transform;
                    go.transform.SetParent(parent, false);
                }

                if (rematrixProp.boolValue)
                {
                    LoadmatrixInfo(matrixProp, go.transform);
                }
            }


        }

        internal static void SavePrefab( SerializedProperty instanceIDProp,SerializedProperty rematrixProp, SerializedProperty matrixProp)
        {
            var gitem = EditorUtility.InstanceIDToObject(instanceIDProp.intValue);
            if (gitem != null)
            {
                if (rematrixProp.boolValue)
                {
                    ActionEditorUtility.SaveMatrixInfo(matrixProp, (gitem as GameObject).transform);
                }
                ActionEditorUtility.ApplyPrefab(gitem as GameObject);
                GameObject.DestroyImmediate(gitem);
            }
            instanceIDProp.intValue = 0;
        }

        internal static void InsertItem(SerializedProperty prefabProp, UnityEngine.Object obj)
        {
            var prefab = PrefabUtility.GetPrefabParent(obj);
            if (prefab != null)
            {
                prefabProp.objectReferenceValue = PrefabUtility.FindPrefabRoot(prefab as GameObject);
            }
            else
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path))
                {
                    prefabProp.objectReferenceValue = obj;
                }
            }
        }
    }
}