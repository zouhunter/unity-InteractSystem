using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace InteractSystem
{
    [CustomEditor(typeof(PrefabGroupObj))]
    public class PrefabGroupObjDrawer : Editor
    {
        SerializedProperty scriptProp;
        SerializedProperty discriptionProp;
        SerializedProperty prefabsProp;
        SerializedProperty prefabInfosProp;

        ReorderableList prefabsList;
        ReorderableList infosList;
        Dictionary<Object, int> catchDic = new Dictionary<Object, int>();
        const float btnWidth = 60;
        private void OnEnable()
        {
            scriptProp = serializedObject.FindProperty("m_Script");
            discriptionProp = serializedObject.FindProperty("discription");
            prefabInfosProp = serializedObject.FindProperty("prefabInfos");
            prefabsProp = serializedObject.FindProperty("prefabs");
            InitReorderList();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("一组预制体,保持每个元素独立,当需要时,加载出这一组的相对坐标", MessageType.Info);
            discriptionProp.stringValue = EditorGUILayout.TextArea(discriptionProp.stringValue);
            EditorGUILayout.ObjectField(scriptProp.objectReferenceValue,typeof(MonoScript),false);
            EditorGUILayout.ObjectField(target, typeof(PrefabGroupObj), false);
            serializedObject.Update();
            prefabsList.DoLayoutList();
            AcceptPrefab();
            infosList.DoLayoutList();
            AcceptPrefabInfo();
            serializedObject.ApplyModifiedProperties();
        }

        private void AcceptPrefabInfo()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            if (rect.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        break;
                    case EventType.DragPerform:
                        if (DragAndDrop.objectReferences != null)
                        {
                            foreach (var item in DragAndDrop.objectReferences)
                            {
                                if (item is GameObject)
                                {
                                    var obj = item as GameObject;
                                    var prefab = PrefabUtility.GetPrefabParent(obj) as GameObject;
                                    if (prefab != null)
                                    {
                                        prefabInfosProp.InsertArrayElementAtIndex(prefabInfosProp.arraySize);
                                        var prop = prefabInfosProp.GetArrayElementAtIndex(prefabInfosProp.arraySize - 1);
                                        UpdateOneInfo(prop, obj as GameObject, prefab as GameObject);
                                    }
                                  
                                }
                            }
                        }
                        break;
                    case EventType.DragExited:
                        break;
                    default:
                        break;
                }
            }
        }

        private void AcceptPrefab()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            if (rect.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        break;
                    case EventType.DragPerform:
                        if (DragAndDrop.objectReferences != null)
                        {
                            foreach (var item in DragAndDrop.objectReferences)
                            {
                                if (item is GameObject)
                                {
                                    var obj = item as GameObject;
                                    var parent = PrefabUtility.GetPrefabParent(obj) as GameObject;
                                    if (parent != null)
                                    {
                                        obj = parent;
                                    }
                                    prefabsProp.InsertArrayElementAtIndex(prefabsProp.arraySize);
                                    prefabsProp.GetArrayElementAtIndex(prefabsProp.arraySize - 1).objectReferenceValue = obj;
                                }
                            }
                        }
                        break;
                    case EventType.DragExited:
                        break;
                    default:
                        break;
                }
            }
        }

        private void InitReorderList()
        {
            prefabsList = new ReorderableList(serializedObject, prefabsProp, true, true, true, true);
            prefabsList.drawHeaderCallback += DrawPrefabsHeader;
            prefabsList.drawElementCallback += DrawPrefabElement;

            infosList = new ReorderableList(serializedObject, prefabInfosProp, true, true, true, true);
            infosList.drawHeaderCallback += DrawPrefabsInfoHeader;
            infosList.drawElementCallback += DrawPrefabsInfoElment;
        }

        private void DrawPrefabsInfoHeader(Rect rect)
        {
            var titleRect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(titleRect, "信息列表");
            var createAllRect = new Rect(rect.x + rect.width - 3 * btnWidth, rect.y, btnWidth, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(createAllRect, "clear"))
            {
                var confer = EditorUtility.DisplayDialog("温馨提示", "如需清除所有记录的信息,请按确认", "确认");
                if (confer)
                {
                    prefabInfosProp.ClearArray();
                }
            }
            createAllRect.x += btnWidth;
            if (GUI.Button(createAllRect, "record"))
            {
                if (Selection.instanceIDs != null)
                {
                    List<GameObject> prefabs = (target as PrefabGroupObj).prefabs;
                    foreach (var item in Selection.instanceIDs)
                    {
                        var obj = EditorUtility.InstanceIDToObject(item);
                        if (obj is GameObject)
                        {
                            var prefab = PrefabUtility.GetPrefabParent(obj);
                            if (prefab != null)
                            {
                                prefabInfosProp.InsertArrayElementAtIndex(prefabInfosProp.arraySize);
                                var prop = prefabInfosProp.GetArrayElementAtIndex(prefabInfosProp.arraySize - 1);
                                UpdateOneInfo(prop, obj as GameObject, prefab as GameObject);

                                if(!prefabs.Contains(prefab as GameObject))
                                {
                                    prefabs.Add(prefab as GameObject);
                                    prefabsProp.InsertArrayElementAtIndex(prefabsProp.arraySize);
                                    prefabsProp.GetArrayElementAtIndex(prefabsProp.arraySize - 1).objectReferenceValue = prefab;
                                }
                            }
                        }
                    }
                    EditorUtility.SetDirty(target);

                }
            }
            createAllRect.x += btnWidth;
            if (GUI.Button(createAllRect, "load"))
            {
                var parent = new GameObject(target.name).GetComponent<Transform>();
                List<GameObject> prefabs = (target as PrefabGroupObj).prefabs;
                for (int i = 0; i < prefabInfosProp.arraySize; i++)
                {
                    var prop = prefabInfosProp.GetArrayElementAtIndex(i);

                    var instenceNameProp = prop.FindPropertyRelative("instenceName");
                    var prefabNameProp = prop.FindPropertyRelative("prefabName");
                    var positionProp = prop.FindPropertyRelative("position");
                    var rotationProp = prop.FindPropertyRelative("rotation");
                    var sizeProp = prop.FindPropertyRelative("size");

                    var prefab = prefabs.Find(x => x.name == prefabNameProp.stringValue);
                    if(prefab != null)
                    {
                        var instence = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                        instence.transform.SetParent(parent, false);
                        instence.name = instenceNameProp.stringValue;
                        instence.transform.position = positionProp.vector3Value;
                        instence.transform.rotation = rotationProp.quaternionValue;
                        instence.transform.localScale = sizeProp.vector3Value;
                    }
                }
            }
        }
        private void UpdateOneInfo(SerializedProperty prop, GameObject instence, GameObject prefab)
        {
            var instenceNameProp = prop.FindPropertyRelative("instenceName");
            var prefabNameProp = prop.FindPropertyRelative("prefabName");
            var positionProp = prop.FindPropertyRelative("position");
            var rotationProp = prop.FindPropertyRelative("rotation");
            var sizeProp = prop.FindPropertyRelative("size");

            if (instence)
            {
                instenceNameProp.stringValue = instence.name;
                positionProp.vector3Value = instence.transform.position;
                rotationProp.quaternionValue = instence.transform.rotation;
                sizeProp.vector3Value = instence.transform.localScale;
            }
            if (prefab) prefabNameProp.stringValue = prefab.name;
        }
        private void DrawPrefabsInfoElment(Rect rect, int index, bool isActive, bool isFocused)
        {
            var prop = prefabInfosProp.GetArrayElementAtIndex(index);
            var instenceNameProp = prop.FindPropertyRelative("instenceName");
            var prefabNameProp = prop.FindPropertyRelative("prefabName");
            var nameRect = new Rect(rect.x, rect.y, rect.width * 0.15f, EditorGUIUtility.singleLineHeight);
            var valueRect = new Rect(rect.x + 0.15f * rect.width, rect.y, rect.width * 0.25f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(nameRect, "name");
            instenceNameProp.stringValue = EditorGUI.TextField(valueRect, instenceNameProp.stringValue);
            nameRect.x += 0.4f * rect.width;
            valueRect.x += 0.4f * rect.width;
            EditorGUI.LabelField(nameRect, "prefab");
            prefabNameProp.stringValue = EditorGUI.TextField(valueRect, prefabNameProp.stringValue);

            var btnRect = new Rect(rect.x + 0.8f * rect.width, rect.y, rect.width * 0.2f, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(btnRect, "update"))
            {
                if (Selection.activeTransform != null)
                {
                    var prefab = PrefabUtility.GetPrefabParent(Selection.activeGameObject);
                    UpdateOneInfo(prop, Selection.activeGameObject as GameObject, prefab as GameObject);
                }
                else
                {
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("温馨提示", "请先选中目标对象后重试", "确定");
                }
            }
        }
        private void DrawPrefabsHeader(Rect rect)
        {
            var titleRect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(titleRect, "预制体列表");
            var createAllRect = new Rect(rect.x + rect.width - 3 * btnWidth, rect.y, btnWidth, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(createAllRect, "clear"))
            {
                var confer = EditorUtility.DisplayDialog("温馨提示", "如需清除所有记录的预制体,请按确认", "确认");
                if (confer)
                {
                    prefabsProp.ClearArray();
                }
            }
            createAllRect.x += btnWidth;
            if (GUI.Button(createAllRect, "open"))
            {
                for (int i = 0; i < prefabsProp.arraySize; i++)
                {
                    var obj = prefabsProp.GetArrayElementAtIndex(i);
                    if (obj.objectReferenceValue != null)
                    {
                        var instenceID = GetInstenceID(obj.objectReferenceValue);
                        ActionEditorUtility.LoadPrefab(obj.objectReferenceValue as GameObject, ref instenceID);
                        SaveInstenceID(obj.objectReferenceValue, instenceID);
                    }
                }
            }
            createAllRect.x += btnWidth;
            if (GUI.Button(createAllRect, "save"))
            {
                for (int i = 0; i < prefabsProp.arraySize; i++)
                {
                    var obj = prefabsProp.GetArrayElementAtIndex(i);
                    if (obj.objectReferenceValue != null)
                    {
                        var instenceID = GetInstenceID(obj.objectReferenceValue);
                        ActionEditorUtility.SavePrefab(ref instenceID);
                        SaveInstenceID(obj.objectReferenceValue, instenceID);
                    }
                }
            }
        }
        private void DrawPrefabElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var btnRect = new Rect(rect.x, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight);
            var titleRect = new Rect(rect.x + btnRect.width * 0.5f, rect.y, btnRect.width * 0.5f, btnRect.height);

            var obj = prefabsProp.GetArrayElementAtIndex(index);
            var instenceID = GetInstenceID(obj.objectReferenceValue);
            var objName = obj.objectReferenceValue == null ? "" : obj.objectReferenceValue.name;
            if (GUI.Button(btnRect, objName, EditorStyles.toolbarDropDown))
            {
                if (obj.objectReferenceValue != null)
                {
                    if (instenceID == 0 || EditorUtility.InstanceIDToObject(instenceID) == null)
                    {
                        ActionEditorUtility.LoadPrefab(obj.objectReferenceValue as GameObject, ref instenceID);
                    }
                    else
                    {
                        ActionEditorUtility.SavePrefab(ref instenceID);
                    }

                }
                SaveInstenceID(obj.objectReferenceValue, instenceID);
            }
            if (instenceID != 0)
            {
                EditorGUI.LabelField(titleRect, instenceID.ToString());
            }

            var objRect = new Rect(rect.x + btnRect.width, rect.y, 60, btnRect.height);
            obj.objectReferenceValue = EditorGUI.ObjectField(objRect, obj.objectReferenceValue, typeof(GameObject), false);
        }
        private int GetInstenceID(Object obj)
        {
            if (obj == null) return 0;

            var instenceID = 0;
            if (catchDic.ContainsKey(obj))
            {
                instenceID = catchDic[obj];
            }
            else
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path))
                {
                    var importer = AssetImporter.GetAtPath(path);
                    int.TryParse(importer.userData, out instenceID);
                    importer.userData = instenceID.ToString();
                    EditorUtility.SetDirty(importer);
                    catchDic.Add(obj, instenceID);
                }
            }
            return instenceID;
        }
        private void SaveInstenceID(Object obj, int instenceID)
        {
            catchDic[obj] = instenceID;
            var path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path))
            {
                var importer = AssetImporter.GetAtPath(path);
                importer.userData = instenceID.ToString();
                EditorUtility.SetDirty(importer);
            }
        }
    }

}