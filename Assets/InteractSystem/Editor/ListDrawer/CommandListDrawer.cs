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
    public class CommandListDrawer : ReorderListDrawer
    {
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

        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());

            var prop = property.GetArrayElementAtIndex(index);
            var commandProp = prop.FindPropertyRelative("command");
            var ignoreProp = prop.FindPropertyRelative("ignore");
            var commandNameProp = prop.FindPropertyRelative("commandName");

            if (commandProp.objectReferenceValue != null)
            {
                commandNameProp.stringValue = (commandProp.objectReferenceValue as ActionCommand).StepName;
            }

            var btnRect = new Rect(rect.x, rect.y, rect.width - ActionGUIUtil.middleButtonWidth - ActionGUIUtil.smallButtonWidth, EditorGUIUtility.singleLineHeight);
            GUI.contentColor = ignoreProp.boolValue ? ActionGUIUtil.IgnoreColor : ActionGUIUtil.NormalColor;
            if (!string.IsNullOrEmpty(ActionGUIUtil.searchWord)) GUI.contentColor = commandNameProp.stringValue.ToLower().Contains(ActionGUIUtil.searchWord.ToLower()) ? ActionGUIUtil.MatchColor : GUI.contentColor;
            if (GUI.Button(btnRect, commandNameProp.stringValue, EditorStyles.toolbarDropDown))
            {
                prop.isExpanded = !prop.isExpanded;
            }
            GUI.contentColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

            var openGraphRect = new Rect(btnRect.x + btnRect.width + 5, btnRect.y, ActionGUIUtil.smallButtonWidth, btnRect.height);
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

        protected override void DrawHeaderCallBack(Rect rect)
        {

            var btnRect = new Rect(rect.x + rect.width - ActionGUIUtil.middleButtonWidth - ActionGUIUtil.padding, rect.y, ActionGUIUtil.middleButtonWidth, rect.height);
            if (GUI.Button(btnRect, new GUIContent("clear", "关闭全部"), EditorStyles.miniButtonRight))
            {
                Undo.RecordObject(property.serializedObject.targetObject, "清除commandList");
                property.ClearArray();
            }
            btnRect.x -= ActionGUIUtil.middleButtonWidth + ActionGUIUtil.padding;
            if (GUI.Button(btnRect, new GUIContent("check", "资源检查"), EditorStyles.miniButton))
            {

            }
            btnRect.x -= ActionGUIUtil.middleButtonWidth + ActionGUIUtil.padding;
            if (GUI.Button(btnRect, new GUIContent("export", "导出步骤"), EditorStyles.miniButton))
            {

            }
            btnRect.x -= ActionGUIUtil.middleButtonWidth + ActionGUIUtil.padding;
            if (GUI.Button(btnRect, new GUIContent("import", "导入步骤"), EditorStyles.miniButton))
            {

            }
            var labelRect = new Rect(rect.x, rect.y, btnRect.x - rect.x, rect.height);
            EditorGUI.LabelField(labelRect, "commands");
        }

        protected override float ElementHeightCallback(int index)
        {
            var prop = property.GetArrayElementAtIndex(index);
            if (prop.isExpanded)
            {
                return 2 * EditorGUIUtility.singleLineHeight + 2 * ActionGUIUtil.padding;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight + 2 * ActionGUIUtil.padding;
            }
        }
        public override void DoLayoutList()
        {
            base.DoLayoutList();
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
                        property.InsertArrayElementAtIndex(property.arraySize);
                        var prop = property.GetArrayElementAtIndex(property.arraySize - 1);
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
            for (int i = 0; i < property.arraySize; i++)
            {
                var prop = property.GetArrayElementAtIndex(i);
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