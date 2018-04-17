
using UnityEditor;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;

namespace WorldActionSystem
{
    public static class LayoutOption
    {
        public static GUILayoutOption minWidth = GUILayout.Width(20);
        public static GUILayoutOption shortWidth = GUILayout.Width(50);
        public static GUILayoutOption mediaWidth = GUILayout.Width(75);
        public static GUILayoutOption longWidth = GUILayout.Width(100);
        public static GUILayoutOption maxWidth = GUILayout.Width(200);

        public static GUILayoutOption shortHigh = GUILayout.Height(EditorGUIUtility.singleLineHeight);
        public static GUILayoutOption mediaHigh = GUILayout.Height(EditorGUIUtility.singleLineHeight * 2);
        public static GUILayoutOption longHigh = GUILayout.Height(EditorGUIUtility.singleLineHeight * 5);
        public static GUILayoutOption maxHight = GUILayout.Height(EditorGUIUtility.singleLineHeight * 10);
    }
    [CustomEditor(typeof(LinkItem))]
    public class LinkItemDrawer : Editor
    {
        private LinkItem targetItem;
        private LinkPort[] linkPorts;
        private ReorderableList[] portLists;
        private const float buttonWidth = 60;
        private void OnEnable()
        {
            targetItem = target as LinkItem;
            linkPorts = targetItem.ChildNodes.ToArray();
            InitReorderLists();
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            if (GUILayout.Button("Clamp")){
                LinkUtil.Clamp(targetItem.transform);
            }
            DrawLinkPortInfos();
            serializedObject.ApplyModifiedProperties();
        }

        private void InitReorderLists()
        {
            portLists = new ReorderableList[linkPorts.Length];
            for (int i = 0; i < portLists.Length; i++)
            {
                var port = linkPorts[i];
                port.connectAble.Sort();
                
                portLists[i] = new ReorderableList(port.connectAble,typeof(LinkInfo));
                portLists[i].drawHeaderCallback = (rect) => {
                    var nameRect = new Rect(rect.x + 15, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight);
                    var idRect = new Rect(rect.x + rect.width * 0.4f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                    var rangeRect = new Rect(rect.x + rect.width * 0.55f, rect.y, 60, EditorGUIUtility.singleLineHeight);
                    
                    EditorGUI.LabelField(nameRect,string.Format( "目标（{0}:{1}）" , port.name , port.NodeID));
                    EditorGUI.LabelField(idRect, "端口");
                    EditorGUI.BeginChangeCheck();
                    port.Range = GUI.HorizontalSlider(rangeRect, port.Range,0.1f,2);
                    if(EditorGUI.EndChangeCheck())
                    {
                        port.Range = (float)System.Math.Round(port.Range, 2);
                    }
                    var btnRect = new Rect(rect.x + rect.width - buttonWidth * 2f, rect.y, buttonWidth, EditorGUIUtility.singleLineHeight);

                    if (GUI.Button(btnRect,"Record"))
                    {
                        List<LinkPort> otherPorts;
                        if (LinkUtil.FindTriggerNodes(port, out otherPorts))
                        {
                            if(otherPorts != null && otherPorts.Count > 0)
                            {
                                var window = EditorWindow.GetWindow<LinkWindow>();
                                window.InitPortGroup(port, otherPorts.ToArray());
                            }
                            
                        }
                    }
                    btnRect.x += buttonWidth;
                    if (GUI.Button(btnRect,"Link"))
                    {
                        if (Selection.activeGameObject != null)
                        {
                            var linkport = Selection.activeGameObject.GetComponentInParent<LinkPort>();
                            if(linkport != null)
                            {
                                var linkItem = linkport.GetComponentInParent<LinkItem>();
                                var linkInfo = port.connectAble.Find(x => x.itemName == linkItem.Name && x.nodeId == linkport.NodeID);
                                if(linkInfo != null)
                                {
                                    LinkUtil.ResetTargetTranform(targetItem, linkItem, linkInfo.relativePos, linkInfo.relativeDir);
                                }
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("温馨提示", "请选择目标端口后重试", "确认");
                            }
                        }
                    }
                };
                portLists[i].drawElementCallback = (rect, index, isactive, isfoces) =>
                {
                    var linkInfo = port.connectAble[index];
                    var nameRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(nameRect,linkInfo.itemName);
                    var idRect = new Rect(rect.x + rect.width * 0.4f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(idRect, linkInfo.nodeId.ToString());
                    var rangeRect = new Rect(rect.x + rect.width * 0.55f, rect.y, 60, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(rangeRect, port.Range.ToString());
                };
            }
        }

        private void DrawLinkPortInfos()
        {
            foreach (var item in portLists)
            {
                item.DoLayoutList();
            }
        }



    }
}