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
        protected SerializedProperty elementGroup_prop;
        protected ReorderListDrawer commands_list = new CommandListDrawer();
       
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
        protected ElementGroupDrawer elementGroupDrawer;
        protected int selected;
        protected const string prefer_selected = "actiongroup_prefer_selected";

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
            
        }
        private void FindPropertys()
        {
            script_prop = serializedObject.FindProperty("m_Script");
            actionCommands_prop = serializedObject.FindProperty("actionCommands");
            elementGroup_prop = serializedObject.FindProperty("elementGroup");
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
        }

 
        private void SwitchDrawLists()
        {
            if (selected == 0)
            {
                commands_list.DoLayoutList();
            }
            else if (selected == 1)
            {
                DrawElementGroupHold();
                DrawElementGroup(); 
            }
        }

        private void DrawElementGroupHold()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight +ActionGUIUtil.padding * 2);
            var innerRect = ActionGUIUtil.DrawBoxRect(rect, "");
            var btnRect = new Rect(innerRect.x, innerRect.y, innerRect.width - ActionGUIUtil.middleButtonWidth, innerRect.height);
            var style = elementGroupDrawer != null && elementGroupDrawer.showAll ? EditorStyles.toolbarPopup : EditorStyles.toolbarDropDown;
            if (GUI.Button(btnRect,"元素列表", style))
            {
                if (elementGroupDrawer != null){
                    elementGroupDrawer.showAll = !elementGroupDrawer.showAll;
                }
            }

            var contentRect = new Rect(innerRect.x + innerRect.width - ActionGUIUtil.middleButtonWidth, innerRect.y, ActionGUIUtil.middleButtonWidth, innerRect.height);
            if (elementGroup_prop.objectReferenceValue == null)
            {
                elementGroup_prop.objectReferenceValue =  EditorGUI.ObjectField(contentRect, elementGroup_prop.objectReferenceValue, typeof(ElementGroup), false);
            }
            else
            {
                ActionGUIUtil.AcceptDrawField(contentRect, elementGroup_prop, typeof(ElementGroup));
            }
        }

        private void DrawElementGroup()
        {
            if (elementGroup_prop.objectReferenceValue == null)
            {
                if (GUILayout.Button("create new element group"))
                {
                    var group = ScriptableObject.CreateInstance<ElementGroup>();
                    ProjectWindowUtil.CreateAsset(group, "new element_group.asset");
                }
            }
            else
            {
                if (elementGroupDrawer == null)
                {
                    Editor editor = null;
                    Editor.CreateCachedEditor(elementGroup_prop.objectReferenceValue, typeof(ElementGroupDrawer), ref editor);
                    elementGroupDrawer = editor as ElementGroupDrawer;
                    elementGroupDrawer.showScript = false;
                }
                elementGroupDrawer.OnInspectorGUI();
            }
        }
    }

}