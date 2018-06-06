using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

namespace InteractSystem
{
    public static class ActionEditorUtility
    {
        //记录坐标加载时,不需要记录下列信息变化
        public static List<string> coondinatePaths = new List<string>
        {
            "m_LocalPosition.x",
            "m_LocalPosition.y",
            "m_LocalPosition.z",
            "m_LocalRotation.x",
            "m_LocalRotation.y",
            "m_LocalRotation.z",
            "m_LocalRotation.w",
            "m_LocalScale.x",
            "m_LocalScale.y",
            "m_LocalScale.z",
            "m_RootOrder"
        };
        public static void LoadCoordinatePropInfo(SerializedProperty coordinateProp, Transform transform)
        {
            transform.localPosition = coordinateProp.FindPropertyRelative("localPosition").vector3Value;
            transform.localEulerAngles = coordinateProp.FindPropertyRelative("localEulerAngles").vector3Value;
            transform.localScale = coordinateProp.FindPropertyRelative("localScale").vector3Value;
        }
        public static void SaveCoordinatesInfo(SerializedProperty coordinate, Transform transform)
        {
            coordinate.FindPropertyRelative("localPosition").vector3Value = RecordClamp(transform.localPosition);
            coordinate.FindPropertyRelative("localEulerAngles").vector3Value = RecordClamp(transform.localEulerAngles);
            coordinate.FindPropertyRelative("localScale").vector3Value = RecordClamp(transform.localScale);
        }

        private static Vector3 RecordClamp(Vector3 vec)
        {
            vec.x = (float)Math.Round(vec.x, 2);
            vec.y = (float)Math.Round(vec.y, 2);
            vec.z = (float)Math.Round(vec.z, 2);
            return vec;
        }

        public static void ApplyPrefab(GameObject gitem)
        {
            var instanceRoot = PrefabUtility.FindValidUploadPrefabInstanceRoot(gitem);
            var prefab = PrefabUtility.GetPrefabParent(instanceRoot);
            if (prefab != null)
            {
                if (prefab.name == gitem.name)
                {
                    PrefabUtility.ReplacePrefab(gitem, prefab, ReplacePrefabOptions.ConnectToPrefab);
                }
            }
        }

        internal static SerializedProperty AddItem(this SerializedProperty arrayProperty)
        {
            arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
            var prop = arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1);
            return prop;
        }

        internal static GameObject LoadPrefab(GameObject prefab, ref int instanceID)
        {
            if (instanceID != 0)
            {
                var gitem = EditorUtility.InstanceIDToObject(instanceID);
                if (gitem != null)
                {
                    GameObject.DestroyImmediate(gitem);
                }
            }
            if (prefab != null)
            {
                var actionSystem = GameObject.FindObjectOfType<ActionGroup>();
                var parent = actionSystem == null ? null : actionSystem.transform;
                GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                instanceID = go.GetInstanceID();
                go.transform.SetParent(parent, false);
                return go;
            }
            return null;
        }

        internal static GameObject LoadPrefab(string guid, SerializedProperty instenceIDProp, SerializedProperty coordinateProp)
        {
            if (instenceIDProp.intValue != 0)
            {
                var gitem = EditorUtility.InstanceIDToObject(instenceIDProp.intValue);
                if (gitem != null)
                {
                    GameObject.DestroyImmediate(gitem);
                }
            }

            if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid)))
            {
                return null;
            }
            else
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                var id = instenceIDProp.intValue;
                var instenceObj = ActionEditorUtility.LoadPrefab(prefab, ref id);
                instenceIDProp.intValue = id;
                ActionEditorUtility.LoadCoordinatePropInfo(coordinateProp, instenceObj.transform);
                return instenceObj;
            }

        }
        internal static void LoadPrefab(SerializedProperty prefabProp, SerializedProperty instanceIDProp)
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
                var actionSystem = GameObject.FindObjectOfType<ActionGroup>();
                var parent = actionSystem == null ? null : actionSystem.transform;
                GameObject go = PrefabUtility.InstantiatePrefab(gopfb) as GameObject;
                instanceIDProp.intValue = go.GetInstanceID();
                go.transform.SetParent(parent, false);
            }
        }
        internal static void LoadPrefab(SerializedProperty prefabProp, SerializedProperty instanceIDProp, SerializedProperty coodinate)
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
                var actionSystem = GameObject.FindObjectOfType<ActionGroup>();
                var parent = actionSystem == null ? null : actionSystem.transform;
                GameObject go = PrefabUtility.InstantiatePrefab(gopfb) as GameObject;
                instanceIDProp.intValue = go.GetInstanceID();
                instanceIDProp.serializedObject.ApplyModifiedProperties();
                go.transform.SetParent(parent, false);
                LoadCoordinatePropInfo(coodinate, go.transform);
            }
        }
        internal static void SavePrefab(ref int instanceID)
        {
            var gitem = EditorUtility.InstanceIDToObject(instanceID);
            if (gitem != null)
            {
                ActionEditorUtility.ApplyPrefab(gitem as GameObject);
                GameObject.DestroyImmediate(gitem);
            }
            instanceID = 0;
        }
        internal static void SavePrefab(SerializedProperty instanceIDProp)
        {
            var gitem = EditorUtility.InstanceIDToObject(instanceIDProp.intValue);
            if (gitem != null)
            {
                ActionEditorUtility.ApplyPrefab(gitem as GameObject);
                GameObject.DestroyImmediate(gitem);
            }
            instanceIDProp.intValue = 0;
        }
        private static bool Ignore(PropertyModification[] modifyed)
        {
            foreach (var item in modifyed)
            {
                if (!coondinatePaths.Contains(item.propertyPath))
                {
                    Debug.Log(item.propertyPath);
                    return false;
                }
            }
            Debug.Log("ignore changes");
            return true;
        }
        internal static void SavePrefab(SerializedProperty instanceIDProp, SerializedProperty coordinate)
        {
            var gitem = EditorUtility.InstanceIDToObject(instanceIDProp.intValue);
            if (gitem != null)
            {
                var transform = (gitem as GameObject).transform;
                ActionEditorUtility.SaveCoordinatesInfo(coordinate, transform);
                var modifyeds = PrefabUtility.GetPropertyModifications(gitem);
                if (!Ignore(modifyeds))
                {
                    ActionEditorUtility.ApplyPrefab(gitem as GameObject);
                }
                GameObject.DestroyImmediate(gitem);
            }
            instanceIDProp.intValue = 0;
        }
        internal static void InsertPrefab(SerializedProperty prefabProp, UnityEngine.Object obj)
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
        public static void ResetValue(SerializedProperty property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = false;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = 0f;
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = "";
                    break;
                case SerializedPropertyType.Color:
                    property.colorValue = Color.black;
                    break;
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = null;
                    break;
                case SerializedPropertyType.LayerMask:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.Enum:
                    property.enumValueIndex = 0;
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = default(Vector2);
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = default(Vector3);
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = default(Vector4);
                    break;
                case SerializedPropertyType.Rect:
                    property.rectValue = default(Rect);
                    break;
                case SerializedPropertyType.ArraySize:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.Character:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);
                    break;
                case SerializedPropertyType.Bounds:
                    property.boundsValue = default(Bounds);
                    break;
                case SerializedPropertyType.Gradient:
                    //!TODO: Amend when Unity add a public API for setting the gradient.
                    break;
            }

            if (property.isArray)
            {
                property.arraySize = 0;
            }

            ResetChildPropertyValues(property);
        }

        public static bool HaveElement(this SerializedProperty arryProp, string path, UnityEngine.Object obj)
        {
            for (int i = 0; i < arryProp.arraySize; i++)
            {
                var prop = arryProp.GetArrayElementAtIndex(i);
                var prefab_prop = prop.FindPropertyRelative(path);
                if (prefab_prop.objectReferenceValue == obj)
                {
                    return true;
                }
            }
            return false;
        }
        private static void ResetChildPropertyValues(SerializedProperty element)
        {
            if (!element.hasChildren)
                return;

            var childProperty = element.Copy();
            int elementPropertyDepth = element.depth;
            bool enterChildren = true;

            while (childProperty.Next(enterChildren) && childProperty.depth > elementPropertyDepth)
            {
                enterChildren = false;
                ResetValue(childProperty);
            }
        }

        /// <summary>
        /// Copies value of <paramref name="sourceProperty"/> into <pararef name="destProperty"/>.
        /// </summary>
        /// <param name="destProperty">Destination property.</param>
        /// <param name="sourceProperty">Source property.</param>
        public static void CopyPropertyValue(SerializedProperty destProperty, SerializedProperty sourceProperty)
        {
            if (destProperty == null)
                throw new ArgumentNullException("destProperty");
            if (sourceProperty == null)
                throw new ArgumentNullException("sourceProperty");

            sourceProperty = sourceProperty.Copy();
            destProperty = destProperty.Copy();

            CopyPropertyValueSingular(destProperty, sourceProperty);

            if (sourceProperty.hasChildren)
            {
                int elementPropertyDepth = sourceProperty.depth;
                while (sourceProperty.Next(true) && destProperty.Next(true) && sourceProperty.depth > elementPropertyDepth)
                    CopyPropertyValueSingular(destProperty, sourceProperty);
            }
        }

        private static void CopyPropertyValueSingular(SerializedProperty destProperty, SerializedProperty sourceProperty)
        {
            switch (destProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    destProperty.intValue = sourceProperty.intValue;
                    break;
                case SerializedPropertyType.Boolean:
                    destProperty.boolValue = sourceProperty.boolValue;
                    break;
                case SerializedPropertyType.Float:
                    destProperty.floatValue = sourceProperty.floatValue;
                    break;
                case SerializedPropertyType.String:
                    destProperty.stringValue = sourceProperty.stringValue;
                    break;
                case SerializedPropertyType.Color:
                    destProperty.colorValue = sourceProperty.colorValue;
                    break;
                case SerializedPropertyType.ObjectReference:
                    destProperty.objectReferenceValue = sourceProperty.objectReferenceValue;
                    break;
                case SerializedPropertyType.LayerMask:
                    destProperty.intValue = sourceProperty.intValue;
                    break;
                case SerializedPropertyType.Enum:
                    destProperty.enumValueIndex = sourceProperty.enumValueIndex;
                    break;
                case SerializedPropertyType.Vector2:
                    destProperty.vector2Value = sourceProperty.vector2Value;
                    break;
                case SerializedPropertyType.Vector3:
                    destProperty.vector3Value = sourceProperty.vector3Value;
                    break;
                case SerializedPropertyType.Vector4:
                    destProperty.vector4Value = sourceProperty.vector4Value;
                    break;
                case SerializedPropertyType.Rect:
                    destProperty.rectValue = sourceProperty.rectValue;
                    break;
                case SerializedPropertyType.ArraySize:
                    destProperty.intValue = sourceProperty.intValue;
                    break;
                case SerializedPropertyType.Character:
                    destProperty.intValue = sourceProperty.intValue;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    destProperty.animationCurveValue = sourceProperty.animationCurveValue;
                    break;
                case SerializedPropertyType.Bounds:
                    destProperty.boundsValue = sourceProperty.boundsValue;
                    break;
                case SerializedPropertyType.Gradient:
                    //!TODO: Amend when Unity add a public API for setting the gradient.
                    break;
            }
        }

    }
}