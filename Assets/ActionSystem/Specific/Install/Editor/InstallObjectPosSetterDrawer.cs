using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(InstallObjectPosSetter))]
public class InstallObjectPosSetterDrawer : Editor
{
    public SerializedProperty switchList;
    public SerializedProperty objectList;
    string activeName;
    int _sremoveindex;
    int _oremoveindex;
    void OnEnable()
    {
        switchList = serializedObject.FindProperty("switchList");
        objectList = serializedObject.FindProperty("objectList");
    }
    protected override void OnHeaderGUI()
    {
        base.OnHeaderGUI();
    }

    public override void OnInspectorGUI()
    {
        DrawSwithAbleHeader();
        DrawSwitchAbleKeys();
        DrawGameObjectsHeader();
        DrawGameObjectList();
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawGameObjectsHeader()
    {
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.blue;
        EditorGUILayout.SelectableLabel("2.坐标受控制的对象");
        EditorGUILayout.SelectableLabel("-->当前连接：-->");
        GUI.backgroundColor = Color.red;
        EditorGUILayout.SelectableLabel(activeName);
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            int idex = objectList.arraySize - 1;
            idex = idex >= 0 ? idex : 0;
            objectList.InsertArrayElementAtIndex(idex);
            OnInsetAnObject(idex);
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGameObjectList()
    {
        _oremoveindex = -1;

        var item = default(SerializedProperty);

        for (int i = 0; i < objectList.arraySize; i++)
        {
            item = objectList.GetArrayElementAtIndex(i);

            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(item,new GUIContent("id:" + i));

                GUI.enabled = i != 0;

                if (GUILayout.Button("^", GUILayout.Width(20)))
                {
                    objectList.MoveArrayElement(i, i - 1);
                    OnChangeObjectIndex(i, i - 1);
                }

                GUI.enabled = i < objectList.arraySize - 1;

                if (GUILayout.Button("v", GUILayout.Width(20)))
                {
                    objectList.MoveArrayElement(i, i + 1);
                    OnChangeObjectIndex(i, i + 1);
                }

                GUI.enabled = true;
                if (GUILayout.Button("record", GUILayout.Width(60)))
                {
                    UpdateObjectPos(i);
                }
                if (GUILayout.Button("reset",GUILayout.Width(60)))
                {
                    UseOldPos(i);
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    _oremoveindex = i;
                }
            }
        }

        if (_oremoveindex != -1)
        {
            objectList.GetArrayElementAtIndex(_oremoveindex).objectReferenceValue = null;
            objectList.DeleteArrayElementAtIndex(_oremoveindex);
            OnRemoveAnObject(_oremoveindex);
            _oremoveindex = -1;
        }
    }

    void DrawSwithAbleHeader()
    {
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.blue;
        EditorGUILayout.SelectableLabel("1.实验关键字列表");

        if (GUILayout.Button("Record", GUILayout.Width(60)))
        {
            if (string.IsNullOrEmpty(activeName))
            {
                EditorUtility.DisplayDialog("未联接", "先将信息与对象连接", "OK");
            }
            else if (EditorUtility.DisplayDialog("确认", "是否要记录新的坐标信息到对象列表", "执行"))
            {
                for (int i = 0; i < switchList.arraySize; i++)
                {
                    var item = switchList.GetArrayElementAtIndex(i);
                    if (item.FindPropertyRelative("key").stringValue == activeName)
                    {
                        RecordTransformsPositions(item);
                        break;
                    }
                }
            }
        }
        if (GUILayout.Button("Load", GUILayout.Width(60)))
        {
            if (string.IsNullOrEmpty(activeName))
            {
                EditorUtility.DisplayDialog("未联接", "先将信息与对象连接", "OK");
            }
            else if (EditorUtility.DisplayDialog("确认", "是否加载" + activeName + "状态下保存的坐标等信息", "执行"))
            {
                for (int i = 0; i < switchList.arraySize; i++)
                {
                    var item = switchList.GetArrayElementAtIndex(i);
                    if (item.FindPropertyRelative("key").stringValue == activeName)
                    {
                        UnRecordTransformPosition(item);
                        break;
                    }
                }
            }
        }
        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            int idex = switchList.arraySize - 1;
            idex = idex >= 0 ? idex : 0;
            switchList.InsertArrayElementAtIndex(idex);
            OnInsetNewState(idex);
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    void DrawSwitchAbleKeys()
    {
        _sremoveindex = -1;

        var item = default(SerializedProperty);

        for (int i = 0; i < switchList.arraySize; i++)
        {
            item = switchList.GetArrayElementAtIndex(i);

            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                var key = item.FindPropertyRelative("key");
                EditorGUILayout.PropertyField(item,new GUIContent(key.stringValue),true);

                GUI.enabled = i != 0;

                if (GUILayout.Button("^", GUILayout.Width(20)))
                {
                    switchList.MoveArrayElement(i, i - 1);
                }

                GUI.enabled = i < switchList.arraySize - 1;

                if (GUILayout.Button("v", GUILayout.Width(20)))
                {
                    switchList.MoveArrayElement(i, i + 1);
                }
                GUI.enabled = true;
                
                if (GUILayout.Button("Connect", GUILayout.Width(60)))
                {
                     activeName = key.stringValue;
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    _sremoveindex = i;
                }
            }
        }

        if (_sremoveindex != -1)
        {
            switchList.DeleteArrayElementAtIndex(_sremoveindex);
            _sremoveindex = -1;
        }
        serializedObject.ApplyModifiedProperties();
    }

    void UpdateObjectPos(int index)
    {
        if (string.IsNullOrEmpty(activeName))
        {
            EditorUtility.DisplayDialog("未关联","请先将对象与实验关键字进行关联","ok");
            return;
        }
        for (int i = 0; i < switchList.arraySize; i++)
        {
            var sitem = switchList.GetArrayElementAtIndex(i);
            string key = sitem.FindPropertyRelative("key").stringValue;
            Transform item = (Transform)objectList.GetArrayElementAtIndex(index).objectReferenceValue;
            if (item == null) return;
            if (key == activeName)
            {
                var transforms = sitem.FindPropertyRelative("objTransforms");

                var objitem = transforms.GetArrayElementAtIndex(index);
                var posProp = objitem.FindPropertyRelative("position");
                var eularProp = objitem.FindPropertyRelative("eular");
                var sizeProp = objitem.FindPropertyRelative("size");

                posProp.vector3Value = item.localPosition;
                eularProp.vector3Value = item.localEulerAngles;
                sizeProp.vector3Value = item.localScale;
                break;
            }
        }
    }

    void UseOldPos(int index)
    {
        if (string.IsNullOrEmpty(activeName))
        {
            EditorUtility.DisplayDialog("未关联", "请先将对象与实验关键字进行关联", "ok");
            return;
        }
        for (int i = 0; i < switchList.arraySize; i++)
        {
            var sitem = switchList.GetArrayElementAtIndex(i);
            string key = sitem.FindPropertyRelative("key").stringValue;
            Transform item = (Transform)objectList.GetArrayElementAtIndex(index).objectReferenceValue;
            if (item == null) return;
            if (key == activeName)
            {
                var transforms = sitem.FindPropertyRelative("objTransforms");

                var objitem = transforms.GetArrayElementAtIndex(index);
                var posProp = objitem.FindPropertyRelative("position");
                var eularProp = objitem.FindPropertyRelative("eular");
                var sizeProp = objitem.FindPropertyRelative("size");

                item.localPosition = posProp.vector3Value;
                item.localEulerAngles = eularProp.vector3Value;
                item.localScale = sizeProp.vector3Value;
                break;
            }
        }
       
    }

    void RecordTransformsPositions(SerializedProperty item)
    {
        var transList = item.FindPropertyRelative("objTransforms");
        for (int i = 0; i < objectList.arraySize; i++)
        {
            var transDataProp = transList.GetArrayElementAtIndex(i);
            var transProp = objectList.GetArrayElementAtIndex(i);
            if (transProp == null)
            {
                Debug.LogError(i + "对象为空");
                continue;
            }
            var trans = (Transform)transProp.objectReferenceValue;
           
            var posProp = transDataProp.FindPropertyRelative("position");
            var eularProp = transDataProp.FindPropertyRelative("eular");
            var sizeProp = transDataProp.FindPropertyRelative("size");

            posProp.vector3Value = trans.localPosition;
            eularProp.vector3Value = trans.localEulerAngles;
            sizeProp.vector3Value = trans.localScale;
        }
    }

    void UnRecordTransformPosition(SerializedProperty item)
    {
        var transList = item.FindPropertyRelative("objTransforms");
        for (int i = 0; i < transList.arraySize; i++)
        {
            var transDataProp = transList.GetArrayElementAtIndex(i);
            var transProp = objectList.GetArrayElementAtIndex(i);
            if (transProp == null)
            {
                Debug.LogError(i + "对象为空");
                continue;
            }
            var trans = (Transform)transProp.objectReferenceValue;

            var posProp = transDataProp.FindPropertyRelative("position");
            var eularProp = transDataProp.FindPropertyRelative("eular");
            var sizeProp = transDataProp.FindPropertyRelative("size");

            trans.localPosition = posProp.vector3Value;
            trans.localEulerAngles = eularProp.vector3Value;
            trans.localScale = sizeProp.vector3Value;
        }
    }

    //上下移动对象
    void OnChangeObjectIndex(int index,int newIndex)
    {
        for (int i = 0; i < switchList.arraySize; i++)
        {
            var transProp = switchList.GetArrayElementAtIndex(i).FindPropertyRelative("objTransforms");
            transProp.MoveArrayElement(index, newIndex);
        }
    }

    //插入对象
    void OnInsetAnObject(int index)
    {
        for (int i = 0; i < switchList.arraySize; i++)
        {
            var transProp = switchList.GetArrayElementAtIndex(i).FindPropertyRelative("objTransforms");
            transProp.InsertArrayElementAtIndex(index);
        }
    }

    //移除对象
    void OnRemoveAnObject(int index)
    {
        for (int i = 0; i < switchList.arraySize; i++)
        {
            var transProp = switchList.GetArrayElementAtIndex(i).FindPropertyRelative("objTransforms");
            transProp.DeleteArrayElementAtIndex(index);
        }
    }
    //插入新的状态
    void OnInsetNewState(int index)
    {

    }
}
