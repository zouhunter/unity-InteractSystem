using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Rotorz.ReorderableList;
using Rotorz.ReorderableList.Internal;

namespace WorldActionSystem
{
    [CustomEditor(typeof(ActionSystem)), CanEditMultipleObjects]
    public class ActionSystemDrawer : Editor
    {
        protected bool swink;
        protected string query;

        protected SerializedProperty script;

        protected SerializedProperty prefabListProp;
        protected DragAdapt prefabListPropAdapt;

        protected SerializedProperty prefabListWorp;
        protected DragAdapt prefabListWorpAdapt;

        private void OnEnable()
        {
            script = serializedObject.FindProperty("m_Script");

            prefabListProp = serializedObject.FindProperty("prefabList");
            prefabListPropAdapt = new DragAdapt(prefabListProp, "prefabList");

            var sobj = new SerializedObject(ScriptableObject.CreateInstance<ActionSystemObj>());

            prefabListWorp = sobj.FindProperty("prefabList");
            prefabListWorpAdapt = new DragAdapt(prefabListWorp, "prefabList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawScript();
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                DrawToolButtons();
            }
            DrawRuntimeItems();
            DrawAcceptRegion();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawScript()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(script);
            EditorGUI.EndDisabledGroup();
        }

        protected virtual void DrawRuntimeItems()
        {
            EditorGUI.BeginChangeCheck();
            query = EditorGUILayout.TextField(query);
            if (EditorGUI.EndChangeCheck())
            {
                MarchList();
            }
            if (string.IsNullOrEmpty(query))
            {
                ReorderableListGUI.Title("元素列表");
                Rotorz.ReorderableList.ReorderableListGUI.ListField(prefabListPropAdapt);
            }
            else
            {
                ReorderableListGUI.Title("[March]");
                Rotorz.ReorderableList.ReorderableListGUI.ListField(prefabListWorpAdapt);
            }

        }

        private void MarchList()
        {
            if (!string.IsNullOrEmpty(query))
            {
                prefabListWorp.ClearArray();
                for (int i = 0; i < prefabListProp.arraySize; i++)
                {
                    var prefabProp = prefabListProp.GetArrayElementAtIndex(i).FindPropertyRelative("prefab");
                    if (prefabProp.objectReferenceValue != null && prefabProp.objectReferenceValue.name.ToLower().Contains(query.ToLower()))
                    {
                        prefabListWorp.InsertArrayElementAtIndex(0);
                        SerializedPropertyUtility.CopyPropertyValue(prefabListWorp.GetArrayElementAtIndex(i), prefabProp.GetArrayElementAtIndex(0));
                    }
                }
            }
        }

        private void DrawToolButtons()
        {
            var btnStyle = EditorStyles.toolbarButton;
            if (GUILayout.Button(new GUIContent("%", "移除重复"), btnStyle))
            {
                RemoveDouble(prefabListProp);
            }
            if (GUILayout.Button(new GUIContent("！", "排序"), btnStyle))
            {
                SortPrefabs(prefabListProp);
            }
            if (GUILayout.Button(new GUIContent("o", "批量加载"), btnStyle))
            {
                GroupLoadPrefabs(prefabListProp);
            }
            if (GUILayout.Button(new GUIContent("c", "批量关闭"), btnStyle))
            {
                CloseAllCreated(prefabListProp);
            }
        }


        /// <summary>
        /// 绘制作快速导入的区域
        /// </summary>
        private void DrawAcceptRegion()
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent("哈哈"), EditorStyles.toolbarButton);
            rect.y -= EditorGUIUtility.singleLineHeight;
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                    break;
                case EventType.DragPerform:
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        var objs = DragAndDrop.objectReferences;
                        for (int i = 0; i < objs.Length; i++)
                        {
                            var obj = objs[i];
                            prefabListProp.InsertArrayElementAtIndex(prefabListProp.arraySize);
                            var itemprefab = prefabListProp.GetArrayElementAtIndex(prefabListProp.arraySize - 1);
                            itemprefab.FindPropertyRelative("prefab").objectReferenceValue = obj;
                        }
                    }
                    break;
            }
        }

        private void GroupLoadPrefabs(SerializedProperty proprety)
        {
            for (int i = 0; i < proprety.arraySize; i++)
            {
                var itemProp = proprety.GetArrayElementAtIndex(i);
                GameObject prefab = null;
                var prefabProp = itemProp.FindPropertyRelative("prefab");
                var instanceIDProp = itemProp.FindPropertyRelative("instanceID");

                prefab = prefabProp.objectReferenceValue as GameObject;

                if (prefab == null)
                {
                    UnityEditor.EditorUtility.DisplayDialog("空对象", "找不到预制体", "确认");
                }
                else
                {
                    var resetProp = itemProp.FindPropertyRelative("reset");
                    var targetProp = itemProp.FindPropertyRelative("target");
                    GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

                    go.transform.SetParent((target as ActionSystem).transform, true);

                    if (resetProp.boolValue && targetProp.objectReferenceValue != null)
                    {
                        var targetO = targetProp.objectReferenceValue as Transform;
                        go.transform.position = targetO.position;
                        go.transform.rotation = targetO.rotation;
                        go.transform.localScale = targetO.localScale;
                    }

                    instanceIDProp.intValue = go.GetInstanceID();
                }

            }
        }

        private void RemoveDouble(SerializedProperty property)
        {
            var actionSystem = target as ActionSystem;
            var newList = new List<ActionPrefabItem>();
            var needRemove = new List<ActionPrefabItem>();
            foreach (var item in actionSystem.prefabList)
            {
                if(newList.Find(x=>x.ID == item.ID) == null)
                {
                    newList.Add(item);
                }
                else
                {
                    needRemove.Add(item);
                }
            }
            foreach (var item in needRemove)
            {
                actionSystem.prefabList.Remove(item);
            }
            EditorUtility.SetDirty(actionSystem);
        }

        private void CloseAllCreated(SerializedProperty arrayProp)
        {
            TrySaveAllPrefabs(arrayProp);
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                var item = arrayProp.GetArrayElementAtIndex(i);
                var instanceIDPorp = item.FindPropertyRelative("instanceID");
                var obj = EditorUtility.InstanceIDToObject(instanceIDPorp.intValue);
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
                instanceIDPorp.intValue = 0;
            }
        }
        private void SortPrefabs(SerializedProperty property)
        {
            for (int i = 0; i < property.arraySize; i++)
            {
                for (int j = i; j < property.arraySize - i - 1; j++)
                {
                    var itemj = property.GetArrayElementAtIndex(j).FindPropertyRelative("prefab");
                    var itemj1 = property.GetArrayElementAtIndex(j + 1).FindPropertyRelative("prefab");
                    if(itemj.objectReferenceValue != null &&itemj1.objectReferenceValue != null)
                    {
                        var name1 = itemj.objectReferenceValue.name;
                        var name2 = itemj1.objectReferenceValue.name;

                        if (string.Compare(name1, name2) > 0)
                        {
                            property.MoveArrayElement(j, j + 1);
                        }
                    }
                   
                }
            }
        }

        private void TrySaveAllPrefabs(SerializedProperty arrayProp)
        {
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                var item = arrayProp.GetArrayElementAtIndex(i);
                var instanceIDPorp = item.FindPropertyRelative("instanceID");
                var obj = EditorUtility.InstanceIDToObject(instanceIDPorp.intValue);
                if (obj == null) continue;
                var prefab = PrefabUtility.GetPrefabParent(obj);
                if (prefab != null)
                {
                    var root = PrefabUtility.FindPrefabRoot((GameObject)prefab);
                    if (root != null)
                    {
                        PrefabUtility.ReplacePrefab(obj as GameObject, root, ReplacePrefabOptions.ConnectToPrefab);
                    }
                }
            }
        }
    }
}