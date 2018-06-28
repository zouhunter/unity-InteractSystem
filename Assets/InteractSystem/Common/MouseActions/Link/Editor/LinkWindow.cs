using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace InteractSystem.Actions
{

    public class LinkWindow : EditorWindow
    {
        private LinkPort port;
        private LinkPort[] otherPorts;
        private bool[] selection;
        private ReorderableList reorderList;

        public void InitPortGroup(LinkPort port, LinkPort[] otherPorts)
        {
            this.port = port;
            this.otherPorts = otherPorts;
            selection = new bool[otherPorts.Length];
            InitReorderList();
        }

        private void InitReorderList()
        {
            reorderList = new ReorderableList(otherPorts, typeof(LinkPort));
            reorderList.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(rect, "可连接列表");
            };
            reorderList.drawElementCallback += DrawElements;
        }

        private void DrawElements(Rect rect, int i, bool isActive, bool isFocused)
        {
            var togRect = new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight);
            selection[i] = EditorGUI.Toggle(togRect, selection[i]);

            var labelRect = new Rect(rect.x + 30, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, "A" + string.Format("  {0}:{1}", port.Body.Name, port.name));

            labelRect.x += rect.width * 0.4f;
            var otherPort = otherPorts[i];
            EditorGUI.LabelField(labelRect, "B" + i + string.Format("  {0}:{1}", otherPort.Body.Name, otherPort.name));
        }

        private void OnGUI()
        {
            if (reorderList != null && otherPorts != null)
                reorderList.DoLayoutList();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Link"))
            {
                for (int i = 0; i < otherPorts.Length; i++)
                {
                    if(selection[i])
                    {
                        TryRecordConnect(port, otherPorts[i]);
                    }
                }
            }
        }

        public static void TryRecordConnect(LinkPort node_A, LinkPort node_B)
        {
            if (!node_A || !node_B) return;
            LinkItem item_A = node_A.GetComponentInParent<LinkItem>();
            LinkItem item_B = node_B.GetComponentInParent<LinkItem>();

            if (node_A == null || node_B == null || item_A == null || item_B == null)
            {
                return;
            }

            var confer = EditorUtility.DisplayDialog("[connected]", item_A.Name + ":" + (node_A.name) + "<->" + item_B.Name + ":" + (node_B.name), "确认");
            if (confer)
            {
                LinkInfo nodeArecored = node_A.connectAble.Find((x) => x.itemName == item_B.name && x.nodeId == node_B.NodeID);
                LinkInfo nodeBrecored = node_B.connectAble.Find((x) => x.itemName == item_A.name && x.nodeId == node_A.NodeID);
                //已经记录过
                if (nodeArecored == null)
                {
                    nodeArecored = new LinkInfo();
                    node_A.connectAble.Add(nodeArecored);
                }
                if (nodeBrecored == null)
                {
                    nodeBrecored = new LinkInfo();
                    node_B.connectAble.Add(nodeBrecored);
                }

                nodeArecored.itemName = item_B.Name;
                nodeBrecored.itemName = item_A.Name;
                nodeArecored.nodeId = node_B.NodeID;
                nodeBrecored.nodeId = node_A.NodeID;
                LinkUtil.RecordTransform(nodeArecored, nodeBrecored, item_A.transform, item_B.transform);
                EditorUtility.SetDirty(node_A);
                EditorUtility.SetDirty(node_B);
            }

        }
    }

}