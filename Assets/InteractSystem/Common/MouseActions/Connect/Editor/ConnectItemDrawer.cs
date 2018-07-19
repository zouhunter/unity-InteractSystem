using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using InteractSystem.Actions;
using UnityEditorInternal;
using System;

namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(ConnectItem))]
    public class ConnectItemDrawer : ActionItemDrawer
    {
        [InitializeOnLoadMethod]
        static void ImportLayer()
        {
            LayerTool.ImportLayers(ConnectItem.layer);
        }

        private ReorderableList groupList;
        private ReorderableList nodeList;
        private SerializedProperty connectGroup_prop;
        private SerializedProperty nodes_prop;
        protected override void OnEnable()
        {
            base.OnEnable();
            InitNodeList();
            InitGroupList();
        }
        protected void InitNodeList()
        {
            nodes_prop = serializedObject.FindProperty("nodes");
            nodeList = new ReorderableList(serializedObject, nodes_prop);
            nodeList.elementHeight = EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 2f;
            nodeList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect,"连接节点列表");
            };
            nodeList.drawElementCallback = DrawNodeElement;
        }

   

        protected void InitGroupList()
        {
            connectGroup_prop = serializedObject.FindProperty("connectGroup");
            groupList = new ReorderableList(serializedObject, connectGroup_prop);
            groupList.elementHeight = EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 2f;
            groupList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "连接组合列表");
            };
            groupList.drawElementCallback = DrawGroupItem;
        }

        private void DrawNodeElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, "");
            var prop = nodes_prop.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop, new GUIContent("第" + index + "个节点"));
        }


        private void DrawGroupItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, "");
            var prop = connectGroup_prop.GetArrayElementAtIndex(index);
            var prop_1 = prop.FindPropertyRelative("p1");
            var prop_2 = prop.FindPropertyRelative("p2");
            var rect0 = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect0, new GUIContent("第" + index + "组"));

            var rect1 = new Rect(rect.x + rect.width * 0.3f, rect.y, rect.width * 0.35f, EditorGUIUtility.singleLineHeight);
            prop_1.intValue = EditorGUI.IntField(rect1, prop_1.intValue);
            rect1.x += rect.width * 0.35f;
            prop_2.intValue = EditorGUI.IntField(rect1, prop_2.intValue);
        }

        protected override void OnDrawProperty(SerializedProperty property)
        {
            if (property.propertyPath == "connectGroup")
            {
                groupList.DoLayoutList();
            }
            else if (property.propertyPath == "nodes")
            {
                nodeList.DoLayoutList();
            }
            else
            {
                base.OnDrawProperty(property);
            }
        }
    }

}