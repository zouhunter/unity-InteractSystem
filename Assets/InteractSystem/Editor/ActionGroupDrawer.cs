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

        protected ReorderableList commands_list;
        protected ReorderableList autoElemnts_list;
        protected ReorderableList runtimeElements_list;
        protected ReorderableList enviroments_list;
        
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
                if(_secondSelectables == null || _secondSelectables.Length == 0)
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
        protected GUIContent _commandContent;
        protected GUIContent commandContent
        {
            get
            {
                if (_commandContent == null || _commandContent.image == null)
                {
                    var guid = "cce58f60250df8146a074414b8ad53a0";
                    var texture = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(guid));
                    _commandContent = new GUIContent(texture);
                }
                return _commandContent;
            }
        }
    
        protected List<ActionCommand> dragedCommands = new List<ActionCommand>();
        protected List<GameObject> dragedGameObject = new List<GameObject>();
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
            DrawScript();
            serializedObject.Update();
            DrawSwitchOptions();
            SwitchDrawLists();
            serializedObject.ApplyModifiedProperties();
        }
        private void DrawScript()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(script_prop);
            EditorGUI.EndDisabledGroup();
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
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight + ActionGUIUtil.span * 2f);
            rect = DrawBoxRect(rect,"");
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
            InitCommandReorderList();
            InitAutoElementsList();
            InitRuntimeElementsList();
            InitEnviromentsList();
        }


        #region AutoElements
        private void InitAutoElementsList()
        {
            autoElemnts_list = new ReorderableList(serializedObject, autoElements_prop);
            autoElemnts_list.drawHeaderCallback = DrawAutoElementsHead;
            autoElemnts_list.elementHeightCallback = (index) =>
            {
                var prop = autoElements_prop.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(prop, null, true) + ActionGUIUtil.span * 2;
            };
            autoElemnts_list.drawElementCallback = DrawAutoElementItem;
        }

        private void DrawAutoElementsHead(Rect rect)
        {
            DrawHeadSwitch(rect);
        }

        private void DrawAutoElementItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = DrawBoxRect(rect, index.ToString());
            var prop = autoElements_prop.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop);
        }

        private void DrawAutoElementList()
        {
            autoElemnts_list.DoLayoutList();
            var rect = GetDragRect();
            if (Event.current.type == EventType.dragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".prefab", dragedGameObject);  
            }
            else if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragedGameObject)
                {
                    if (!HaveElement(autoElements_prop, item))
                    {
                        autoElements_prop.InsertArrayElementAtIndex(autoElements_prop.arraySize);
                        var prop = autoElements_prop.GetArrayElementAtIndex(autoElements_prop.arraySize - 1);
                        var prefabProp = prop.FindPropertyRelative("prefab");
                        prefabProp.objectReferenceValue = item;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("警告", "预制体重复,无法添加:" + item.name, "ok");
                    }
                }
            }
        }
        #endregion

        #region RuntimeElements
        private void InitRuntimeElementsList()
        {
            runtimeElements_list = new ReorderableList(serializedObject, runtimeElements_prop);
            runtimeElements_list.drawHeaderCallback = DrawRuntimeElementsHead;
            runtimeElements_list.drawElementCallback = DrawRuntimeElementItem;
            runtimeElements_list.elementHeightCallback = (index) =>
            {
                var prop = runtimeElements_prop.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(prop, null, true) + ActionGUIUtil.span * 2;
            };
        }

        private void DrawRuntimeElementsHead(Rect rect)
        {
            DrawHeadSwitch(rect);
        }

        private void DrawRuntimeElementItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = DrawBoxRect(rect, index.ToString());
            var prop = runtimeElements_prop.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop, true);
        }
        private void DrawRuntimeElementList()
        {
            runtimeElements_list.DoLayoutList();
            var rect = GetDragRect();
            if (Event.current.type == EventType.dragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".prefab", dragedGameObject);
            }
            else if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragedGameObject)
                {
                    if (!HaveElement(runtimeElements_prop,item))
                    {
                        runtimeElements_prop.InsertArrayElementAtIndex(runtimeElements_prop.arraySize);
                        var prop = runtimeElements_prop.GetArrayElementAtIndex(runtimeElements_prop.arraySize - 1);
                        var prefabProp = prop.FindPropertyRelative("prefab");
                        prefabProp.objectReferenceValue = item;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("警告", "预制体重复,无法添加:" + item.name, "ok");
                    }
                }
            }
        }

        #endregion

        #region Enviroments

        private void InitEnviromentsList()
        {
            enviroments_list = new ReorderableList(serializedObject, enviroments_prop);
            enviroments_list.drawHeaderCallback = DrawEnviromentsHead;
            enviroments_list.drawElementCallback = DrawEnviromentElement;
            enviroments_list.elementHeightCallback = (index) =>
            {
                var prop = enviroments_prop.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(prop, null, true) + ActionGUIUtil.span * 2;
            };
        }

        private void DrawEnviromentsHead(Rect rect)
        {
            DrawHeadSwitch(rect);
        }

        private void DrawEnviromentElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = DrawBoxRect(rect, index.ToString());
            var prop = enviroments_prop.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop, true);

            var objRect = new Rect(rect.x + rect.width - ActionGUIUtil.bigButtonWidth, rect.y, ActionGUIUtil.bigButtonWidth, rect.height);

            if (Event.current.type == EventType.DragUpdated && objRect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".prefab", dragedGameObject);
            }
            else if (Event.current.type == EventType.DragPerform && objRect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragedGameObject)
                {
                    if (!HaveElement(enviroments_prop, item))
                    {
                        enviroments_prop.InsertArrayElementAtIndex(enviroments_prop.arraySize);
                        var eprop = enviroments_prop.GetArrayElementAtIndex(enviroments_prop.arraySize - 1);
                        var prefabProp = eprop.FindPropertyRelative("prefab");
                        prefabProp.objectReferenceValue = item;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("警告", "预制体重复,无法添加:" + item.name, "ok");
                    }
                }
            }
        }

        private void DrawEnviromentList()
        {
            enviroments_list.DoLayoutList();
            var rect = GetDragRect();
            if (Event.current.type == EventType.dragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".prefab", dragedGameObject);
            }
            else if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragedGameObject)
                {
                    if (!HaveElement(enviroments_prop, item))
                    {
                        enviroments_prop.InsertArrayElementAtIndex(enviroments_prop.arraySize);
                        var prop = enviroments_prop.GetArrayElementAtIndex(enviroments_prop.arraySize - 1);
                        var prefabProp = prop.FindPropertyRelative("prefab");
                        var _name_prop = prop.FindPropertyRelative("_name");
                        _name_prop.stringValue = item.name;
                        prefabProp.objectReferenceValue = item;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("警告", "预制体重复,无法添加:" + item.name, "ok");
                    }
                }
            }
        }

        private bool HaveElement(SerializedProperty arryProp, GameObject prefab)
        {
            for (int i = 0; i < arryProp.arraySize; i++)
            {
                var prop = arryProp.GetArrayElementAtIndex(i);
                var prefab_prop = prop.FindPropertyRelative("prefab");
                if (prefab_prop.objectReferenceValue == prefab)
                {
                    Debug.Log(prefab_prop.objectReferenceValue);
                    Debug.Log(prefab);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region CommandList
        private void InitCommandReorderList()
        {
            commands_list = new ReorderableList(serializedObject, actionCommands_prop, true, true, true, true);
            commands_list.drawHeaderCallback = DrawCommandHead;
            commands_list.elementHeightCallback = CommandElementHeight;
            commands_list.drawElementCallback = DrawCommandItem;
        }

        private void DrawCommandHead(Rect rect)
        {
            var btnRect = new Rect(rect.x + rect.width - ActionGUIUtil.middleButtonWidth - ActionGUIUtil.span, rect.y, ActionGUIUtil.middleButtonWidth, rect.height);
            if (GUI.Button(btnRect, new GUIContent("clear", "关闭全部"), EditorStyles.miniButtonRight))
            {
                Undo.RecordObject(target, "清除commandList");
                actionCommands_prop.ClearArray();
            }
            btnRect.x -= ActionGUIUtil.middleButtonWidth + ActionGUIUtil.span;
            if (GUI.Button(btnRect, new GUIContent("check", "资源检查"), EditorStyles.miniButton))
            {

            }
            btnRect.x -= ActionGUIUtil.middleButtonWidth + ActionGUIUtil.span;
            if (GUI.Button(btnRect, new GUIContent("export", "导出步骤"), EditorStyles.miniButton))
            {

            }
            btnRect.x -= ActionGUIUtil.middleButtonWidth + ActionGUIUtil.span;
            if (GUI.Button(btnRect, new GUIContent("import", "导入步骤"), EditorStyles.miniButton))
            {

            }
            var labelRect = new Rect(rect.x, rect.y, btnRect.x - rect.x, rect.height);
            EditorGUI.LabelField(labelRect, "commands");
        }

        private float CommandElementHeight(int index)
        {
            var prop = actionCommands_prop.GetArrayElementAtIndex(index);
            if (prop.isExpanded)
            {
                return 2 * EditorGUIUtility.singleLineHeight + 2 * ActionGUIUtil.span;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight + 2 * ActionGUIUtil.span;
            }
        }

        private void DrawCommandItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = DrawBoxRect(rect,index.ToString());

            var prop = actionCommands_prop.GetArrayElementAtIndex(index);
            var commandProp = prop.FindPropertyRelative("command");
            var ignoreProp = prop.FindPropertyRelative("ignore");
            var commandNameProp = prop.FindPropertyRelative("commandName");

            if (commandProp.objectReferenceValue != null)
            {
                commandNameProp.stringValue = (commandProp.objectReferenceValue as ActionCommand).StepName;
            }

            var btnRect = new Rect(rect.x, rect.y, rect.width - ActionGUIUtil.middleButtonWidth - ActionGUIUtil.smallButtonWidth, EditorGUIUtility.singleLineHeight);
            GUI.contentColor = ignoreProp.boolValue ? ActionGUIUtil. IgnoreColor : ActionGUIUtil. NormalColor;
            if (!string.IsNullOrEmpty(ActionGUIUtil.searchWord)) GUI.contentColor = commandNameProp.stringValue.ToLower().Contains(ActionGUIUtil.searchWord.ToLower()) ? ActionGUIUtil.MatchColor : GUI.contentColor;
            if (GUI.Button(btnRect, commandNameProp.stringValue, EditorStyles.toolbarDropDown))
            {
                prop.isExpanded = !prop.isExpanded;
            }
            GUI.contentColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

            var openGraphRect = new Rect(btnRect.x + btnRect.width + 5, btnRect.y , ActionGUIUtil.smallButtonWidth, btnRect.height);
            if (GUI.Button(openGraphRect, "o", EditorStyles.miniButtonRight) && commandProp.objectReferenceValue != null)
            {
                NodeGraph.NodeGraphWindow.OnOpenAsset(commandProp.objectReferenceInstanceIDValue, 0);
            }

            var objRect = new Rect(rect.x + rect.width - ActionGUIUtil.smallButtonWidth - 10, rect.y + 2, ActionGUIUtil.smallButtonWidth, EditorGUIUtility.singleLineHeight);
            if (commandProp.objectReferenceValue == null)
            {
                commandProp.objectReferenceValue = EditorGUI.ObjectField(objRect, commandProp.objectReferenceValue, typeof(ActionCommand), false);
            }
            else
            {
                if (GUI.Button(objRect, commandContent, EditorStyles.label))
                {
                    EditorGUIUtility.PingObject(commandProp.objectReferenceInstanceIDValue);
                }

                if (Event.current.type == EventType.dragUpdated && objRect.Contains(Event.current.mousePosition))
                {
                    ActionGUIUtil.UpdateDragedObjects(".asset", dragedCommands);
                }

                if (Event.current.type == EventType.DragPerform && objRect.Contains(Event.current.mousePosition))
                {
                    foreach (var item in dragedCommands)
                    {
                        if (!HaveCommand(item as ActionCommand))
                        {
                            commandProp.objectReferenceValue = item;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("警告", "步骤重复,无法添加:" + (item as ActionCommand).StepName, "ok");
                        }
                    }
                }
            }
            if (prop.isExpanded)
            {
                var ignoreRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight);
                ignoreProp.boolValue = EditorGUI.ToggleLeft(ignoreRect, "[ignore]", ignoreProp.boolValue);
            }
        }

        private void DrawCommandList()
        {
            commands_list.DoLayoutList();
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            if (Event.current.type == EventType.dragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".asset", dragedCommands);
            }
            else if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragedCommands)
                {
                    if (!HaveCommand(item))
                    {
                        actionCommands_prop.InsertArrayElementAtIndex(actionCommands_prop.arraySize);
                        var prop = actionCommands_prop.GetArrayElementAtIndex(actionCommands_prop.arraySize - 1);
                        prop.FindPropertyRelative("command").objectReferenceValue = item;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("警告", "步骤重复,无法添加:" + (item as ActionCommand).StepName, "ok");
                    }
                }
            }
        }

        private bool HaveCommand(ActionCommand command)
        {
            for (int i = 0; i < actionCommands_prop.arraySize; i++)
            {
                var prop = actionCommands_prop.GetArrayElementAtIndex(i);
                var cmd = prop.FindPropertyRelative("command");
                if (cmd.objectReferenceValue == command)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion


        private void DrawHeadSwitch(Rect rect)
        {
            var headRect = new Rect(rect.x, rect.y, rect.width * 0.3f, rect.height);
            EditorGUI.BeginChangeCheck();
            GUI.contentColor = ActionGUIUtil.WarningColor;
            secondSelected = EditorGUI.Popup(headRect, secondSelected, SecondSelectables,EditorStyles.miniLabel);
            GUI.contentColor = Color.white;
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(prefer_second_selected,secondSelected);
            }
        }

        private Rect DrawBoxRect(Rect orignalRect,string index)
        {
            var idRect = new Rect(orignalRect.x - ActionGUIUtil.span, orignalRect.y + ActionGUIUtil.span, 20, 20);
            EditorGUI.LabelField(idRect, index.ToString());
            var boxRect = PaddingRect(orignalRect, ActionGUIUtil.span * 0.5f);
            GUI.Box(boxRect, "");
            var rect = PaddingRect(orignalRect);
            return rect;
        }

        //private void UpdateDragedGameObjects()
        //{
        //    dragedGameObject.Clear();
        //    foreach (var item in DragAndDrop.objectReferences)
        //    {
        //        if (item is GameObject)
        //        {
        //            dragedGameObject.Add(item as GameObject);
        //        }
        //        else if (ProjectWindowUtil.IsFolder(item.GetInstanceID()))
        //        {
        //            var folder = AssetDatabase.GetAssetPath(item);
        //            SearchDeep(folder, ".prefab", dragedGameObject);
        //        }
        //    }
        //    DragAndDrop.visualMode = dragedGameObject.Count > 0 ? DragAndDropVisualMode.Move : DragAndDropVisualMode.Rejected;
        //}

        private Rect PaddingRect(Rect orignalRect, float padding = ActionGUIUtil.span)
        {
            var rect = new Rect(orignalRect.x + padding, orignalRect.y + padding, orignalRect.width - padding * 2, orignalRect.height - padding * 2);
            return rect;
        }

        private Rect GetDragRect()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            rect.y -= EditorGUIUtility.singleLineHeight;
            rect.height += EditorGUIUtility.singleLineHeight;
            return rect;
        }

        private void SwitchDrawLists()
        {
            if (selected == 0)
            {
                DrawCommandList();
            }
            else if (selected == 1)
            {
                DrawElements();
            }
        }

        private void DrawElements()
        {
            if (secondSelected == 0)
            {
                DrawAutoElementList();
            }
            else if(secondSelected == 1)
            {
                DrawRuntimeElementList();
            }
            else
            {
                DrawEnviromentList();
            }
        }


    }

}