using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(ActionCommand)), CanEditMultipleObjects]
    public class ActionCommandDrawer : Editor
    {
        public Graph.OperaterNode[] opreateNodes;
        private ActionCommand command { get { return target as ActionCommand; } }
        protected SerializedProperty script_prop;
        protected SerializedProperty commandBindings_prop;
        protected SerializedProperty _stepName_prop;
        protected SerializedProperty environments_prop;
        protected SerializedProperty startHooks_prop;
        protected SerializedProperty completeHooks_prop;

        protected ReorderListDrawer nodeList = new NodeListDrawer();
        protected ReorderListDrawer bindingList = new CommandBindingListDrawer();
        protected ReorderListDrawer enviromentList = new EnviromentInfoListDrawer();
        protected ReorderListDrawer startHooksList = new HookListDrawer("操作对象启动前");
        protected ReorderListDrawer completeHooksList = new HookListDrawer("操作对象完成后");

        private GUIContent[] _options;
        protected GUIContent[] options
        {
            get
            {
                if (_options == null)
                {
                    _options = new GUIContent[] {
                        new GUIContent("操作节点列表"),
                        new GUIContent("功能绑定列表"),
                        new GUIContent("环境控制列表"),
                        new GUIContent("前提支持列表")
                    };
                }
                return _options;
            }
        }
        protected int selected;
        protected const string prefer_selected = "prefer_actioncommand_drawer_selected";
    
        private void OnEnable()
        {
            InitPrefers();
            InitNodes();
            InitProps();
            InitReorderLists();
        }

        public override void OnInspectorGUI()
        {
            ActionGUIUtil.DrawDisableProperty(script_prop);
            serializedObject.Update();
            DrawSwitchToolBar();
            serializedObject.ApplyModifiedProperties();
        }

        private void InitReorderLists()
        {
            InitBindingList();
            InitEnviromentInfoList();
            InitHookLists();
        }

        private void InitPrefers()
        {
            if (EditorPrefs.HasKey(prefer_selected))
            {
                selected = EditorPrefs.GetInt(prefer_selected);
            }
        }

        private void InitProps()
        {
            script_prop = serializedObject.FindProperty("m_Script");
            _stepName_prop = serializedObject.FindProperty("_stepName");
            environments_prop = serializedObject.FindProperty("environments");
            startHooks_prop = serializedObject.FindProperty("startHooks");
            completeHooks_prop = serializedObject.FindProperty("completeHooks");
            commandBindings_prop = serializedObject.FindProperty("commandBindings");
        }

        private void DrawSwitchToolBar()
        {
            EditorGUI.BeginChangeCheck();
            selected = GUILayout.Toolbar(selected, options);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(prefer_selected, selected);
            }
            if (selected == 0)
            {
                DrawNodeList();
            }
            else if (selected == 1)
            {
                DrawBindigList();
            }
            else if (selected == 2)
            {
                DrawEnviromentList();
            }
            else
            {
                DrawHookLists();
            }
        }


        #region Hooks
        private void InitHookLists()
        {
            startHooksList.InitReorderList(startHooks_prop);
            completeHooksList.InitReorderList(completeHooks_prop);
        }

        private void DrawHookLists()
        {
            startHooksList.DoLayoutList();
            completeHooksList.DoLayoutList();
        }

        #endregion

        #region EnviromentInfo
        private void InitEnviromentInfoList()
        {
            enviromentList.InitReorderList(environments_prop);
        }
        private void DrawEnviromentList()
        {
            enviromentList.DoLayoutList();
        }
        #endregion

        #region CommandBinding
        private void InitBindingList()
        {
            bindingList.InitReorderList(commandBindings_prop);
        }
        private void DrawBindigList()
        {
            bindingList.DoLayoutList();
        }
        #endregion

        #region Nodes
        private void InitNodes()
        {
            opreateNodes = (from node in command.Nodes
                            where node.Object is Graph.OperaterNode
                            select node.Object as Graph.OperaterNode).ToArray();
            nodeList.InitReorderList(opreateNodes, typeof(Graph.OperaterNode));
        }
        private void DrawNodeList()
        {
            nodeList.DoLayoutList();
        }
        #endregion

    }
}