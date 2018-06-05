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
        public Graph.OperateNode[] opreateNodes;
        private ActionCommand command { get { return target as ActionCommand; } }
        protected SerializedProperty script_prop;
        protected SerializedProperty commandBindings_prop;
        protected SerializedProperty _stepName_prop;
        protected SerializedProperty environments_prop;
        protected SerializedProperty startHooks_prop;
        protected SerializedProperty completeHooks_prop;

        protected ReorderableList nodeList;
        protected ReorderableList bindingList;
        protected ReorderableList enviromentList;

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
        private List<Type> _commandBindingTypes;
        protected List<Type> commandBindingTypes
        {
            get
            {
                if (_commandBindingTypes == null || _commandBindingTypes.Count == 0)
                {
                    _commandBindingTypes = typeof(ActionGroup).Assembly.GetTypes().
                        Where(x => x.IsSubclassOf(typeof(Binding.CommandBinding))).ToList();
                }
                return _commandBindingTypes;
            }

        }

        private List<Binding.CommandBinding> dragBindings = new List<Binding.CommandBinding>();
        private void OnEnable()
        {
            InitPrefers();
            InitNodes();
            InitProps();
            InitReorderLists();
        }

        public override void OnInspectorGUI()
        {
            DrawScirpt();
            serializedObject.Update();
            DrawSwitchToolBar();
            serializedObject.ApplyModifiedProperties();
        }

        private void InitReorderLists()
        {
            InitBindingList();
            InitEnviromentInfoList();
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

        #region EnviromentInfo

        private void InitEnviromentInfoList()
        {
            enviromentList = new ReorderableList(serializedObject, environments_prop);
            enviromentList.elementHeightCallback = (index) => {
                var prop = environments_prop.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(prop,null,true) + ActionGUIUtil.span * 2;
            };
            enviromentList.drawElementCallback = DrawEnviromentItem;
        }

        private void DrawEnviromentItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var prop = environments_prop.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect,prop, null, true);
        }

        private void DrawEnviromentList()
        {
            enviromentList.DoLayoutList();
        }

        #endregion

        #region CommandBinding
        private void InitBindingList()
        {
            bindingList = new ReorderableList(serializedObject, commandBindings_prop);
            bindingList.drawElementCallback = DrawBindingItem;
            bindingList.elementHeight = EditorGUIUtility.singleLineHeight + ActionGUIUtil.span * 2;
            bindingList.drawHeaderCallback = DrawBindigHeader;
            //bindingList.onAddCallback = OnAddBindingItem;
        }

        private void DrawBindigHeader(Rect rect)
        {
            var btnRect = new Rect(rect.x + rect.width - ActionGUIUtil.bigButtonWidth, rect.y, ActionGUIUtil.bigButtonWidth, rect.height);
            if (GUI.Button(btnRect, "new", EditorStyles.miniButtonRight))
            {
                OnAddBindingItem();
            }
        }

        private void OnAddBindingItem()
        {
            var options = commandBindingTypes.ConvertAll(x => new GUIContent(x.FullName)).ToArray();
            Debug.Log(options.Length);
            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), options, -1, (data, ops, s) =>
            {
                if (s >= 0)
                {
                    var type = commandBindingTypes[s];
                    var asset = ScriptableObject.CreateInstance(type);
                    ProjectWindowUtil.CreateAsset(asset, "new_" + type.Name + ".asset");
                }
            }, null);
        }

        private void DrawBindingItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var prop = commandBindings_prop.GetArrayElementAtIndex(index);
            var content = prop.objectReferenceValue == null ? new GUIContent("Null") : new GUIContent(prop.objectReferenceValue.GetType().Name);
            EditorGUI.PropertyField(rect, prop, content);
        }

        private void DrawBindigList()
        {
            bindingList.DoLayoutList();
            var rect = ActionGUIUtil.GetDragRect();

            if (Event.current.type == EventType.dragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".asset", dragBindings);
            }
            else if (Event.current.type == EventType.dragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragBindings)
                {
                    commandBindings_prop.InsertArrayElementAtIndex(commandBindings_prop.arraySize);
                    var prop = commandBindings_prop.GetArrayElementAtIndex(commandBindings_prop.arraySize - 1);
                    prop.objectReferenceValue = item;
                }
            }
        }
        #endregion

        #region Nodes
        private void InitNodes()
        {
            opreateNodes = (from node in command.Nodes
                            where node.Object is Graph.OperateNode
                            select node.Object as Graph.OperateNode).ToArray();

            nodeList = new ReorderableList(opreateNodes, typeof(Graph.OperateNode), true, true, false, false);
            nodeList.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, ""); };
            nodeList.drawElementCallback = DrawNodeItem;
            nodeList.elementHeight = EditorGUIUtility.singleLineHeight + ActionGUIUtil.span * 2;
        }

        private void DrawNodeItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var item = opreateNodes[index];
            EditorGUI.ObjectField(rect, item.Name, item, typeof(Graph.OperateNode), false);
            if (isActive)
            {
                var editor = Editor.CreateEditor(item);
                editor.OnInspectorGUI();
            }
        }
        private void DrawNodeList()
        {
            nodeList.DoLayoutList();
        }
        #endregion

        private void DrawScirpt()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(script_prop);
            EditorGUI.EndDisabledGroup();
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
        }

     
    }
}