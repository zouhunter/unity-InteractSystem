using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(ActionGroup))]
    public class ActionGroupDrawer : Editor
    {
        protected SerializedProperty script_prop;
        protected SerializedProperty actionCommands_prop;
        protected SerializedProperty runtimeElements_prop;
        protected SerializedProperty autoElements_prop;
        protected SerializedProperty enviroments_prop;

        protected ReorderListDrawer commands_list = new CommandListDrawer();
        protected ReorderListDrawer autoElemnts_list = new AutoElementListDrawer();
        protected ReorderListDrawer runtimeElements_list = new RuntimeElementListDrawer();
        protected ReorderListDrawer enviroments_list = new EnviromentItemListDrawer();

        protected GUIContent[] _selectables;
        protected GUIContent[] Selectables
        {
            get
            {
                if (_selectables == null)
                {
                    _selectables = new GUIContent[] {
                        new GUIContent("步骤列表"),
                        new GUIContent("元素列表")
                    };
                }
                return _selectables;
            }
        }
        protected GUIContent[] _secondSelectables;
        protected GUIContent[] SecondSelectables
        {
            get
            {
                if (_secondSelectables == null || _secondSelectables.Length == 0)
                {
                    _secondSelectables = new GUIContent[] {
                        new GUIContent("静态生成元素"),
                        new GUIContent("动态创建元素"),
                        new GUIContent("环境切换元素"),
                    };
                }
                return _secondSelectables;
            }
        }
        protected int selected;
        protected int secondSelected;


        protected const string prefer_selected = "actiongroup_prefer_selected";
        protected const string prefer_second_selected = "actiongroup_prefer_second_selected";

        private void OnEnable()
        {
            FindPropertys();
            InitReorderLists();
            InitSelected();
        }

        public override void OnInspectorGUI()
        {
            ActionGUIUtil.DrawDisableProperty(script_prop);
            serializedObject.Update();
            DrawSwitchOptions();
            SwitchDrawLists();
            serializedObject.ApplyModifiedProperties();
        }

        private void InitSelected()
        {
            if (EditorPrefs.HasKey(prefer_selected))
            {
                selected = EditorPrefs.GetInt(prefer_selected);
            }
            if (EditorPrefs.HasKey(prefer_second_selected))
            {
                secondSelected = EditorPrefs.GetInt(prefer_second_selected);
            }
        }
        private void FindPropertys()
        {
            script_prop = serializedObject.FindProperty("m_Script");
            actionCommands_prop = serializedObject.FindProperty("actionCommands");
            runtimeElements_prop = serializedObject.FindProperty("runTimeElements");
            autoElements_prop = serializedObject.FindProperty("autoElements");
            enviroments_prop = serializedObject.FindProperty("enviroments");
        }
        private void DrawSwitchOptions()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 2f);
            rect = ActionGUIUtil.DrawBoxRect(rect, "");
            var searchRect = new Rect(rect.x, rect.y, rect.width * 0.55f, rect.height);
            ActionGUIUtil.searchWord = EditorGUI.TextField(searchRect, ActionGUIUtil.searchWord);

            var toolBarRect = new Rect(rect.x + rect.width * 0.6f, rect.y, rect.width * 0.4f, rect.height);
            EditorGUI.BeginChangeCheck();
            selected = GUI.Toolbar(toolBarRect, selected, Selectables);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(prefer_selected, selected);
            }
        }


        private void InitReorderLists()
        {
            commands_list.InitReorderList(actionCommands_prop);

            autoElemnts_list.InitReorderList(autoElements_prop);
            autoElemnts_list.drawHeaderCallback = DrawHeadSwitch;

            runtimeElements_list.InitReorderList(runtimeElements_prop);
            runtimeElements_list.drawHeaderCallback = DrawHeadSwitch;

            enviroments_list.InitReorderList(enviroments_prop);
            enviroments_list.drawHeaderCallback = DrawHeadSwitch;
        }

        private void DrawHeadSwitch(Rect rect)
        {
            var headRect = new Rect(rect.x, rect.y, rect.width * 0.3f, rect.height);
            EditorGUI.BeginChangeCheck();
            GUI.contentColor = ActionGUIUtil.WarningColor;
            secondSelected = EditorGUI.Popup(headRect, secondSelected, SecondSelectables, EditorStyles.miniLabel);
            GUI.contentColor = Color.white;
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(prefer_second_selected, secondSelected);
            }
        }
        private void SwitchDrawLists()
        {
            if (selected == 0)
            {
                commands_list.DoLayoutList();
            }
            else if (selected == 1)
            {
                if (secondSelected == 0)
                {
                    autoElemnts_list.DoLayoutList();
                }
                else if (secondSelected == 1)
                {
                    runtimeElements_list.DoLayoutList();
                }
                else
                {
                    enviroments_list.DoLayoutList();
                }
            }
        }

    }

}