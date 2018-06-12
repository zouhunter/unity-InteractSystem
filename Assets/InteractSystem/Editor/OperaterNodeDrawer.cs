using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(Graph.OperaterNode), true)]
    public class OperateNodeDrawer : Editor
    {
        private Graph.OperaterNode node { get { return target as Graph.OperaterNode; } }

        protected SerializedProperty script_prop;
        protected SerializedProperty bindings_prop;
        protected SerializedProperty _name_prop;
        protected SerializedProperty environments_prop;
        protected SerializedProperty startHooks_prop;
        protected SerializedProperty completeHooks_prop;

        protected ReorderListDrawer bindingList = new OperaterBindingListDrawer();
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
                        new GUIContent("详情配制"),
                        new GUIContent("功能绑定"),
                        new GUIContent("环境控制"),
                        new GUIContent("前提支持")
                    };
                }
                return _options;
            }
        }
        protected int selected;
        protected const string prefer_selected = "prefer_operate_drawer_selected";
        protected List<string> ignoredPaths = new List<string> {
            "m_Script","bindings","_name","environments","startHooks","completeHooks"
        };

        protected virtual void OnEnable()
        {
            if (target == null)
            {
                Debug.LogError("no target !!!");
                DestroyImmediate(this);
            }

            InitPrefers();
            InitPropertys();
            InitDrawers();
        }

        public override void OnInspectorGUI()
        {
            ActionGUIUtil.DrawDisableProperty(script_prop);
            serializedObject.Update();
            EditorGUILayout.PropertyField(_name_prop);
            DrawSwitch();
            serializedObject.ApplyModifiedProperties();
        }


        protected virtual void InitPrefers()
        {
            if (EditorPrefs.HasKey(prefer_selected))
            {
                selected = EditorPrefs.GetInt(prefer_selected);
            }
        }

        protected virtual void InitPropertys()
        {
            script_prop = serializedObject.FindProperty("m_Script");
            bindings_prop = serializedObject.FindProperty("bindings");
            _name_prop = serializedObject.FindProperty("_name");
            environments_prop = serializedObject.FindProperty("environments");
            startHooks_prop = serializedObject.FindProperty("startHooks");
            completeHooks_prop = serializedObject.FindProperty("completeHooks");
        }

        protected virtual void InitDrawers()
        {
            bindingList.InitReorderList(bindings_prop);
            enviromentList.InitReorderList(environments_prop);
            startHooksList.InitReorderList(startHooks_prop);
            completeHooksList.InitReorderList(completeHooks_prop);
        }

        protected virtual void DrawSwitch()
        {
            EditorGUI.BeginChangeCheck();
            selected = GUILayout.Toolbar(selected, options);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(prefer_selected, selected);
            }

            if (selected == 0)
            {
                OnDrawDefult();
            }
            else if (selected == 1)
            {
                bindingList.DoLayoutList();
            }
            else if (selected == 2)
            {
                enviromentList.DoLayoutList();
            }
            else
            {
                startHooksList.DoLayoutList();
                completeHooksList.DoLayoutList();
            }
        }
        protected virtual void OnDrawDefult()
        {
            var iterator = serializedObject.GetIterator();
            var enterChildern = true;
            while (iterator.NextVisible(enterChildern))
            {
                if (!ignoredPaths.Contains(iterator.propertyPath)){
                    EditorGUILayout.PropertyField(iterator,true);
                }
                enterChildern = false;
            }
        }

    }
}