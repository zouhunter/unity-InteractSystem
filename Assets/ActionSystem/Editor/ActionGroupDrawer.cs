using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace WorldActionSystem.Drawer
{
    [CustomEditor(typeof(ActionGroup))]
    public class ActionGroupDrawer : Editor
    {
        protected SerializedProperty actionCommands_prop;
        protected SerializedProperty runTimeElements_prop;
        protected SerializedProperty autoElements_prop;
        protected SerializedProperty enviroments_prop;

        protected ReorderableList command_list;

        protected const float smallButtonWidth = 20f;
        protected const float middleButtonWidth = 45f;
        protected const float bigButtonWidth = 60f;
        protected const float span = 5;
        protected const string waveText = @"/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\";

        protected GUIContent[] selectables;
        protected GUIContent[] Selectables
        {
            get
            {
                if (selectables == null)
                {
                    selectables = new GUIContent[] {
                        new GUIContent("步骤列表"),
                        new GUIContent("元素列表")
                    };
                }
                return selectables;
            }
        }
        protected int selected;
        protected string searchWord;
        protected GUIContent _commandContent;
        protected GUIContent commandContent
        {
            get
            {
                if (_commandContent == null)
                {
                    var guid = "cce58f60250df8146a074414b8ad53a0";
                    var texture = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(guid));
                    _commandContent = new GUIContent(texture);
                }
                return _commandContent;
            }
        }
        protected Color IgnoreColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.red : Color.black;
            }
        }
        protected Color NormalColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.green : Color.white;
            }
        }
        protected Color MatchColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.cyan : Color.white;
            }
        }
        protected List<ActionCommand> dragedCommands = new List<ActionCommand>();
        private void OnEnable()
        {
            FindPropertys();
            InitReorderLists();
        }

        private void FindPropertys()
        {
            actionCommands_prop = serializedObject.FindProperty("actionCommands");
            runTimeElements_prop = serializedObject.FindProperty("runTimeElements");
            autoElements_prop = serializedObject.FindProperty("autoElements");
            enviroments_prop = serializedObject.FindProperty("enviroments");
        }

        private void InitReorderLists()
        {
            InitCommandReorderList();
        }

        private void InitCommandReorderList()
        {
            command_list = new ReorderableList(serializedObject, actionCommands_prop, true, true, true, true);
            command_list.drawHeaderCallback = DrawCommandHead;
            command_list.elementHeightCallback = CommandElementHeight;
            command_list.drawElementCallback = DrawCommandItem;
        }

        private void DrawCommandHead(Rect rect)
        {
            var btnRect = new Rect(rect.x + rect.width - middleButtonWidth - span, rect.y, middleButtonWidth, rect.height);
            if (GUI.Button(btnRect, new GUIContent("clear", "关闭全部"), EditorStyles.miniButtonRight))
            {
                Undo.RecordObject(target, "清除commandList");
                actionCommands_prop.ClearArray();
            }
            btnRect.x -= middleButtonWidth + span;
            if (GUI.Button(btnRect, new GUIContent("check", "资源检查"), EditorStyles.miniButton))
            {

            }
            btnRect.x -= middleButtonWidth + span;
            if (GUI.Button(btnRect, new GUIContent("export", "导出步骤"), EditorStyles.miniButton))
            {

            }
            btnRect.x -= middleButtonWidth + span;
            if (GUI.Button(btnRect, new GUIContent("import", "导入步骤"), EditorStyles.miniButton))
            {

            }
            var labelRect = new Rect(rect.x, rect.y, btnRect.x - rect.x, rect.height);
            EditorGUI.LabelField(labelRect, waveText);
        }

        private float CommandElementHeight(int index)
        {
            var prop = actionCommands_prop.GetArrayElementAtIndex(index);
            if (prop.isExpanded)
            {
                return 2 * EditorGUIUtility.singleLineHeight + 2 * span;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight + 2 * span;
            }
        }

        private void DrawCommandItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var boxRect = PaddingRect(rect, span * 0.5f);
            GUI.Box(rect, "");
            rect = PaddingRect(rect);
            var prop = actionCommands_prop.GetArrayElementAtIndex(index);
            var commandProp = prop.FindPropertyRelative("command");
            var ignoreProp = prop.FindPropertyRelative("ignore");
            var commandNameProp = prop.FindPropertyRelative("commandName");

            if (commandProp.objectReferenceValue != null)
            {
                commandNameProp.stringValue = (commandProp.objectReferenceValue as ActionCommand).StepName;
            }

            var btnRect = new Rect(rect.x, rect.y, rect.width - middleButtonWidth - smallButtonWidth, EditorGUIUtility.singleLineHeight);
            GUI.contentColor = ignoreProp.boolValue ? IgnoreColor : NormalColor;
            if (!string.IsNullOrEmpty(searchWord)) GUI.contentColor = commandNameProp.stringValue.ToLower().Contains(searchWord.ToLower()) ? MatchColor : GUI.contentColor;
            if (GUI.Button(btnRect, commandNameProp.stringValue, EditorStyles.toolbarDropDown))
            {
                prop.isExpanded = !prop.isExpanded;
            }
            GUI.contentColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

            var openGraphRect = new Rect(btnRect.x + btnRect.width + 5, btnRect.y + 2, smallButtonWidth, btnRect.height);
            if (GUI.Button(openGraphRect, "o", EditorStyles.miniButtonRight) && commandProp.objectReferenceValue != null)
            {
                NodeGraph.NodeGraphWindow.OnOpenAsset(commandProp.objectReferenceInstanceIDValue, 0);
            }

            var objRect = new Rect(rect.x + rect.width - smallButtonWidth - 10, rect.y + 2, smallButtonWidth, EditorGUIUtility.singleLineHeight);
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
                    UpdateDragedCommands();
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
                ignoreProp.boolValue = EditorGUI.ToggleLeft(ignoreRect, "ignore", ignoreProp.boolValue);
            }
        }

        private void UpdateDragedCommands()
        {
            dragedCommands.Clear();
            foreach (var item in DragAndDrop.objectReferences)
            {
                if (item is ActionCommand)
                {
                    dragedCommands.Add(item as ActionCommand);
                }
                else if (ProjectWindowUtil.IsFolder(item.GetInstanceID()))
                {
                    var folder = AssetDatabase.GetAssetPath(item);
                    SearchDeep(folder, dragedCommands);
                }
            }
            DragAndDrop.visualMode = dragedCommands.Count > 0 ? DragAndDropVisualMode.Move : DragAndDropVisualMode.Rejected;
        }

        private static void SearchDeep<T>(string folder, List<T> list) where T : UnityEngine.ScriptableObject
        {
            var files = System.IO.Directory.GetFiles(folder,"*.asset",System.IO.SearchOption.AllDirectories);
            foreach (var filePath in files)
            {
                var root = System.IO.Path.GetPathRoot(filePath);

                if (filePath.EndsWith(".asset"))
                {
                    var path = filePath.Substring(root.Length);
                    var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                    if (asset != null)
                    {
                        list.Add(asset);
                    }
                }
            }
        }


        private Rect PaddingRect(Rect orignalRect, float padding = span)
        {

            var rect = new Rect(orignalRect.x + padding, orignalRect.y + padding, orignalRect.width - padding * 2, orignalRect.height - padding * 2);
            return rect;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawSwitchOptions();
            DrawSearchField();
            SwitchDrawLists();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSwitchOptions()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            selected = GUI.Toolbar(rect, selected, Selectables);
        }

        private void DrawSearchField()
        {
            searchWord = EditorGUILayout.TextField(searchWord);
        }

        private void SwitchDrawLists()
        {
            if (selected == 0)
            {
                DrawCommandList();
            }
            else if (selected == 1)
            {

            }
        }

        private void DrawCommandList()
        {
            command_list.DoLayoutList();
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            if (Event.current.type == EventType.dragUpdated && rect.Contains(Event.current.mousePosition))
            {
                UpdateDragedCommands();
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
    }

}