using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System;

namespace WorldActionSystem
{

    public abstract class ActionGroupDrawerBase : Editor
    {
        public enum SortType
        {
            ByName
        }
        protected bool swink;
        protected string query;
        protected string runtimeQuery;
        protected SerializedProperty script;
        protected SerializedProperty groupKeyProp;
        protected SerializedProperty totalCommandProp;
        protected SerializedProperty autoLoadListProp;
        protected SerializedProperty autoLoadListWorp;

        protected SerializedProperty runTimeElementProp;
        protected SerializedProperty runTimeElementWorp;

        protected static int selected = 0;
        protected static SortType currSortType;
        protected GUIContent[] selectables;
        protected GUIContent[] Selectables
        {
            get
            {
                if (selectables == null)
                {
                    selectables = new GUIContent[] {
                        new GUIContent(EditorGUIUtility.IconContent("winbtn_mac_max").image,"all"),
                        new GUIContent(EditorGUIUtility.IconContent("winbtn_mac_close").image,"contains command"),
                        new GUIContent(EditorGUIUtility.IconContent("winbtn_mac_min").image,"contains pickup"),
                        new GUIContent(EditorGUIUtility.IconContent("winbtn_mac_inact").image,"normal"),
                    };
                }
                return selectables;
            }
        }

        protected ReorderableList prefabList_r;
        protected ReorderableList prefabListWorp_r;

        protected ReorderableList runtimeElement_r;
        protected ReorderableList runtimeElementWorp_r;

        private void OnEnable()
        {
            script = serializedObject.FindProperty("m_Script");
            totalCommandProp = serializedObject.FindProperty("totalCommand");
            groupKeyProp = serializedObject.FindProperty("groupKey");
            autoLoadListProp = serializedObject.FindProperty("autoLoadElement");
            runTimeElementProp = serializedObject.FindProperty("runTimeElements");
            MarchList();
            CalcuteCommandCount();
            InitAutoLoadReorderList();
            InitRunTimeReorderList();
        }


        private void OnDisable()
        {
            ApplyWorpPrefabList();
            CalcuteCommandCount();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawScript();
            DrawControllers();
            DrawAutoLoadList();
            DrawRuntimeLoadList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAutoLoadList()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                DrawToolButtons(autoLoadListProp, autoLoadListWorp);
                if (GUILayout.Button(new GUIContent("反向忽略"), EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    IgnoreNotIgnored(autoLoadListProp);
                }
                GUILayout.FlexibleSpace();
                GUILayout.Label("(初始化便自动加载)");
            }
            DrawSelection();
            DrawAutoItems();
            DrawAcceptRegion(autoLoadListProp);
        }

        private void DrawRuntimeLoadList()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                DrawToolButtons(runTimeElementProp, runTimeElementWorp);
                GUILayout.FlexibleSpace();
                GUILayout.Label("(手动调用选择加载)");
            }
            DrawRunTimeItems();
            DrawAcceptRegion(runTimeElementProp);
        }

        private void InitAutoLoadReorderList()
        {
            prefabList_r = new ReorderableList(serializedObject, autoLoadListProp);
            prefabList_r.elementHeightCallback += GetElementHeight;
            prefabList_r.drawHeaderCallback += DrawPrefabListHeader;
            prefabList_r.drawElementCallback += DrawListElement;

            var sobj = new SerializedObject(ScriptableObject.CreateInstance<ActionGroupObj>());
            autoLoadListWorp = sobj.FindProperty("autoLoadElement");
            prefabListWorp_r = new ReorderableList(sobj, autoLoadListWorp);
            prefabListWorp_r.drawHeaderCallback += DrawWorpListHeader;
            prefabListWorp_r.drawElementCallback += DrawWorpListElement;
            prefabListWorp_r.elementHeightCallback += GetWorpElementHeight;
        }
        private void InitRunTimeReorderList()
        {
            runtimeElement_r = new ReorderableList(serializedObject, runTimeElementProp);
            runtimeElement_r.drawHeaderCallback += (rect) =>
            {
                if (GUI.Button(rect, "动态元素列表", EditorStyles.boldLabel))
                {
                    RemoveRuntimeElements(GetRunTimePrefabs());
                    EditorUtility.SetDirty(target);
                }
            };
            runtimeElement_r.drawElementCallback += DrawRuntimeElements;
            var sobj = new SerializedObject(ScriptableObject.CreateInstance<ActionGroupObj>());
            runTimeElementWorp = sobj.FindProperty("runTimeElements");
            runtimeElementWorp_r = new ReorderableList(sobj, runTimeElementWorp);
            runtimeElementWorp_r.drawHeaderCallback += DrawWorpListHeader;
            runtimeElementWorp_r.drawElementCallback += DrawRuntimeElementsWorp;
        }

        private void DrawRuntimeElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            var prop = runTimeElementProp.GetArrayElementAtIndex(index);
            if (prop != null) EditorGUI.PropertyField(rect, prop, true);
        }
        private void DrawRuntimeElementsWorp(Rect rect, int index, bool isActive, bool isFocused)
        {
            var prop = runTimeElementWorp.GetArrayElementAtIndex(index);
            if (prop != null) EditorGUI.PropertyField(rect, prop, true);
        }

        private float GetWorpElementHeight(int index)
        {
            var property = autoLoadListWorp.GetArrayElementAtIndex(index);
            return (property.isExpanded ? 2 : 1) * EditorGUIUtility.singleLineHeight;
        }

        private float GetElementHeight(int index)
        {
            var property = autoLoadListProp.GetArrayElementAtIndex(index);
            return (property.isExpanded ? 2 : 1) * EditorGUIUtility.singleLineHeight;
        }

        private void DrawWorpListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var prop = autoLoadListWorp.GetArrayElementAtIndex(index);
            if (prop != null) EditorGUI.PropertyField(rect, prop, true);
        }

        private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var prop = autoLoadListProp.GetArrayElementAtIndex(index);
            if (prop != null) EditorGUI.PropertyField(rect, prop, true);
        }

        private void DrawWorpListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "[March]", EditorStyles.boldLabel);
        }

        private void DrawPrefabListHeader(Rect rect)
        {
            if (GUI.Button(rect, "静态元素列表", EditorStyles.boldLabel))
            {
                RemoveAutoPrefabs(GetAutoPrefabs());
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawSelection()
        {
            EditorGUI.BeginChangeCheck();
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                query = EditorGUILayout.TextField(query);
                selected = GUILayout.SelectionGrid(selected, Selectables, 4);
            }
            if (EditorGUI.EndChangeCheck())
            {
                MarchList();
            }
        }

        private void DrawControllers()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(groupKeyProp);
            }

            if (string.IsNullOrEmpty(groupKeyProp.stringValue))
            {
                groupKeyProp.stringValue = target.name;
            }
        }

        private void DrawScript()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(script);
            EditorGUI.EndDisabledGroup();
        }

        protected virtual void DrawRunTimeItems()
        {
            if (string.IsNullOrEmpty(runtimeQuery))
            {
                runtimeElement_r.DoLayoutList();
            }
            else
            {
                runtimeElementWorp_r.DoLayoutList();
            }

        }
        protected virtual void DrawAutoItems()
        {
            if (string.IsNullOrEmpty(query) && selected == 0)
            {
                prefabList_r.DoLayoutList();
            }
            else
            {
                prefabListWorp_r.DoLayoutList();
            }

        }
        protected abstract List<AutoPrefabItem> GetAutoPrefabs();
        protected abstract List<RunTimePrefabItem> GetRunTimePrefabs();
        private void RemoveAutoPrefabs(List<AutoPrefabItem> list)
        {
            var newList = new List<AutoPrefabItem>();
            var needRemove = new List<AutoPrefabItem>();
            foreach (var item in list)
            {
                if (newList.Find(x => x.ID == item.ID) == null)
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
                list.Remove(item);
            }
        }
        private void RemoveRuntimeElements(List<RunTimePrefabItem> list)
        {
            var newList = new List<RunTimePrefabItem>();
            var needRemove = new List<RunTimePrefabItem>();
            foreach (var item in list)
            {
                if (newList.Find(x => x.ID == item.ID) == null)
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
                list.Remove(item);
            }
        }
        private void MarchList()
        {
            if (string.IsNullOrEmpty(query) && selected == 0) return;

            autoLoadListWorp.ClearArray();

            for (int i = 0; i < autoLoadListProp.arraySize; i++)
            {
                var prop = autoLoadListProp.GetArrayElementAtIndex(i);
                var prefabProp = prop.FindPropertyRelative("prefab");

                if (prefabProp.objectReferenceValue == null || !prefabProp.objectReferenceValue.name.ToLower().Contains(query.ToLower()))
                {
                    continue;
                }

                var containCommandProp = prop.FindPropertyRelative("containsCommand");
                var containsPickupProp = prop.FindPropertyRelative("containsPickup");

                if (selected == 1)
                {
                    if (!containCommandProp.boolValue)
                    {
                        continue;
                    }
                }
                else if (selected == 2)
                {
                    if (!containsPickupProp.boolValue)
                    {
                        continue;
                    }
                }
                else if (selected == 3)
                {
                    if (containCommandProp.boolValue || containsPickupProp.boolValue)
                    {
                        continue;
                    }
                }

                autoLoadListWorp.InsertArrayElementAtIndex(0);
                ActionEditorUtility.CopyPropertyValue(autoLoadListWorp.GetArrayElementAtIndex(0), autoLoadListProp.GetArrayElementAtIndex(i));
            }
        }

        private void DrawToolButtons(SerializedProperty property, SerializedProperty worpProperty)
        {
            var btnStyle = EditorStyles.miniButton;

            if (GUILayout.Button(new GUIContent("排序"), btnStyle, GUILayout.Width(60)))
            {
                SortPrefabs(selected == 0 ? property : worpProperty);
            }
            if (GUILayout.Button(new GUIContent("批量加载"), btnStyle, GUILayout.Width(60)))
            {
                GroupLoadPrefabs(selected == 0 ? property : worpProperty);
            }
            if (GUILayout.Button(new GUIContent("批量关闭"), btnStyle, GUILayout.Width(60)))
            {
                CloseAllCreated(selected == 0 ? property : worpProperty);
            }

        }
        protected void SortPrefabs(SerializedProperty property)
        {
            if (currSortType == SortType.ByName)
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    for (int j = i; j < property.arraySize - i - 1; j++)
                    {
                        var itemj = property.GetArrayElementAtIndex(j).FindPropertyRelative("prefab");
                        var itemj1 = property.GetArrayElementAtIndex(j + 1).FindPropertyRelative("prefab");

                        if (itemj.objectReferenceValue == null || itemj1.objectReferenceValue == null) continue;

                        if (string.Compare(itemj.objectReferenceValue.name, itemj1.objectReferenceValue.name) > 0)
                        {
                            property.MoveArrayElement(j, j + 1);
                        }
                    }
                }
            }
        }

        private void IgnoreNotIgnored(SerializedProperty prefabListProp)
        {
            for (int i = 0; i < prefabListProp.arraySize; i++)
            {
                var ignorePorp = prefabListProp.GetArrayElementAtIndex(i).FindPropertyRelative("ignore");
                if (ignorePorp != null)
                {
                    ignorePorp.boolValue = !ignorePorp.boolValue;
                }
            }
        }
        /// <summary>
        /// 绘制作快速导入的区域
        /// </summary>
        private void DrawAcceptRegion(SerializedProperty property)
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            rect.y -= EditorGUIUtility.singleLineHeight;
            if (rect.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:

                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        break;
                    case EventType.DragPerform:
                        if (DragAndDrop.objectReferences.Length > 0)
                        {
                            var objs = DragAndDrop.objectReferences;
                            for (int i = 0; i < objs.Length; i++)
                            {
                                var obj = objs[i];
                                var prefab = PrefabUtility.GetPrefabParent(obj);
                                UnityEngine.Object goodObj = null;
                                if (prefab != null)
                                {
                                    goodObj = PrefabUtility.FindPrefabRoot(prefab as GameObject);
                                }
                                else
                                {
                                    var path = AssetDatabase.GetAssetPath(obj);
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        goodObj = obj;
                                    }
                                }
                                property.InsertArrayElementAtIndex(property.arraySize);
                                var itemprefab = property.GetArrayElementAtIndex(property.arraySize - 1);
                                itemprefab.FindPropertyRelative("prefab").objectReferenceValue = goodObj;
                            }
                        }
                        break;
                }
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
                if (instanceIDProp.intValue != 0)
                {
                    var gitem = EditorUtility.InstanceIDToObject(instanceIDProp.intValue);
                    if (gitem != null)
                    {
                        continue;
                    }
                }
                prefab = prefabProp.objectReferenceValue as GameObject;

                if (prefab == null)
                {
                    UnityEditor.EditorUtility.DisplayDialog("空对象", "找不到预制体", "确认");
                }
                else
                {
                    var matrixProp = itemProp.FindPropertyRelative("matrix");
                    var rematrixProp = itemProp.FindPropertyRelative("rematrix");
                    var containsCommandProp = itemProp.FindPropertyRelative("containsCommand");
                    var containsPickupProp = itemProp.FindPropertyRelative("containsPickup");
                    if (matrixProp != null)
                    {
                        ActionEditorUtility.LoadPrefab(prefabProp, instanceIDProp, containsCommandProp, containsPickupProp,  rematrixProp, matrixProp);
                    }
                    else
                    {
                        ActionEditorUtility.LoadPrefab(prefabProp, instanceIDProp);
                    }
                    itemProp.isExpanded = true;
                }

            }
        }



        private void CloseAllCreated(SerializedProperty arrayProp)
        {
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                var itemProp = arrayProp.GetArrayElementAtIndex(i);
                var instanceIDPorp = itemProp.FindPropertyRelative("instanceID");
                var obj = EditorUtility.InstanceIDToObject(instanceIDPorp.intValue);
                if (obj != null)
                {
                    var matrixProp = itemProp.FindPropertyRelative("matrix");
                    var rematrixProp = itemProp.FindPropertyRelative("rematrix");
                    if (rematrixProp != null)
                    {
                        ActionEditorUtility.SavePrefab(instanceIDPorp, rematrixProp, matrixProp);
                    }
                    else
                    {
                        ActionEditorUtility.SavePrefab(instanceIDPorp);
                    }
                    DestroyImmediate(obj);
                }
                itemProp.isExpanded = false;
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

        private void CalcuteCommandCount()
        {
            if (target == null) return;

            var commandList = new List<ActionCommand>();

            if (target is ActionGroup)
            {
                var transform = (target as ActionGroup).transform;
                //Utility.RetiveBehaiver<ActionCommand>(transform, (x) => { if (!commandList.Contains(x)) commandList.Add(x); });
            }


            for (int i = 0; i < autoLoadListProp.arraySize; i++)
            {
                var prop = autoLoadListProp.GetArrayElementAtIndex(i);
                var ignore = prop.FindPropertyRelative("ignore");

                var pfb = prop.FindPropertyRelative("prefab");
                var contaionCommand = prop.FindPropertyRelative("containsCommand");
                contaionCommand.boolValue = false;
                var contaionPickUp = prop.FindPropertyRelative("containsPickup");
                contaionPickUp.boolValue = false;

                if (pfb.objectReferenceValue != null)
                {
                    var go = pfb.objectReferenceValue as GameObject;
                    //Utility.RetiveBehaiver<ActionCommand>(go.transform, (x) =>
                    //{
                    //    if (x != null)
                    //    {
                    //        contaionCommand.boolValue = true;
                    //        if (!commandList.Contains(x) && !ignore.boolValue) commandList.Add(x);
                    //    }
                    //});
                    //Utility.RetiveBehaiver<>(go.transform, (x) =>
                    //{
                    //    if (x != null)
                    //    {
                    //        contaionPickUp.boolValue = true;
                    //    }
                    //});
                }
            }
            totalCommandProp.intValue = commandList.Count;
            serializedObject.ApplyModifiedProperties();
        }

        private void ApplyWorpPrefabList()
        {
            List<SerializedProperty> needAdd = new List<SerializedProperty>();

            if (autoLoadListWorp.arraySize > 0)
            {
                for (int i = 0; i < autoLoadListWorp.arraySize; i++)
                {
                    var newProp = autoLoadListWorp.GetArrayElementAtIndex(i);
                    var newprefabProp = newProp.FindPropertyRelative("prefab");
                    var newrematrixProp = newProp.FindPropertyRelative("rematrix");
                    bool contain = false;
                    for (int j = 0; j < autoLoadListProp.arraySize; j++)
                    {
                        var prop = autoLoadListProp.GetArrayElementAtIndex(j);
                        var prefabProp = prop.FindPropertyRelative("prefab");
                        var rematrixProp = prop.FindPropertyRelative("rematrix");
                        if (prefabProp.objectReferenceValue == newprefabProp.objectReferenceValue)
                        {
                            if (newrematrixProp.boolValue == rematrixProp.boolValue && rematrixProp.boolValue == false)
                            {
                                ActionEditorUtility.CopyPropertyValue(prop, newProp);
                            }
                            contain = true;
                        }
                    }
                    if (!contain)
                    {
                        needAdd.Add(newProp);
                    }
                }
            }

            for (int i = 0; i < needAdd.Count; i++)
            {
                autoLoadListProp.InsertArrayElementAtIndex(0);
                var newProp = needAdd[i];
                var prop = autoLoadListProp.GetArrayElementAtIndex(0);
                ActionEditorUtility.CopyPropertyValue(prop, newProp);
            }
        }
    }
}